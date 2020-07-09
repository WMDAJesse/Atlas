﻿using System.Collections.Generic;
using Atlas.Common.GeneticData;
using Atlas.Common.GeneticData.PhenotypeInfo;
using Atlas.MatchPrediction.Models;
using Atlas.MatchPrediction.Services.MatchProbability;
using Atlas.MatchPrediction.Test.TestHelpers.Builders;
using FluentAssertions;
using NUnit.Framework;
using static Atlas.Common.Test.SharedTestHelpers.Builders.DictionaryBuilder;

namespace Atlas.MatchPrediction.Test.Services.MatchProbability
{
    [TestFixture]
    public class MatchProbabilityCalculatorTests
    {
        private IMatchProbabilityCalculator matchProbabilityCalculator;

        private readonly PhenotypeInfo<string> defaultDonorHla1 = new PhenotypeInfo<string>("donor-hla-1");
        private readonly PhenotypeInfo<string> defaultDonorHla2 = new PhenotypeInfo<string>("donor-hla-2");
        private readonly PhenotypeInfo<string> defaultPatientHla1 = new PhenotypeInfo<string>("patient-hla-1");
        private readonly PhenotypeInfo<string> defaultPatientHla2 = new PhenotypeInfo<string>("patient-hla-2");

        [SetUp]
        public void Setup()
        {
            matchProbabilityCalculator = new MatchProbabilityCalculator();
        }

        [Test]
        public void CalculateMatchProbability_ReturnsMatchProbability()
        {
            var matchingPairs = new HashSet<GenotypeMatchDetails>
            {
                GenotypeMatchDetailsBuilder.New
                    .WithGenotypes(defaultDonorHla1, defaultPatientHla1)
                    .WithMatchCounts(new MatchCountsBuilder().TenOutOfTen().Build())
                    .Build(),
                GenotypeMatchDetailsBuilder.New
                    .WithGenotypes(defaultDonorHla2, defaultPatientHla2)
                    .WithMatchCounts(new MatchCountsBuilder().TenOutOfTen().WithDoubleMismatchAt(Locus.Dqb1, Locus.Drb1).Build())
                    .Build(),
            };

            var likelihoods = DictionaryWithCommonValue(0.5m, defaultDonorHla1, defaultDonorHla2, defaultPatientHla1, defaultPatientHla2);

            var actualProbability = matchProbabilityCalculator.CalculateMatchProbability(
                new HashSet<PhenotypeInfo<string>> {defaultPatientHla1, defaultPatientHla2},
                new HashSet<PhenotypeInfo<string>> {defaultDonorHla1, defaultDonorHla2},
                matchingPairs,
                likelihoods
            );

            var expectedMatchProbabilityPerLocus = new LociInfo<decimal?> {A = 0.5M, B = 0.5M, C = 0.5M, Dpb1 = null, Dqb1 = 0.25M, Drb1 = 0.25M};
            actualProbability.ZeroMismatchProbability.Should().Be(0.25m);
            actualProbability.ZeroMismatchProbabilityPerLocus.Should().Be(expectedMatchProbabilityPerLocus);
        }

        [Test]
        public void CalculateMatchProbability_WhenLocusWithOneMismatch_ReturnsMatchProbability()
        {
            var matchingPairs = new HashSet<GenotypeMatchDetails>
            {
                GenotypeMatchDetailsBuilder.New
                    .WithGenotypes(defaultDonorHla1, defaultPatientHla1)
                    .WithMatchCounts(new MatchCountsBuilder().TenOutOfTen().Build())
                    .Build(),
                GenotypeMatchDetailsBuilder.New
                    .WithGenotypes(defaultDonorHla2, defaultPatientHla2)
                    .WithMatchCounts(new MatchCountsBuilder().TenOutOfTen().WithSingleMismatchAt(Locus.Drb1).Build())
                    .Build(),
            };

            var likelihoods = DictionaryWithCommonValue(0.5m, defaultDonorHla1, defaultDonorHla2, defaultPatientHla1, defaultPatientHla2);

            var actualProbability = matchProbabilityCalculator.CalculateMatchProbability(
                new HashSet<PhenotypeInfo<string>> {defaultPatientHla1, defaultPatientHla2},
                new HashSet<PhenotypeInfo<string>> {defaultDonorHla1, defaultDonorHla2},
                matchingPairs,
                likelihoods
            );

            var expectedMatchProbabilityPerLocus = new LociInfo<decimal?> {A = 0.5M, B = 0.5M, C = 0.5M, Dpb1 = null, Dqb1 = 0.5M, Drb1 = 0.25M};
            actualProbability.OneMismatchProbability.Should().Be(0.25m);
            actualProbability.ZeroMismatchProbabilityPerLocus.Should().Be(expectedMatchProbabilityPerLocus);
        }

