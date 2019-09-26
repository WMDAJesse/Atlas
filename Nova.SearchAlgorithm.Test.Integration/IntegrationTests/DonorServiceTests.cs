using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Nova.SearchAlgorithm.Client.Models;
using Nova.SearchAlgorithm.Common.Models;
using Nova.SearchAlgorithm.Common.Repositories.DonorRetrieval;
using Nova.SearchAlgorithm.Services.ConfigurationProviders.TransientSqlDatabase.RepositoryFactories;
using Nova.SearchAlgorithm.Services.Donors;
using Nova.SearchAlgorithm.Test.Integration.TestHelpers;
using Nova.SearchAlgorithm.Test.Integration.TestHelpers.Builders;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.SearchAlgorithm.Test.Integration.IntegrationTests
{
    [TestFixture]
    public class DonorServiceTests
    {
        private IDonorService donorService;
        private IDonorInspectionRepository donorInspectionRepository;

        [SetUp]
        public void SetUp()
        {
            var repositoryFactory = DependencyInjection.DependencyInjection.Provider.GetService<IActiveRepositoryFactory>();

            donorService = DependencyInjection.DependencyInjection.Provider.GetService<IDonorService>();
            donorInspectionRepository = repositoryFactory.GetDonorInspectionRepository();
        }

        [Test]
        public async Task CreateOrUpdateDonorBatch_DonorDoesNotExist_CreatesDonorInDatabase()
        {
            var inputDonor = new InputDonorBuilder(DonorIdGenerator.NextId()).Build();
            await donorService.CreateOrUpdateDonorBatch(new[] { inputDonor });

            var donor = await donorInspectionRepository.GetDonor(inputDonor.DonorId);
            donor.Should().NotBeNull();
        }

        [Test]
        public async Task CreateOrUpdateDonorBatch_DonorDoesNotExist_PopulatesPGroupsForDonorHla()
        {
            var inputDonor = new InputDonorBuilder(DonorIdGenerator.NextId()).Build();
            await donorService.CreateOrUpdateDonorBatch(new[] { inputDonor });

            var pGroupCount = await GetPGroupCount(inputDonor.DonorId, Locus.A, TypePosition.One);
            pGroupCount.Should().NotBe(0);
        }

        [Test]
        public async Task CreateOrUpdateDonorBatch_WhenCalledMultipleTimesForADonor_DoesNotCreateMultipleDonorsWithTheSameId()
        {
            var inputDonor = new InputDonorBuilder(DonorIdGenerator.NextId()).Build();

            await donorService.CreateOrUpdateDonorBatch(new[] { inputDonor });
            await donorService.CreateOrUpdateDonorBatch(new[] { inputDonor });
            await donorService.CreateOrUpdateDonorBatch(new[] { inputDonor });

            var donors = await donorInspectionRepository.GetDonors(new[] { inputDonor.DonorId });
            donors.Count().Should().Be(1);
        }

        [Test]
        public async Task CreateOrUpdateDonorBatch_DonorExists_DoesNotCreateANewDonorWithTheSameDonorId()
        {
            var donorId = DonorIdGenerator.NextId();
            var inputDonor = new InputDonorBuilder(donorId).WithDonorType(DonorType.Adult).Build();
            await donorService.CreateOrUpdateDonorBatch(new[] { inputDonor });

            var updatedDonor = new InputDonorBuilder(donorId).WithDonorType(DonorType.Cord).Build();
            await donorService.CreateOrUpdateDonorBatch(new[] { updatedDonor });

            var donors = await donorInspectionRepository.GetDonors(new[] { donorId });
            donors.Count().Should().Be(1);
        }

        [Test]
        public async Task CreateOrUpdateDonorBatch_DonorExists_UpdatesDonorDetailsInDatabase()
        {
            var donorId = DonorIdGenerator.NextId();
            var inputDonor = new InputDonorBuilder(donorId).WithDonorType(DonorType.Adult).Build();
            await donorService.CreateOrUpdateDonorBatch(new[] { inputDonor });

            const DonorType newDonorType = DonorType.Cord;
            var updatedDonor = new InputDonorBuilder(donorId).WithDonorType(newDonorType).Build();
            await donorService.CreateOrUpdateDonorBatch(new[] { updatedDonor });

            var donor = await donorInspectionRepository.GetDonor(donorId);
            donor.DonorType.Should().Be(newDonorType);
        }

        [Test]
        public async Task CreateOrUpdateDonorBatch_DonorExists_ReprocessesHla()
        {
            var donorId = DonorIdGenerator.NextId();
            const Locus locus = Locus.A;
            const TypePosition position = TypePosition.One;

            var inputDonor = new InputDonorBuilder(donorId).WithHlaAtLocus(locus, position, "*01:01").Build();
            await donorService.CreateOrUpdateDonorBatch(new[] { inputDonor });
            var initialPGroupsCount = await GetPGroupCount(donorId, locus, position);

            // XX code will always have more p-groups than a single allele
            var updatedDonor = new InputDonorBuilder(donorId).WithHlaAtLocus(locus, position, "*01:XX").Build();

            await donorService.CreateOrUpdateDonorBatch(new[] { updatedDonor });
            var updatedPGroupsCount = await GetPGroupCount(donorId, locus, position);

            updatedPGroupsCount.Should().BeGreaterThan(initialPGroupsCount);
        }

        [Test]
        public async Task CreateOrUpdateDonorBatch_CreatesMultipleDonorsInDatabase()
        {
            var inputDonor1 = new InputDonorBuilder(DonorIdGenerator.NextId()).Build();
            var inputDonor2 = new InputDonorBuilder(DonorIdGenerator.NextId()).Build();
            await donorService.CreateOrUpdateDonorBatch(new[] { inputDonor1, inputDonor2
            });

            var donor1 = await donorInspectionRepository.GetDonor(inputDonor1.DonorId);
            var donor2 = await donorInspectionRepository.GetDonor(inputDonor2.DonorId);
            donor1.Should().NotBeNull();
            donor2.Should().NotBeNull();
        }

        [Test]
        public async Task CreateOrUpdateDonorBatch_PopulatesPGroupsForMultipleDonors()
        {
            var inputDonor1 = new InputDonorBuilder(DonorIdGenerator.NextId()).Build();
            var inputDonor2 = new InputDonorBuilder(DonorIdGenerator.NextId()).Build();
            await donorService.CreateOrUpdateDonorBatch(new[] { inputDonor1, inputDonor2 });

            var pGroupCounts = (await GetPGroupCounts(new[] { inputDonor1.DonorId, inputDonor2.DonorId }, Locus.A, TypePosition.One)).ToList();
            pGroupCounts.First().Should().NotBe(0);
            pGroupCounts.Last().Should().NotBe(0);
        }

        [Test]
        public async Task CreateOrUpdateDonorBatch_UpdatesMultipleDonorDetailsInDatabase()
        {
            const DonorType oldDonorType = DonorType.Adult;
            var donorId1 = DonorIdGenerator.NextId();
            var donorId2 = DonorIdGenerator.NextId();
            var inputDonor1 = new InputDonorBuilder(donorId1).WithDonorType(oldDonorType).Build();
            var inputDonor2 = new InputDonorBuilder(donorId2).WithDonorType(oldDonorType).Build();
            await donorService.CreateOrUpdateDonorBatch(new[] { inputDonor1, inputDonor2 });

            const DonorType newDonorType = DonorType.Cord;
            var updatedDonor1 = new InputDonorBuilder(donorId1).WithDonorType(newDonorType).Build();
            var updatedDonor2 = new InputDonorBuilder(donorId2).WithDonorType(newDonorType).Build();
            await donorService.CreateOrUpdateDonorBatch(new[] { updatedDonor1, updatedDonor2 });

            var donor1 = await donorInspectionRepository.GetDonor(donorId1);
            var donor2 = await donorInspectionRepository.GetDonor(donorId2);
            donor1.DonorType.Should().Be(newDonorType);
            donor2.DonorType.Should().Be(newDonorType);
        }

        [Test]
        public async Task CreateOrUpdateDonorBatch_ReprocessesMultipleDonorsHla()
        {
            var donorId1 = DonorIdGenerator.NextId();
            var donorId2 = DonorIdGenerator.NextId();
            const Locus locus = Locus.A;
            const TypePosition position = TypePosition.One;

            var inputDonor1 = new InputDonorBuilder(donorId1).WithHlaAtLocus(locus, position, "*01:01").Build();
            var inputDonor2 = new InputDonorBuilder(donorId2).WithHlaAtLocus(locus, position, "*01:01:01").Build();
            await donorService.CreateOrUpdateDonorBatch(new[] { inputDonor1, inputDonor2 });
            var initialPGroupsCounts = (await GetPGroupCounts(new[] { donorId1, donorId2 }, locus, position)).ToList();

            // XX code will always have more p-groups than a single allele
            var updatedDonor1 = new InputDonorBuilder(donorId1).WithHlaAtLocus(locus, position, "*01:XX").Build();
            var updatedDonor2 = new InputDonorBuilder(donorId2).WithHlaAtLocus(locus, position, "*01:XX").Build();

            await donorService.CreateOrUpdateDonorBatch(new[] { updatedDonor1, updatedDonor2 });
            var updatedPGroupsCounts = (await GetPGroupCounts(new[] { donorId1, donorId2 }, locus, position)).ToList();

            updatedPGroupsCounts.First().Should().BeGreaterThan(initialPGroupsCounts.First());
            updatedPGroupsCounts.Last().Should().BeGreaterThan(initialPGroupsCounts.Last());
        }

        [Test]
        public async Task CreateOrUpdateDonorBatch_CreatesNewAndUpdatesExistingMultipleDonorDetailsInDatabase()
        {
            const DonorType oldDonorType = DonorType.Adult;
            var donorId1 = DonorIdGenerator.NextId();
            var donorId2 = DonorIdGenerator.NextId();
            var inputDonor1 = new InputDonorBuilder(donorId1).WithDonorType(oldDonorType).Build();
            var inputDonor2 = new InputDonorBuilder(donorId2).WithDonorType(oldDonorType).Build();
            await donorService.CreateOrUpdateDonorBatch(new[] { inputDonor1, inputDonor2 });

            var donorId3 = DonorIdGenerator.NextId();
            var donorId4 = DonorIdGenerator.NextId();
            var inputDonor3 = new InputDonorBuilder(donorId3).WithDonorType(oldDonorType).Build();
            var inputDonor4 = new InputDonorBuilder(donorId4).WithDonorType(oldDonorType).Build();

            const DonorType newDonorType = DonorType.Cord;
            var updatedDonor1 = new InputDonorBuilder(donorId1).WithDonorType(newDonorType).Build();
            var updatedDonor2 = new InputDonorBuilder(donorId2).WithDonorType(newDonorType).Build();

            await donorService.CreateOrUpdateDonorBatch(new[] { updatedDonor1, updatedDonor2, inputDonor3, inputDonor4 });

            var donor1 = await donorInspectionRepository.GetDonor(donorId1);
            var donor2 = await donorInspectionRepository.GetDonor(donorId2);
            var donor3 = await donorInspectionRepository.GetDonor(donorId3);
            var donor4 = await donorInspectionRepository.GetDonor(donorId4);
            donor1.DonorType.Should().Be(newDonorType);
            donor2.DonorType.Should().Be(newDonorType);
            donor3.Should().NotBeNull();
            donor4.Should().NotBeNull();
        }

        [Test]
        public async Task CreateOrUpdateDonorBatch_DonorsDoNotExist_CreatesDonorBatchAsAvailableForSearch()
        {
            var inputDonor1 = new InputDonorBuilder(DonorIdGenerator.NextId()).Build();
            var inputDonor2 = new InputDonorBuilder(DonorIdGenerator.NextId()).Build();
            await donorService.CreateOrUpdateDonorBatch(new[] { inputDonor1, inputDonor2 });

            var donors = (await donorInspectionRepository.GetDonors(new[] { inputDonor1.DonorId, inputDonor2.DonorId })).ToList();
            donors.First().Value.IsAvailableForSearch.Should().BeTrue();
            donors.Last().Value.IsAvailableForSearch.Should().BeTrue();
        }

        [Test]
        public async Task SetDonorBatchAsUnavailableForSearch_SetsDonorBatchAsUnavailableForSearch()
        {
            var inputDonor1 = new InputDonorBuilder(DonorIdGenerator.NextId()).Build();
            var inputDonor2 = new InputDonorBuilder(DonorIdGenerator.NextId()).Build();
            var inputDonorIds = new[] { inputDonor1.DonorId, inputDonor2.DonorId };
            await donorService.CreateOrUpdateDonorBatch(new[] { inputDonor1, inputDonor2 });

            await donorService.SetDonorBatchAsUnavailableForSearch(inputDonorIds);

            var donors = (await donorInspectionRepository.GetDonors(inputDonorIds)).ToList();
            donors.First().Value.IsAvailableForSearch.Should().BeFalse();
            donors.Last().Value.IsAvailableForSearch.Should().BeFalse();
        }

        [Test]
        public async Task CreateOrUpdateDonorBatch_DonorsExist_SetsDonorBatchAsAvailableForSearch()
        {
            // Arrange: first create donors, then set as unavailable
            var inputDonor1 = new InputDonorBuilder(DonorIdGenerator.NextId()).Build();
            var inputDonor2 = new InputDonorBuilder(DonorIdGenerator.NextId()).Build();
            var inputDonors = new[] { inputDonor1, inputDonor2 };
            var inputDonorIds = new[] { inputDonor1.DonorId, inputDonor2.DonorId };
            await donorService.CreateOrUpdateDonorBatch(inputDonors);
            await donorService.SetDonorBatchAsUnavailableForSearch(inputDonorIds);

            await donorService.CreateOrUpdateDonorBatch(inputDonors);

            var donors = (await donorInspectionRepository.GetDonors(inputDonorIds)).ToList();
            donors.First().Value.IsAvailableForSearch.Should().BeTrue();
            donors.Last().Value.IsAvailableForSearch.Should().BeTrue();
        }

        [Test]
        public async Task CreateOrUpdateDonorBatch_DonorExists_SetsDonorAsAvailableForSearchAndUpdatesDonorDetailsAndReprocessesHla()
        {
            var donorId = DonorIdGenerator.NextId();
            const DonorType oldDonorType = DonorType.Adult;
            const DonorType newDonorType = DonorType.Cord;

            // XX code will always have more p-groups than a single allele
            const string oldHla = "*01:01";
            const string newHla = "*01:XX";
            const Locus locus = Locus.A;
            const TypePosition position = TypePosition.One;

            var inputDonor = new InputDonorBuilder(donorId)
                .WithDonorType(oldDonorType)
                .WithHlaAtLocus(locus, position, oldHla)
                .Build();
            await donorService.CreateOrUpdateDonorBatch(new[] { inputDonor });
            var initialPGroupsCount = await GetPGroupCount(donorId, locus, position);

            var updatedDonor = new InputDonorBuilder(donorId)
                .WithDonorType(newDonorType)
                .WithHlaAtLocus(locus, position, newHla)
                .Build();
            await donorService.CreateOrUpdateDonorBatch(new[] { updatedDonor });

            var donor = await donorInspectionRepository.GetDonor(donorId);
            var updatedPGroupsCount = await GetPGroupCount(donorId, locus, position);

            donor.IsAvailableForSearch.Should().BeTrue();
            donor.DonorType.Should().Be(newDonorType);
            updatedPGroupsCount.Should().BeGreaterThan(initialPGroupsCount);
        }

        private async Task<int> GetPGroupCount(int donorId, Locus locus, TypePosition position)
        {
            var counts = await GetPGroupCounts(new[] { donorId }, locus, position);
            return counts.Single();
        }

        private async Task<IEnumerable<int>> GetPGroupCounts(IEnumerable<int> donorIds, Locus locus, TypePosition position)
        {
            var pGroupsForDonor = await donorInspectionRepository.GetPGroupsForDonors(donorIds);
            return pGroupsForDonor.Select(p => p.PGroupNames.DataAtPosition(locus, position).Count());
        }
    }
}