﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Nova.SearchAlgorithm.ApplicationInsights;
using Nova.SearchAlgorithm.Common.Models;
using Nova.SearchAlgorithm.Common.Repositories;
using Nova.SearchAlgorithm.Common.Repositories.DonorUpdates;
using Nova.SearchAlgorithm.MatchingDictionary.Exceptions;
using Nova.SearchAlgorithm.MatchingDictionary.Repositories;
using Nova.SearchAlgorithm.Services.ConfigurationProviders.TransientSqlDatabase;
using Nova.SearchAlgorithm.Services.MatchingDictionary;
using Nova.Utils.ApplicationInsights;

namespace Nova.SearchAlgorithm.Services.DataRefresh
{
    public interface IHlaProcessor
    {
        /// <summary>
        /// For any donors with a higher id than the last updated donor, fetches p-groups for all donor's hla
        /// And stores the pre-processed p-groups for use in matching
        /// </summary>
        Task UpdateDonorHla(string hlaDatabaseVersion);
    }

    public class HlaProcessor : IHlaProcessor
    {
        private const int BatchSize = 1000;
        
        private readonly ILogger logger;
        private readonly IExpandHlaPhenotypeService expandHlaPhenotypeService;
        private readonly IAntigenCachingService antigenCachingService;
        private readonly IDonorImportRepository donorImportRepository;
        private readonly IDataRefreshRepository dataRefreshRepository;
        private readonly IHlaMatchingLookupRepository hlaMatchingLookupRepository;
        private readonly IAlleleNamesLookupRepository alleleNamesLookupRepository;
        private readonly IPGroupRepository pGroupRepository;

        public HlaProcessor(
            ILogger logger,
            IExpandHlaPhenotypeService expandHlaPhenotypeService,
            IAntigenCachingService antigenCachingService,
            ITransientRepositoryFactory repositoryFactory,
            IHlaMatchingLookupRepository hlaMatchingLookupRepository,
            IAlleleNamesLookupRepository alleleNamesLookupRepository)
        {
            this.logger = logger;
            this.expandHlaPhenotypeService = expandHlaPhenotypeService;
            this.antigenCachingService = antigenCachingService;
            donorImportRepository = repositoryFactory.GetDonorImportRepository(false);
            dataRefreshRepository = repositoryFactory.GetDataRefreshRepository(false);
            pGroupRepository = repositoryFactory.GetPGroupRepository(false);
            this.hlaMatchingLookupRepository = hlaMatchingLookupRepository;
            this.alleleNamesLookupRepository = alleleNamesLookupRepository;
        }

        public async Task UpdateDonorHla(string hlaDatabaseVersion)
        {
            try
            {
                await PerformUpfrontSetup(hlaDatabaseVersion);
            }
            catch (Exception e)
            {
                logger.SendEvent(new HlaRefreshSetUpFailureEventModel(e));
                throw;
            }

            try
            {
                var totalDonorCount = await dataRefreshRepository.GetDonorCount();
                var batchedQuery = await dataRefreshRepository.DonorsAddedSinceLastHlaUpdate(BatchSize);
                var donorsProcessed = 0;
                while (batchedQuery.HasMoreResults)
                {
                    var donorBatch = (await batchedQuery.RequestNextAsync()).ToList();
                    
                    // When continuing a donor import there will be some overlap of donors to ensure all donors are processed. 
                    // This ensures we do not end up with duplicate p-groups in the matching hla tables
                    // We do not want to attempt to remove p-groups for all batches as it would be detrimental to performance, so we limit it to the first two batches
                    var shouldRemovePGroups = donorsProcessed < 2 * BatchSize;
                    
                    await UpdateDonorBatch(donorBatch, hlaDatabaseVersion, shouldRemovePGroups);
                    donorsProcessed += BatchSize;
                    logger.SendTrace($"Hla Processing {(double) donorsProcessed/totalDonorCount:0.00%} complete", LogLevel.Info);
                }
            }
            catch (Exception e)
            {
                logger.SendEvent(new HlaRefreshFailureEventModel(e));
                throw;
            }
            finally
            {
                await PerformTearDown();
            }
        }

        private async Task UpdateDonorBatch(IEnumerable<DonorResult> donorBatch, string hlaDatabaseVersion, bool shouldRemovePGroups)
        {
            donorBatch = donorBatch.ToList();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (shouldRemovePGroups)
            {
                await donorImportRepository.RemovePGroupsForDonorBatch(donorBatch.Select(d => d.DonorId));
            }

            var donorHlaData = await Task.WhenAll(donorBatch.Select(d => FetchDonorHlaData(d, hlaDatabaseVersion)));
            var inputDonors = donorHlaData.Where(x => x != null).ToList();
            await donorImportRepository.AddMatchingPGroupsForExistingDonorBatch(inputDonors);

            stopwatch.Stop();
            logger.SendTrace("Updated Donors", LogLevel.Info, new Dictionary<string, string>
            {
                {"NumberOfDonors", inputDonors.Count.ToString()},
                {"UpdateTime", stopwatch.ElapsedMilliseconds.ToString()}
            });
        }

        private async Task PerformUpfrontSetup(string hlaDatabaseVersion)
        {
            logger.SendTrace("HLA PROCESSOR: caching matching dictionary tables", LogLevel.Info);
            // Cloud tables are cached for performance reasons - this must be done upfront to avoid multiple tasks attempting to set up the cache
            await hlaMatchingLookupRepository.LoadDataIntoMemory(hlaDatabaseVersion);
            await alleleNamesLookupRepository.LoadDataIntoMemory(hlaDatabaseVersion);

            logger.SendTrace("HLA PROCESSOR: caching antigens from hla service", LogLevel.Info);
            // All antigens are fetched from the HLA service. We use our cache for NMDP lookups to avoid too much load on the hla service
            await antigenCachingService.GenerateAntigenCache();

            logger.SendTrace("HLA PROCESSOR: inserting new p groups to database", LogLevel.Info);
            // P Groups are inserted (when using relational database storage) upfront. All groups are extracted from the matching dictionary, and new ones added to the SQL database
            var pGroups = hlaMatchingLookupRepository.GetAllPGroups();
            pGroupRepository.InsertPGroups(pGroups);

            logger.SendTrace("HLA PROCESSOR: preparing database", LogLevel.Info);
            await donorImportRepository.FullHlaRefreshSetUp();
        }

        private async Task PerformTearDown()
        {
            logger.SendTrace("HLA PROCESSOR: restoring database", LogLevel.Info);
            await donorImportRepository.FullHlaRefreshTearDown();
        }

        private async Task<InputDonorWithExpandedHla> FetchDonorHlaData(DonorResult donor, string hlaDatabaseVersion)
        {
            try
            {
                var matchingHla = await expandHlaPhenotypeService.GetPhenotypeOfExpandedHla(donor.HlaNames, hlaDatabaseVersion);

                return new InputDonorWithExpandedHla
                {
                    DonorId = donor.DonorId,
                    DonorType = donor.DonorType,
                    RegistryCode = donor.RegistryCode,
                    MatchingHla = matchingHla,
                };
            }
            catch (MatchingDictionaryException e)
            {
                logger.SendEvent(new HlaRefreshMatchingDictionaryLookupFailureEventModel(e, donor.DonorId.ToString()));
                return null;
            }
        }
    }
}