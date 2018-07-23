﻿using System.Collections.Generic;
using Nova.SearchAlgorithm.MatchingDictionary.Exceptions;
using Nova.SearchAlgorithm.MatchingDictionary.Models.HLATypings;
using Nova.SearchAlgorithm.MatchingDictionary.Repositories;
using Nova.SearchAlgorithm.MatchingDictionary.Services;
using NSubstitute;
using NUnit.Framework;
using System.Threading.Tasks;
using Nova.HLAService.Client.Models;
using Nova.HLAService.Client.Services;
using Nova.SearchAlgorithm.MatchingDictionary.Models.Lookups.AlleleNameLookup;

namespace Nova.SearchAlgorithm.Test.MatchingDictionary.Services.AlleleNames
{
    [TestFixture]
    public class AlleleNamesLookupServiceTest
    {
        private IAlleleNamesLookupService lookupService;
        private IAlleleNamesLookupRepository lookupRepository;
        private IHlaCategorisationService hlaCategorisationService;
        private const MatchLocus MatchedLocus = MatchLocus.A;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            lookupRepository = Substitute.For<IAlleleNamesLookupRepository>();
            hlaCategorisationService = Substitute.For<IHlaCategorisationService>();
            lookupService = new AlleleNamesLookupService(lookupRepository, hlaCategorisationService);
        }

        [SetUp]
        public void SetupBeforeEachTest()
        {
            lookupRepository.ClearReceivedCalls();

            lookupRepository
                .GetAlleleNameIfExists(MatchedLocus, Arg.Any<string>())
                .Returns(new AlleleNameLookupResult(MatchedLocus, "FAKE-ALLELE-TO-PREVENT-INVALID-HLA-EXCEPTION", new List<string>()));
        }

        [TestCase(null)]
        [TestCase("")]
        public void GetCurrentAlleleNames_WhenStringNullOrEmpty_ThrowsException(string nullOrEmptyString)
        {
            Assert.ThrowsAsync<MatchingDictionaryException>(
                async () => await lookupService.GetCurrentAlleleNames(MatchedLocus, nullOrEmptyString));
        }

        [Test]
        public void GetCurrentAlleleNames_WhenNotAlleleTyping_ThrowsException()
        {
            const string notAlleleName = "NOT-AN-ALLELE";
            const HlaTypingCategory notAlleleTypingCategory = HlaTypingCategory.Serology;

            hlaCategorisationService.GetHlaTypingCategory(notAlleleName).Returns(notAlleleTypingCategory);

            Assert.ThrowsAsync<MatchingDictionaryException>(
                async () => await lookupService.GetCurrentAlleleNames(MatchedLocus, notAlleleName));
        }

        [TestCase("*AlleleName", "AlleleName")]
        [TestCase("AlleleName", "AlleleName")]
        public async Task GetCurrentAlleleNames_WhenAlleleTyping_LooksUpTheTrimmedAlleleName(
            string submittedLookupName, string trimmedLookupName)
        {
            hlaCategorisationService.GetHlaTypingCategory(Arg.Any<string>()).Returns(HlaTypingCategory.Allele);

            await lookupService.GetCurrentAlleleNames(MatchedLocus, submittedLookupName);

            await lookupRepository.Received().GetAlleleNameIfExists(MatchedLocus, trimmedLookupName);
        }
    }
}
