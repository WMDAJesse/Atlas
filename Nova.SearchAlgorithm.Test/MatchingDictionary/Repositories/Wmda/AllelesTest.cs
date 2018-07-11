﻿using Nova.SearchAlgorithm.MatchingDictionary.Models.Wmda;
using NUnit.Framework;
using System.Collections.Generic;
using Nova.SearchAlgorithm.MatchingDictionary.Models.HLATypings;

namespace Nova.SearchAlgorithm.Test.MatchingDictionary.Repositories.Wmda
{
    [TestFixtureSource(typeof(WmdaRepositoryTestFixtureArgs), nameof(WmdaRepositoryTestFixtureArgs.HlaNomAllelesTestArgs))]
    public class AllelesTest : WmdaRepositoryTestBase<HlaNom>
    {
        public AllelesTest(IEnumerable<HlaNom> hlaNomAlleles, IEnumerable<string> matchLoci)
            : base(hlaNomAlleles, matchLoci)
        {
        }

        [TestCase("A*", "01:26")]
        [TestCase("A*", "01:27N")]
        [TestCase("B*", "07:05:06")]
        [TestCase("C*", "07:491:01N")]
        [TestCase("DQB1*", "03:01:01:07")]
        [TestCase("C*", "07:01:01:14Q")]
        public void WmdaDataRepository_WhenValidAllele_SuccessfullyCaptured(string locus, string alleleName)
        {
            var expectedAllele = new HlaNom(TypingMethod.Molecular, locus, alleleName);

            var actualAllele = GetSingleWmdaHlaTyping(locus, alleleName);
            
            Assert.AreEqual(expectedAllele, actualAllele);
        }

        [TestCase("C*", "07:295", true)]
        public void WmdaDataRepository_WhenDeletedAlleleNoIdenticalHla_SuccessfullyCaptured(string locus, string alleleName, bool isDeleted)
        {
            var expectedAllele = new HlaNom(TypingMethod.Molecular, locus, alleleName, isDeleted);

            var actualAllele = GetSingleWmdaHlaTyping(locus, alleleName);
            
            Assert.AreEqual(expectedAllele, actualAllele);
        }

        [TestCase("DRB1*", "08:01:03", true, "08:01:01")]
        public void WmdaDataRepository_WhenDeletedAlleleWithIdenticalHla_SuccessfullyCaptured(string locus, string alleleName, bool isDeleted, string identicalHla)
        {
            var expectedAllele = new HlaNom(TypingMethod.Molecular, locus, alleleName, isDeleted, identicalHla);

            var actualAllele = GetSingleWmdaHlaTyping(locus, alleleName);
            
            Assert.AreEqual(expectedAllele, actualAllele);
        }
    }
}
