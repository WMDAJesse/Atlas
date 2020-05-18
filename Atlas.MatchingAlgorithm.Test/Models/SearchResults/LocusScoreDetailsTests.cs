﻿using System;
using Atlas.Common.GeneticData.PhenotypeInfo;
using FluentAssertions;
using Atlas.MatchingAlgorithm.Client.Models.SearchResults;
using Atlas.MatchingAlgorithm.Data.Models;
using Atlas.MatchingAlgorithm.Client.Models.SearchResults.PerLocus;
using Atlas.MatchingAlgorithm.Common.Models;
using Atlas.MatchingAlgorithm.Test.TestHelpers.Builders.SearchResults;
using FluentAssertions;
using NUnit.Framework;

namespace Atlas.MatchingAlgorithm.Test.Models.SearchResults
{
    [TestFixture]
    public class LocusScoreDetailsTests
    {
        [TestCase(LocusPosition.Position1)]
        [TestCase(LocusPosition.Position2)]
        public void MatchGradeScore_WhenMatchGradeScoreIsNull_ThrowsException(LocusPosition typePosition)
        {
            var locusScoreDetails = new LocusScoreDetailsBuilder()
                .WithMatchGradeScoreAtPosition(typePosition, null)
                .Build();

            Assert.Throws<Exception>(() =>
            {
                var matchGradeScore = locusScoreDetails.MatchGradeScore;
            });
        }

        [Test]
        public void MatchGradeScore_WhenMatchGradeScoresAreNotNull_ReturnsSumOfMatchGradeScoresAtPositionOneAndTwo()
        {
            const int scoreAtPositionOne = 1;
            const int scoreAtPositionTwo = 2;
            const int expectedTotalScore = 3;

            var locusScoreDetails = new LocusScoreDetailsBuilder()
                .WithMatchGradeScoreAtPosition(LocusPosition.Position1, scoreAtPositionOne)
                .WithMatchGradeScoreAtPosition(LocusPosition.Position2, scoreAtPositionTwo)
                .Build();

            locusScoreDetails.MatchGradeScore.Should().Be(expectedTotalScore);
        }

        [TestCase(LocusPosition.Position1)]
        [TestCase(LocusPosition.Position2)]
        public void MatchConfidenceScore_WhenMatchConfidenceScoreIsNull_ThrowsException(LocusPosition position)
        {
            var locusScoreDetails = new LocusScoreDetailsBuilder()
                .WithMatchConfidenceScoreAtPosition(position, null)
                .Build();

            Assert.Throws<Exception>(() =>
            {
                var matchConfidenceScore = locusScoreDetails.MatchConfidenceScore;
            });
        }

        [Test]
        public void MatchConfidenceScore_WhenMatchConfidenceScoresAreNotNull_ReturnsSumOfMatchConfidenceScoresAtPositionOneAndTwo()
        {
            const int scoreAtPositionOne = 1;
            const int scoreAtPositionTwo = 2;
            const int expectedTotalScore = 3;

            var locusScoreDetails = new LocusScoreDetailsBuilder()
                .WithMatchConfidenceScoreAtPosition(LocusPosition.Position1, scoreAtPositionOne)
                .WithMatchConfidenceScoreAtPosition(LocusPosition.Position2, scoreAtPositionTwo)
                .Build();

            locusScoreDetails.MatchConfidenceScore.Should().Be(expectedTotalScore);
        }

        [Test]
        public void IsPotentialMatch_WhenMatchConfidencesAreBothPotential_ReturnsTrue()
        {
            var locusScoreDetails = new LocusScoreDetailsBuilder()
                .WithMatchConfidenceAtPosition(LocusPosition.Position1, MatchConfidence.Potential)
                .WithMatchConfidenceAtPosition(LocusPosition.Position2, MatchConfidence.Potential)
                .Build();

            locusScoreDetails.IsPotentialMatch.Should().BeTrue();
        }

