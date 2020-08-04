using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.GeneticData;
using Atlas.Common.GeneticData.PhenotypeInfo;
using Atlas.Common.GeneticData.PhenotypeInfo.TransferModels;
using Atlas.MatchingAlgorithm.Client.Models.Donors;
using Atlas.MatchingAlgorithm.Client.Models.SearchRequests;

namespace Atlas.MatchingAlgorithm.Test.TestHelpers.Builders
{
    public class MatchingRequestBuilder
    {
        private readonly MatchingRequest matchingRequest;
        private PhenotypeInfo<string> searchHla;

        public MatchingRequestBuilder()
        {
            matchingRequest = new MatchingRequest
            {
                SearchType = DonorType.Adult,
                MatchCriteria = new MismatchCriteria
                {
                    DonorMismatchCount = 0,
                    LocusMismatchCounts = new LociInfo<int?>
                    {
                        A = 0,
                        B = 0,
                        Drb1 = 0,
                        C = null,
                        Dqb1 = null
                    }
                },
                ScoringCriteria = new ScoringCriteria()
                {
                    LociToScore = new List<Locus>(),
                    LociToExcludeFromAggregateScore = new List<Locus>()
                }
            };

            searchHla = new PhenotypeInfo<string>
            {
                A = new LocusInfo<string>("default-hla-a"),
                B = new LocusInfo<string>("default-hla-b"),
                Drb1 = new LocusInfo<string>("default-hla-drb1")
            };
        }

        #region Match Criteria

        public MatchingRequestBuilder WithTotalMismatchCount(int mismatchCount)
        {
            matchingRequest.MatchCriteria.DonorMismatchCount = mismatchCount;
            return this;
        }

        public MatchingRequestBuilder WithLocusMismatchCount(Locus locus, int locusMismatchCount)
        {
            if (locus == Locus.Dpb1)
            {
                throw new NotImplementedException();
            }
            matchingRequest.MatchCriteria.LocusMismatchCounts.SetLocus(locus, locusMismatchCount);
            return this;
        }

        public MatchingRequestBuilder WithMismatchCountAtLoci(IEnumerable<Locus> loci, int locusMismatchCount)
        {
            return loci.Aggregate(this, (current, locus) => current.WithLocusMismatchCount(locus, locusMismatchCount));
        }

        #endregion

        #region Scoring Criteria

        public MatchingRequestBuilder WithLociToScore(IEnumerable<Locus> loci)
        {
            matchingRequest.ScoringCriteria.LociToScore = loci;
            return this;
        }

        public MatchingRequestBuilder WithLociExcludedFromScoringAggregates(IEnumerable<Locus> loci)
        {
            matchingRequest.ScoringCriteria.LociToExcludeFromAggregateScore = loci;
            return this;
        }

        #endregion

        #region Patient Hla

        private MatchingRequestBuilder WithNullLocusSearchHla(Locus locus, LocusPosition position)
        {
            searchHla.SetPosition(locus, position, null);
            return this;
        }

        public MatchingRequestBuilder WithNullLocusSearchHla(Locus locus)
        {
            return WithNullLocusSearchHla(locus, LocusPosition.One)
                .WithNullLocusSearchHla(locus, LocusPosition.Two);
        }

        /// <summary>
        /// Sets the HLA at given locus/position, ignoring any nulls. To explicitly set nulls within a non-empty LocusInfo, use <see cref="WithNullLocusSearchHla"/> 
        /// </summary>
        public MatchingRequestBuilder WithLocusSearchHla(Locus locus, LocusPosition position, string hlaString)
        {
            // API level validation will fail if individual hla are null, but not if the locus is omitted altogether. 
            // If tests need to be added which set individual values to null (i.e. to test that validation), another builder method should be used
            if (hlaString == null)
            {
                return this;
            }

            // If locus is specified, but currently null, initialise that locus. 
            searchHla = new PhenotypeInfo<string>(searchHla.Map((l, hla) =>
            {
                if (l == locus)
                {
                    return hla ?? new LocusInfo<string>();
                }

                return hla;
            }));

            searchHla.SetPosition(locus, position, hlaString);
            return this;
        }

        public MatchingRequestBuilder WithSearchHla(PhenotypeInfo<string> newSearchHla)
        {
            searchHla = newSearchHla;
            return this;
        }

        #endregion

        public MatchingRequestBuilder WithSearchType(DonorType donorType)
        {
            matchingRequest.SearchType = donorType;
            return this;
        }

        public MatchingRequest Build()
        {
            matchingRequest.SearchHlaData = searchHla.ToPhenotypeInfoTransfer();
            return matchingRequest;
        }
    }
}