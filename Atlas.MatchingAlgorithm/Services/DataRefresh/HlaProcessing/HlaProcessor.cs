﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Common.ApplicationInsights;
using Atlas.Common.Notifications;
using Atlas.HlaMetadataDictionary.ExternalInterface;
using Atlas.MatchingAlgorithm.ApplicationInsights;
using Atlas.MatchingAlgorithm.Data.Models.DonorInfo;
using Atlas.MatchingAlgorithm.Data.Repositories;
using Atlas.MatchingAlgorithm.Data.Repositories.DonorUpdates;
using Atlas.MatchingAlgorithm.Models;
using Atlas.MatchingAlgorithm.Services.ConfigurationProviders.TransientSqlDatabase.RepositoryFactories;
using Atlas.MatchingAlgorithm.Services.Donors;
using Atlas.MatchingAlgorithm.Settings;

namespace Atlas.MatchingAlgorithm.Services.DataRefresh.HlaProcessing
{
    public interface IHlaProcessor
    {
        /// <summary>
        /// For any donors with a higher id than the last updated donor:
        ///  - Fetches p-groups for all donor's hla
        ///  - Stores the pre-processed p-groups for use in matching
        /// </summary>
        Task UpdateDonorHla(string hlaNomenclatureVersion);
    }

    public class HlaProcessor : IHlaProcessor
    {
        private const int BatchSize = 2000; // At 1k this definitely works fine. At 4k it's been seen throwing OOM Exceptions
        private const string HlaFailureEventName = "Imported Donor Hla Processing Failure(s) in the Matching Algorithm's DataRefresh";

        private readonly ILogger logger;
        private readonly IDonorHlaExpanderFactory donorHlaExpanderFactory;
        private readonly IHlaMetadataDictionaryFactory hlaMetadataDictionaryFactory;
        private readonly IFailedDonorsNotificationSender failedDonorsNotificationSender;
        private readonly DataRefreshSettings settings;
        private readonly IDonorImportRepository donorImportRepository;
        private readonly IDataRefreshRepository dataRefreshRepository;
        private readonly IPGroupRepository pGroupRepository;

        public HlaProcessor(
            ILogger logger,
            IDonorHlaExpanderFactory donorHlaExpanderFactory,
            IHlaMetadataDictionaryFactory hlaMetadataDictionaryFactory,
            IFailedDonorsNotificationSender failedDonorsNotificationSender,
            IDormantRepositoryFactory repositoryFactory,
            DataRefreshSettings settings)
        {
            this.logger = logger;
            this.donorHlaExpanderFactory = donorHlaExpanderFactory;
            this.hlaMetadataDictionaryFactory = hlaMetadataDictionaryFactory;
            this.failedDonorsNotificationSender = failedDonorsNotificationSender;
            this.settings = settings;
            donorImportRepository = repositoryFactory.GetDonorImportRepository();
            dataRefreshRepository = repositoryFactory.GetDataRefreshRepository();
            pGroupRepository = repositoryFactory.GetPGroupRepository();
        }

        public async Task UpdateDonorHla(string hlaNomenclatureVersion)
        {
            await PerformUpfrontSetup(hlaNomenclatureVersion);

            try
            {
                await PerformHlaUpdate(hlaNomenclatureVersion);
            }
            catch (Exception e)
            {
                logger.SendEvent(new HlaRefreshFailureEventModel(e));
                throw;
            }
        }

        private async Task PerformHlaUpdate(string hlaNomenclatureVersion)
        {
            var totalDonorCount = await dataRefreshRepository.GetDonorCount();
            var batchedQuery = await dataRefreshRepository.DonorsAddedSinceLastHlaUpdate(BatchSize);
            var donorsProcessed = 0;

            var failedDonors = new List<FailedDonorInfo>();

            while (batchedQuery.HasMoreResults)
            {
                var donorBatch = (await batchedQuery.RequestNextAsync()).ToList();
                var donorsInBatch = donorBatch.Count;

                // When continuing a donor import there will be some overlap of donors to ensure all donors are processed. 
                // This ensures we do not end up with duplicate p-groups in the matching hla tables
                // We do not want to attempt to remove p-groups for all batches as it would be detrimental to performance, so we limit it to the first two batches
                var shouldRemovePGroups = donorsProcessed < DataRefreshRepository.NumberOfBatchesOverlapOnRestart * BatchSize;

                var failedDonorsFromBatch = await UpdateDonorBatch(donorBatch, hlaNomenclatureVersion, shouldRemovePGroups);
                failedDonors.AddRange(failedDonorsFromBatch);

                donorsProcessed += donorsInBatch;
                logger.SendTrace($"Hla Processing {Decimal.Divide(donorsProcessed, totalDonorCount):0.00%} complete");
            }

            if (failedDonors.Any())
            {
                await failedDonorsNotificationSender.SendFailedDonorsAlert(failedDonors, HlaFailureEventName, Priority.Low);
            }
        }