        [TestCase(MatchConfidence.Potential, MatchConfidence.Definite)]
        [TestCase(MatchConfidence.Potential, MatchConfidence.Exact)]
        [TestCase(MatchConfidence.Potential, MatchConfidence.Mismatch)]
        [TestCase(MatchConfidence.Definite, MatchConfidence.Potential)]
        [TestCase(MatchConfidence.Definite, MatchConfidence.Definite)]
        [TestCase(MatchConfidence.Definite, MatchConfidence.Exact)]
        [TestCase(MatchConfidence.Definite, MatchConfidence.Mismatch)]
        [TestCase(MatchConfidence.Mismatch, MatchConfidence.Potential)]
        [TestCase(MatchConfidence.Mismatch, MatchConfidence.Definite)]
        [TestCase(MatchConfidence.Mismatch, MatchConfidence.Exact)]
        [TestCase(MatchConfidence.Mismatch, MatchConfidence.Mismatch)]
        [TestCase(MatchConfidence.Exact, MatchConfidence.Potential)]
        [TestCase(MatchConfidence.Exact, MatchConfidence.Definite)]
        [TestCase(MatchConfidence.Exact, MatchConfidence.Exact)]
        [TestCase(MatchConfidence.Exact, MatchConfidence.Mismatch)]
        public void IsPotentialMatch_WhenOneOrNeitherMatchConfidencesArePotential_ReturnsFalse(
            MatchConfidence matchConfidenceAtOne,
            MatchConfidence matchConfidenceAtTwo)
        {
            var locusScoreDetails = new LocusScoreDetailsBuilder()
                .WithMatchConfidenceAtPosition(LocusPosition.Position1, matchConfidenceAtOne)
                .WithMatchConfidenceAtPosition(LocusPosition.Position2, matchConfidenceAtTwo)
                .Build();

            locusScoreDetails.IsPotentialMatch.Should().BeFalse();
        }

        [TestCase(MatchConfidence.Potential, MatchConfidence.Potential)]
        [TestCase(MatchConfidence.Potential, MatchConfidence.Definite)]
        [TestCase(MatchConfidence.Potential, MatchConfidence.Exact)]
        [TestCase(MatchConfidence.Definite, MatchConfidence.Potential)]
        [TestCase(MatchConfidence.Definite, MatchConfidence.Definite)]
        [TestCase(MatchConfidence.Definite, MatchConfidence.Exact)]
        [TestCase(MatchConfidence.Exact, MatchConfidence.Potential)]
        [TestCase(MatchConfidence.Exact, MatchConfidence.Definite)]
        [TestCase(MatchConfidence.Exact, MatchConfidence.Exact)]
        public void MatchCount_WhenNeitherMatchConfidencesIsMismatch_Returns2(
            MatchConfidence matchConfidenceAtOne,
            MatchConfidence matchConfidenceAtTwo)
        {
            var locusScoreDetails = new LocusScoreDetailsBuilder()
                .WithMatchConfidenceAtPosition(LocusPosition.Position1, matchConfidenceAtOne)
                .WithMatchConfidenceAtPosition(LocusPosition.Position2, matchConfidenceAtTwo)
                .Build();

            locusScoreDetails.MatchCount().Should().Be(2);
        }

        [TestCase(MatchConfidence.Definite, MatchConfidence.Mismatch)]
        [TestCase(MatchConfidence.Potential, MatchConfidence.Mismatch)]
        [TestCase(MatchConfidence.Exact, MatchConfidence.Mismatch)]
        [TestCase(MatchConfidence.Mismatch, MatchConfidence.Definite)]
        [TestCase(MatchConfidence.Mismatch, MatchConfidence.Potential)]
        [TestCase(MatchConfidence.Mismatch, MatchConfidence.Exact)]
        public void MatchCount_WhenOneMatchConfidenceIsMismatch_Returns1(
            MatchConfidence matchConfidenceAtOne,
            MatchConfidence matchConfidenceAtTwo)
        {
            var locusScoreDetails = new LocusScoreDetailsBuilder()
                .WithMatchConfidenceAtPosition(LocusPosition.Position1, matchConfidenceAtOne)
                .WithMatchConfidenceAtPosition(LocusPosition.Position2, matchConfidenceAtTwo)
                .Build();

            locusScoreDetails.MatchCount().Should().Be(1);
        }

        [Test]
        public void MatchCount_WhenBothMatchConfidencesAreMismatch_Returns0()
        {
            var locusScoreDetails = new LocusScoreDetailsBuilder()
                .WithMatchConfidenceAtPosition(LocusPosition.Position1, MatchConfidence.Mismatch)
                .WithMatchConfidenceAtPosition(LocusPosition.Position2, MatchConfidence.Mismatch)
                .Build();

            locusScoreDetails.MatchCount().Should().Be(0);
        }
    }
}
