﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Common.GeneticData;
using Atlas.HlaMetadataDictionary.Test.IntegrationTests.TestHelpers.FileBackedStorageStubs;
using Atlas.MatchingAlgorithm.Data.Models.DonorInfo;
using Atlas.MatchingAlgorithm.Data.Repositories.DonorRetrieval;
using Atlas.MatchingAlgorithm.Data.Repositories.DonorUpdates;
using Atlas.MatchingAlgorithm.Services.ConfigurationProviders.TransientSqlDatabase.RepositoryFactories;
using Atlas.MatchingAlgorithm.Services.DataRefresh.HlaProcessing;
using Atlas.MatchingAlgorithm.Test.Integration.TestHelpers.Builders;
using Atlas.MatchingAlgorithm.Test.Integration.TestHelpers.Repositories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Atlas.MatchingAlgorithm.Test.Integration.IntegrationTests.Import
{
    public class HlaProcessorTests
    {
        private IDonorImportRepository importRepo;
        private IDonorInspectionRepository inspectionRepo;
        private ITestDataRefreshHistoryRepository dataRefreshHistoryRepository;
        private IHlaProcessor processor;

        private int refreshRecordId;
        private const string DefaultHlaNomenclatureVersion = FileBackedHlaMetadataRepositoryBaseReader.OlderTestHlaVersion;

        [SetUp]
        public void SetUp()
        {
            dataRefreshHistoryRepository = DependencyInjection.DependencyInjection.Provider.GetService<ITestDataRefreshHistoryRepository>();

            refreshRecordId = dataRefreshHistoryRepository.InsertDummySuccessfulRefreshRecord(FileBackedHlaMetadataRepositoryBaseReader.OlderTestHlaVersion);

            var repositoryFactory = DependencyInjection.DependencyInjection.Provider.GetService<IDormantRepositoryFactory>();
            importRepo = repositoryFactory.GetDonorImportRepository();
            // We want to inspect the dormant database, as this is what the import will have run on
            inspectionRepo = repositoryFactory.GetDonorInspectionRepository();
            processor = DependencyInjection.DependencyInjection.Provider.GetService<IHlaProcessor>();
        }

        [Test]
        public async Task UpdateDonorHla_DoesNotChangeStoredDonorInformation()
        {
            var donorInfo = new DonorInfoBuilder().Build();
            await importRepo.InsertBatchOfDonors(new List<DonorInfo> { donorInfo });

            await processor.UpdateDonorHla(DefaultHlaNomenclatureVersion, refreshRecordId);

            var storedDonor = await inspectionRepo.GetDonor(donorInfo.DonorId);
            AssertStoredDonorInfoMatchesOriginalDonorInfo(storedDonor, donorInfo);
        }

        [Test]
        public async Task UpdateDonorHla_DonorHlaHasMultiplePGroups_InsertsRowForEachPGroup()
        {
            // We know the number of p-groups for a given hla string from the in-memory metadata dictionary.
            // If the underlying data changes, this may become incorrect.
            const string hlaWithKnownPGroups = "01:XX";
            const int expectedPGroupCount = 213;

            var donorInfo = new DonorInfoBuilder()
                    .WithHlaAtLocus(Locus.A, LocusPosition.One, hlaWithKnownPGroups)
                    .Build();

            await importRepo.InsertBatchOfDonors(new List<DonorInfo> { donorInfo });

            await processor.UpdateDonorHla(DefaultHlaNomenclatureVersion, refreshRecordId);

            var actualPGroupCount = await GetPGroupCountAtLocusAPositionOne(donorInfo.DonorId);
            actualPGroupCount.Should().Be(expectedPGroupCount);
        }
        
        [Test]
        public async Task UpdateDonorHla_WhenHlaUpdateIsContinued_DoesNotAddMorePGroups()
        {
            var donorInfo = new DonorInfoBuilder().Build();
            await importRepo.InsertBatchOfDonors(new List<DonorInfo> { donorInfo });

            await processor.UpdateDonorHla(DefaultHlaNomenclatureVersion, refreshRecordId, null, false);

            var initialPGroupCount = await GetPGroupCountAtLocusAPositionOne(donorInfo.DonorId);

            await processor.UpdateDonorHla(DefaultHlaNomenclatureVersion, refreshRecordId, null, true);

            var finalPGroupCount = await GetPGroupCountAtLocusAPositionOne(donorInfo.DonorId);
            finalPGroupCount.Should().Be(initialPGroupCount);
        }

        [Test]
        public async Task UpdateDonorHla_WhenHlaUpdateIsContinued_ButIsToldItsANewRun_ReAddsPGroups()
        {
            var donorInfo = new DonorInfoBuilder().Build();
            await importRepo.InsertBatchOfDonors(new List<DonorInfo> { donorInfo });

            await processor.UpdateDonorHla(DefaultHlaNomenclatureVersion, refreshRecordId);

            var initialPGroupCount = await GetPGroupCountAtLocusAPositionOne(donorInfo.DonorId);

            await processor.UpdateDonorHla(DefaultHlaNomenclatureVersion, refreshRecordId);

            var finalPGroupCount = await GetPGroupCountAtLocusAPositionOne(donorInfo.DonorId);
            finalPGroupCount.Should().Be(initialPGroupCount * 2);
        }

        [Test]
        public async Task UpdateDonorHla_UpdatesHlaForDonorsInsertedSinceLastRun()
        {
            var donorInfo = new DonorInfoBuilder().Build();
            await importRepo.InsertBatchOfDonors(new List<DonorInfo> { donorInfo });
            await processor.UpdateDonorHla(DefaultHlaNomenclatureVersion, refreshRecordId);

            var newDonor = new DonorInfoBuilder().Build();
            await importRepo.InsertBatchOfDonors(new List<DonorInfo> { newDonor });
            await processor.UpdateDonorHla(DefaultHlaNomenclatureVersion, refreshRecordId);

            var pGroupCount = await GetPGroupCountAtLocusAPositionOne(newDonor.DonorId);
            pGroupCount.Should().NotBeNull();
        }

        private static void AssertStoredDonorInfoMatchesOriginalDonorInfo(DonorInfo donorInfoActual, DonorInfo donorInfoExpected)
        {
            donorInfoActual.DonorId.Should().Be(donorInfoExpected.DonorId);
            donorInfoActual.DonorType.Should().Be(donorInfoExpected.DonorType);
            donorInfoActual.IsAvailableForSearch.Should().Be(donorInfoExpected.IsAvailableForSearch);
            donorInfoActual.HlaNames.Should().BeEquivalentTo(donorInfoExpected.HlaNames);
        }

        private async Task<int?> GetPGroupCountAtLocusAPositionOne(int donorId)
        {
            var pGroups = await inspectionRepo.GetPGroupsForDonors(new[] { donorId });

            return pGroups.First().PGroupNames.A.Position1?.Count();
        }
    }
}