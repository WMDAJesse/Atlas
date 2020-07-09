using System.Collections.Generic;
using System.Linq;
using Atlas.Common.GeneticData;
using Atlas.Common.GeneticData.PhenotypeInfo;

namespace Atlas.MatchPrediction.Test.TestHelpers.Builders
{
    internal class MatchCountsBuilder
    {
        private readonly LociInfo<int?> matchCounts;

        public MatchCountsBuilder()
        {
            matchCounts = new LociInfo<int?>();
        }

        public MatchCountsBuilder TenOutOfTen()
        {
            var loci = EnumStringValues.EnumExtensions.EnumerateValues<Locus>().Except(new List<Locus> {Locus.Dpb1});

            foreach (var locus in loci)
            {
                matchCounts.SetLocus(locus, 2);
            }

            return this;
        }

        private MatchCountsBuilder WithMatchCountAt(Locus locus, int mismatchCount)
        {
            matchCounts.SetLocus(locus, mismatchCount);
            return this;
        }

        private MatchCountsBuilder WithMatchCountAt(int mismatchCount, params Locus[] loci)
        {
            foreach (var locus in loci)
            {
                WithMatchCountAt(locus, mismatchCount);
            }
            return this;
        }

        public MatchCountsBuilder WithDoubleMismatchAt(params Locus[] loci)
        {
            return WithMatchCountAt(0, loci);
        }
        
        public MatchCountsBuilder WithSingleMismatchAt(params Locus[] loci)
        {
            return WithMatchCountAt(1, loci);
        }
        
        public MatchCountsBuilder WithNoMismatchAt(params Locus[] loci)
        {
            return WithMatchCountAt(2, loci);
        }

        public LociInfo<int?> Build()
        {
            return matchCounts;
        }
    }
}