        [Test]
        public void CalculateMatchProbability_WhenLocusWithTwoMismatchesAtSameLocus_ReturnsMatchProbability()
        {
            var matchingPairs = new HashSet<GenotypeMatchDetails>
            {
                GenotypeMatchDetailsBuilder.New
                    .WithGenotypes(defaultDonorHla1, defaultPatientHla1)
                    .WithMatchCounts(new MatchCountsBuilder().TenOutOfTen().Build())
                    .Build(),
                GenotypeMatchDetailsBuilder.New
                    .WithGenotypes(defaultDonorHla2, defaultPatientHla2)
                    .WithMatchCounts(new MatchCountsBuilder().TenOutOfTen().WithDoubleMismatchAt(Locus.Drb1).Build())
                    .Build(),
            };

            var likelihoods = DictionaryWithCommonValue(0.5m, defaultDonorHla1, defaultDonorHla2, defaultPatientHla1, defaultPatientHla2);

            var actualProbability = matchProbabilityCalculator.CalculateMatchProbability(
                new HashSet<PhenotypeInfo<string>> {defaultPatientHla1, defaultPatientHla2},
                new HashSet<PhenotypeInfo<string>> {defaultDonorHla1, defaultDonorHla2},
                matchingPairs,
                likelihoods
            );

            var expectedMatchProbabilityPerLocus = new LociInfo<decimal?> {A = 0.5M, B = 0.5M, C = 0.5M, Dpb1 = null, Dqb1 = 0.5M, Drb1 = 0.25M};
            actualProbability.TwoMismatchProbability.Should().Be(0.25m);
            actualProbability.ZeroMismatchProbabilityPerLocus.Should().Be(expectedMatchProbabilityPerLocus);
        }

        [Test]
        public void CalculateMatchProbability_WhenLocusWithTwoMismatchesAtDifferentLoci_ReturnsMatchProbability()
        {
            var matchingPairs = new HashSet<GenotypeMatchDetails>
            {
                GenotypeMatchDetailsBuilder.New
                    .WithGenotypes(defaultDonorHla1, defaultPatientHla1)
                    .WithMatchCounts(new MatchCountsBuilder().TenOutOfTen().Build())
                    .Build(),
                GenotypeMatchDetailsBuilder.New
                    .WithGenotypes(defaultDonorHla2, defaultPatientHla2)
                    .WithMatchCounts(new MatchCountsBuilder().TenOutOfTen().WithSingleMismatchAt(Locus.B, Locus.C).Build())
                    .Build(),
            };

            var likelihoods = DictionaryWithCommonValue(0.5m, defaultDonorHla1, defaultDonorHla2, defaultPatientHla1, defaultPatientHla2);

            var actualProbability = matchProbabilityCalculator.CalculateMatchProbability(
                new HashSet<PhenotypeInfo<string>> {defaultPatientHla1, defaultPatientHla2},
                new HashSet<PhenotypeInfo<string>> {defaultDonorHla1, defaultDonorHla2},
                matchingPairs,
                likelihoods
            );

            var expectedMatchProbabilityPerLocus = new LociInfo<decimal?> {A = 0.5M, B = 0.25M, C = 0.25M, Dpb1 = null, Dqb1 = 0.5M, Drb1 = 0.5M};
            actualProbability.TwoMismatchProbability.Should().Be(0.25m);
            actualProbability.ZeroMismatchProbabilityPerLocus.Should().Be(expectedMatchProbabilityPerLocus);
        }

        [Test]
        public void CalculateMatchProbability_WithUnrepresentedPhenotypes_HasZeroProbability()
        {
            var likelihoods = DictionaryWithCommonValue(0m, defaultDonorHla1, defaultDonorHla2, defaultPatientHla1, defaultPatientHla2);

            var actualProbability = matchProbabilityCalculator.CalculateMatchProbability(
                new HashSet<PhenotypeInfo<string>> {defaultPatientHla1, defaultPatientHla2},
                new HashSet<PhenotypeInfo<string>> {defaultDonorHla1, defaultDonorHla2},
                new HashSet<GenotypeMatchDetails>(),
                likelihoods
            );

            actualProbability.ZeroMismatchProbability.Should().Be(0m);
            actualProbability.OneMismatchProbability.Should().Be(0m);
            actualProbability.TwoMismatchProbability.Should().Be(0m);
        }
    }
}