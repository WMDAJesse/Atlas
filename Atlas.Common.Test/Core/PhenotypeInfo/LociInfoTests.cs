﻿using System.Collections.Generic;
using Atlas.Common.GeneticData;
using Atlas.Common.GeneticData.PhenotypeInfo;
using FluentAssertions;
using NUnit.Framework;

namespace Atlas.Common.Test.Core.PhenotypeInfo
{
    [TestFixture]
    public class LociInfoTests
    {
        private readonly IEnumerable<Locus> supportedLoci = EnumStringValues.EnumExtensions.EnumerateValues<Locus>();

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void Map_ReturnsMappedLociInfo()
        {
            static string Mapping(string locusValue) => $"Mapped {locusValue}";

            var initial = new LociInfo<string>();
            foreach (var locus in supportedLoci)
            {
                initial.SetLocus(locus, $"TEST-{locus.ToString()}");
            }

            var mapped = initial.Map(Mapping);

            mapped.A.Should().Be(Mapping(initial.A));
            mapped.B.Should().Be(Mapping(initial.B));
            mapped.C.Should().Be(Mapping(initial.C));
            mapped.Dpb1.Should().Be(Mapping(initial.Dpb1));
            mapped.Dqb1.Should().Be(Mapping(initial.Dqb1));
            mapped.Drb1.Should().Be(Mapping(initial.Drb1));
        }

        [Test]
        public void Map_WhenMapperTakesLocus_CallsMapperForEachLocusAndReturnsMappedLociInfo()
        {
            static string Mapping(Locus locus, string locusValue)
            {
                return $"Mapped {locusValue} at {locus.ToString()}";
            }

            var initial = new LociInfo<string>();
            foreach (var locus in supportedLoci)
            {
                initial.SetLocus(locus, $"TEST-{locus.ToString()}");
            }

            var mapped = initial.Map(Mapping);

            mapped.A.Should().Be(Mapping(Locus.A, initial.A));
            mapped.B.Should().Be(Mapping(Locus.B, initial.B));
            mapped.C.Should().Be(Mapping(Locus.C, initial.C));
            mapped.Dpb1.Should().Be(Mapping(Locus.Dpb1, initial.Dpb1));
            mapped.Dqb1.Should().Be(Mapping(Locus.Dqb1, initial.Dqb1));
            mapped.Drb1.Should().Be(Mapping(Locus.Drb1, initial.Drb1));
        }

        [Test]
        public void Reduce_ReducesAllLoci()
        {
            var data = new LociInfo<int>
            {
                A = 1,
                B = 2,
                C = 3,
                Dpb1 = 4,
                Dqb1 = 5,
                Drb1 = 6
            };

            var reducedData = data.Reduce((locus, value, accumulator) => accumulator + value, 0);

            reducedData.Should().Be(21);
        }
    }
}