        /// <summary>
        /// Fetches Expanded HLA information for all donors in a batch, and stores the processed  information in the database.
        /// </summary>
        /// <param name="donorBatch">The collection of donors to update</param>
        /// <param name="hlaNomenclatureVersion">The version of the HLA Nomenclature to use to fetch expanded HLA information</param>
        /// <param name="shouldRemovePGroups">If set, existing p-groups will be removed before adding new ones.</param>
        /// <returns>A collection of donors that failed the import process.</returns>
        private async Task<IEnumerable<FailedDonorInfo>> UpdateDonorBatch(
            IEnumerable<DonorInfo> donorBatch,
            string hlaNomenclatureVersion,
            bool shouldRemovePGroups)
        {
            donorBatch = donorBatch.ToList();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (shouldRemovePGroups)
            {
                await donorImportRepository.RemovePGroupsForDonorBatch(donorBatch.Select(d => d.DonorId));
            }

            var donorHlaExpander = donorHlaExpanderFactory.BuildForSpecifiedHlaNomenclatureVersion(hlaNomenclatureVersion);
            var hlaExpansionResults = await donorHlaExpander.ExpandDonorHlaBatchAsync(donorBatch, HlaFailureEventName);
            EnsureAllPGroupsExist(hlaExpansionResults.ProcessingResults);
            await donorImportRepository.AddMatchingPGroupsForExistingDonorBatch(hlaExpansionResults.ProcessingResults, settings.DataRefreshDonorUpdatesShouldBeFullyTransactional);

            stopwatch.Stop();
            logger.SendTrace("Updated Donors", LogLevel.Verbose, new Dictionary<string, string>
            {
                {"NumberOfDonors", hlaExpansionResults.ProcessingResults.Count.ToString()},
                {"UpdateTime", stopwatch.ElapsedMilliseconds.ToString()}
            });

            return hlaExpansionResults.FailedDonors;
        }

        /// <remarks>
        /// See notes in FindOrCreatePGroupIds.
        /// In practice this will never do anything in Prod code.
        /// But it means that during tests the DonorUpdate code behaves more like
        /// "the real thing", since the PGroups have already been inserted into the DB.
        /// </remarks>
        private void EnsureAllPGroupsExist(IReadOnlyCollection<DonorInfoWithExpandedHla> donorsWithHlas)
        {
            var allPGroups = donorsWithHlas
                .SelectMany(d =>
                    d.MatchingHla?.ToEnumerable().SelectMany(hla =>
                        hla?.MatchingPGroups ?? new string[0]
                    ) ?? new List<string>()
                ).ToList();

            pGroupRepository.FindOrCreatePGroupIds(allPGroups);
        }

        private async Task PerformUpfrontSetup(string hlaNomenclatureVersion)
        {
            try
            {

                logger.SendTrace("HLA PROCESSOR: caching HlaMetadataDictionary tables");
                // Cloud tables are cached for performance reasons - this must be done upfront to avoid multiple tasks attempting to set up the cache
                var dictionaryCacheControl = hlaMetadataDictionaryFactory.BuildCacheControl(hlaNomenclatureVersion);
                await dictionaryCacheControl.PreWarmAllCaches();

                logger.SendTrace("HLA PROCESSOR: inserting new p groups to database");
                // P Groups are inserted (when using relational database storage) upfront. All groups are extracted from the HlaMetadataDictionary, and new ones added to the SQL database
                var hlaDictionary = hlaMetadataDictionaryFactory.BuildDictionary(hlaNomenclatureVersion);
                var pGroups = await hlaDictionary.GetAllPGroups();
                pGroupRepository.InsertPGroups(pGroups);
            }
            catch (Exception e)
            {
                logger.SendEvent(new HlaRefreshSetUpFailureEventModel(e));
                throw;
            }
        }
    }
}