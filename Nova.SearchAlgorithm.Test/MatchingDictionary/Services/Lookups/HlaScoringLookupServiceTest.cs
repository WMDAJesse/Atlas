﻿using FluentAssertions;
using Nova.HLAService.Client.Models;
using Nova.SearchAlgorithm.MatchingDictionary.Models.HLATypings;
using Nova.SearchAlgorithm.MatchingDictionary.Models.Lookups;
using Nova.SearchAlgorithm.MatchingDictionary.Models.Lookups.ScoringLookup;
using Nova.SearchAlgorithm.MatchingDictionary.Repositories;
using Nova.SearchAlgorithm.MatchingDictionary.Repositories.AzureStorage;
using Nova.SearchAlgorithm.MatchingDictionary.Services;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.SearchAlgorithm.Test.MatchingDictionary.Services.Lookups
{
    [TestFixture]
    public class HlaScoringLookupServiceTest :
        HlaSearchingLookupServiceTestBase<IHlaScoringLookupRepository, IHlaScoringLookupService, IHlaScoringLookupResult>
    {
        [SetUp]
        public void HlaScoringLookupServiceTest_SetUpBeforeEachTest()
        {
            LookupService = new HlaScoringLookupService(
                HlaLookupRepository,
                AlleleNamesLookupService,
                HlaCategorisationService,
                AlleleStringSplitterService,
                Cache);
        }

        [Test]
        public async Task GetHlaLookupResult_WhenAlleleNameMapsToMultipleAlleles_ScoringInfoForAllAllelesIsReturned()
        {
            const string expectedLookupName = "999:999";
            const string firstAlleleName = "999:999:01";
            const string secondAlleleName = "999:999:02";

            HlaCategorisationService
                .GetHlaTypingCategory(expectedLookupName)
                .Returns(HlaTypingCategory.Allele);

            var expectedCurrentAlleleNames = new List<string> {firstAlleleName, secondAlleleName};
            AlleleNamesLookupService
                .GetCurrentAlleleNames(MatchedLocus, expectedLookupName, Arg.Any<string>())
                .Returns(expectedCurrentAlleleNames);

            // return null on submitted name to emulate scenario that requires a current name lookup
            HlaLookupRepository
                .GetHlaLookupTableEntityIfExists(
                    MatchedLocus,
                    expectedLookupName,
                    TypingMethod.Molecular,
                    Arg.Any<string>())
                .ReturnsNull();

            // return entries using current names list
            var firstEntry = BuildTableEntityForSingleAllele(firstAlleleName);
            var secondEntry = BuildTableEntityForSingleAllele(secondAlleleName);
            HlaLookupRepository
                .GetHlaLookupTableEntityIfExists(
                    MatchedLocus,
                    Arg.Is<string>(x => x.Equals(firstAlleleName) || x.Equals(secondAlleleName)),
                    TypingMethod.Molecular, Arg.Any<string>())
                .Returns(firstEntry, secondEntry);

            var actualResult = await LookupService.GetHlaLookupResult(MatchedLocus, expectedLookupName, "hla-db-version");
            var expectedResult = BuildMultipleAlleleLookupResult(expectedLookupName, expectedCurrentAlleleNames);

            actualResult.Should().Be(expectedResult);
        }

        [TestCase(HlaTypingCategory.AlleleStringOfSubtypes, "Family:Subtype1/Subtype2", "Family:Subtype1", "Family:Subtype2")]
        [TestCase(HlaTypingCategory.AlleleStringOfNames, "Allele1/Allele2", "Allele1", "Allele2")]
        public async Task GetHlaLookupResult_WhenAlleleString_ScoringInfoForAllAllelesIsReturned(
            HlaTypingCategory category, string expectedLookupName, string firstAlleleName, string secondAlleleName)
        {
            HlaCategorisationService
                .GetHlaTypingCategory(expectedLookupName)
                .Returns(category);

            var expectedAlleleNames = new List<string> {firstAlleleName, secondAlleleName};
            AlleleStringSplitterService.GetAlleleNamesFromAlleleString(expectedLookupName)
                .Returns(expectedAlleleNames);

            var firstEntry = BuildTableEntityForSingleAllele(firstAlleleName);
            var secondEntry = BuildTableEntityForSingleAllele(secondAlleleName);

            HlaLookupRepository
                .GetHlaLookupTableEntityIfExists(MatchedLocus, Arg.Any<string>(), TypingMethod.Molecular, Arg.Any<string>())
                .Returns(firstEntry, secondEntry);

            var actualResult = await LookupService.GetHlaLookupResult(MatchedLocus, expectedLookupName, "hla-db-version");
            var expectedResult = BuildConsolidatedMolecularLookupResult(expectedLookupName, expectedAlleleNames);

            actualResult.Should().Be(expectedResult);
        }

        protected override HlaLookupTableEntity BuildTableEntityForSingleAllele(string alleleName)
        {
            var scoringInfo = BuildSingleAlleleScoringInfo(alleleName);

            var lookupResult = new HlaScoringLookupResult(
                MatchedLocus,
                alleleName,
                LookupNameCategory.OriginalAllele,
                scoringInfo
            );

            return new HlaLookupTableEntity(lookupResult)
            {
                LookupNameCategoryAsString = LookupNameCategory.OriginalAllele.ToString()
            };
        }

        private IHlaScoringLookupResult BuildMultipleAlleleLookupResult(string lookupName, IEnumerable<string> alleleNames)
        {
            var scoringInfo = new MultipleAlleleScoringInfo(
                alleleNames.Select(BuildSingleAlleleScoringInfo),
                new List<SerologyEntry>());

            return new HlaScoringLookupResult(
                MatchedLocus,
                lookupName,
                LookupNameCategory.MultipleAlleles,
                scoringInfo
            );
        }

        private IHlaScoringLookupResult BuildConsolidatedMolecularLookupResult(string lookupName, IEnumerable<string> alleleNames)
        {
            var scoringInfo = new ConsolidatedMolecularScoringInfo(
                alleleNames.Select(ToPGroup),
                alleleNames.Select(ToGGroup),
                new List<SerologyEntry>());

            return new HlaScoringLookupResult(
                MatchedLocus,
                lookupName,
                LookupNameCategory.MultipleAlleles,
                scoringInfo
            );
        }

        private static SingleAlleleScoringInfo BuildSingleAlleleScoringInfo(string alleleName)
        {
            var scoringInfo = new SingleAlleleScoringInfo(
                alleleName,
                AlleleTypingStatus.GetDefaultStatus(),
                ToPGroup(alleleName),
                ToGGroup(alleleName));

            return scoringInfo;
        }

        private static string ToPGroup(string alleleName)
        {
            return alleleName + "P";
        }

        private static string ToGGroup(string alleleName)
        {
            return alleleName + "G";
        }
    }
}