﻿using System.Collections.Generic;
using System.Linq;
using Nova.SearchAlgorithm.MatchingDictionary.Models.HLATypings;
using Nova.SearchAlgorithm.MatchingDictionary.Models.MatchingTypings;
using NUnit.Framework;

namespace Nova.SearchAlgorithm.Test.MatchingDictionary.Services.Matching
{
    [TestFixtureSource(typeof(MatchedHlaTestFixtureArgs), nameof(MatchedHlaTestFixtureArgs.MatchedAlleles))]
    public class AlleleToPGroupMatchingTest : MatchedOnTestBase<IAlleleInfoForMatching>
    {
        public AlleleToPGroupMatchingTest(IEnumerable<IAlleleInfoForMatching> matchingAlleles) : base(matchingAlleles)
        {
        }

        [Test]
        public void ValidAlleleTypingsHaveExpectedNumberOfPGroups()
        {
            var pGroupCounts = MatchingTypings
                .Where(m => !m.HlaTyping.IsDeleted)
                .Select(m => new { Allele = m.HlaTyping as AlleleTyping, PGroupCount = m.MatchingPGroups.Count() })
                .ToList();

            var expressed = pGroupCounts.Where(p =>
                !p.Allele.IsNullExpresser && p.PGroupCount != 1);

            var notExpressed = pGroupCounts.Where(p =>
                p.Allele.IsNullExpresser && p.PGroupCount != 0);

            Assert.Multiple(() =>
            {
                Assert.IsFalse(expressed.Any());
                Assert.IsFalse(notExpressed.Any());
            });
        }

        [Test]
        public void ExpressedAllelesHaveCorrectMatchingPGroup()
        {
            var normalAllele = new AlleleInfoForMatching(
                new AlleleTyping("A*", "01:01:01:01"), new AlleleTyping("A*", "01:01:01:01"), new List<string> { "01:01P" });
            var lowAllele = new AlleleInfoForMatching(
                new AlleleTyping("B*", "39:01:01:02L"), new AlleleTyping("B*", "39:01:01:02L"), new List<string> { "39:01P" });
            var questionableAllele = new AlleleInfoForMatching(
                new AlleleTyping("C*", "07:01:01:14Q"), new AlleleTyping("C*", "07:01:01:14Q"), new List<string> { "07:01P" });
            var secretedAllele = new AlleleInfoForMatching(
                new AlleleTyping("B*", "44:02:01:02S"), new AlleleTyping("B*", "44:02:01:02S"), new List<string> { "44:02P" });

            Assert.AreEqual(normalAllele, GetSingleMatchingTyping(MatchLocus.A, "01:01:01:01"));
            Assert.AreEqual(lowAllele, GetSingleMatchingTyping(MatchLocus.B, "39:01:01:02L"));
            Assert.AreEqual(questionableAllele, GetSingleMatchingTyping(MatchLocus.C, "07:01:01:14Q"));
            Assert.AreEqual(secretedAllele, GetSingleMatchingTyping(MatchLocus.B, "44:02:01:02S"));
        }

        [Test]
        public void NonExpressedAlleleHasNoPGroup()
        {
            var nullAllele = new AlleleInfoForMatching(
                new AlleleTyping("A*", "29:01:01:02N"), new AlleleTyping("A*", "29:01:01:02N"), new List<string>());

            Assert.AreEqual(nullAllele, GetSingleMatchingTyping(MatchLocus.A, "29:01:01:02N"));
        }

        [Test]
        public void IdenticalHlaUsedToFindMatchingPGroupsForDeletedAlleles()
        {
            var deletedWithIdentical = new AlleleInfoForMatching(
                new AlleleTyping("A*", "11:53", true), new AlleleTyping("A*", "11:02:01"), new List<string> { "11:02P" });
            var deletedIsNullIdenticalIsExpressing = new AlleleInfoForMatching(
                new AlleleTyping("A*", "01:34N", true), new AlleleTyping("A*", "01:01:38L"), new List<string> { "01:01P" });
            var deletedIsExpressingIdenticalIsNull = new AlleleInfoForMatching(
                new AlleleTyping("A*", "03:260", true), new AlleleTyping("A*", "03:284N"), new List<string>());

            Assert.AreEqual(deletedWithIdentical, GetSingleMatchingTyping(MatchLocus.A, "11:53"));
            Assert.AreEqual(deletedIsNullIdenticalIsExpressing, GetSingleMatchingTyping(MatchLocus.A, "01:34N"));
            Assert.AreEqual(deletedIsExpressingIdenticalIsNull, GetSingleMatchingTyping(MatchLocus.A, "03:260"));
        }

        [Test]
        public void DeletedAlleleWithNoIdenticalHlaHasNoMatchingPGroup()
        {
            var deletedNoIdentical = new AlleleInfoForMatching(
                new AlleleTyping("A*", "02:100", true), new AlleleTyping("A*", "02:100", true), new List<string>());

            Assert.AreEqual(deletedNoIdentical, GetSingleMatchingTyping(MatchLocus.A, "02:100"));
        }

        [Test]
        public void ConfidentialAllelesAreExcludedFromMatchingAllelesList()
        {
            var confidentialAlleles = new List<HlaTyping>
            {
                new HlaTyping("A*", "02:01:01:28"),
                new HlaTyping("B*", "18:37:02"),
                new HlaTyping("B*", "48:43"),
                new HlaTyping("DQB1*", "03:01:01:20"),
                new HlaTyping("DQB1*", "03:23:03")
            };

            Assert.IsEmpty(MatchingTypings.Where(m => confidentialAlleles.Contains(m.HlaTyping)));
        }
    }
}
