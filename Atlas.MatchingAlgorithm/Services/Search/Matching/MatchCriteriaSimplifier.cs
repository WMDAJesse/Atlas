using System.Collections.Generic;
using System.Linq;
using Atlas.Common.GeneticData;
using Atlas.MatchingAlgorithm.Common.Models;

namespace Atlas.MatchingAlgorithm.Services.Search.Matching
{
    internal static class MatchCriteriaSimplifier
    {
        public static List<AlleleLevelMatchCriteria> SplitSearch(AlleleLevelMatchCriteria criteria)
        {
            if (criteria.DonorMismatchCount == 0)
            {
                return new List<AlleleLevelMatchCriteria> { criteria };
            }

            if (criteria.DonorMismatchCount == 1)
            {
                if (criteria.LocusCriteria.Drb1.MismatchCount == 1)
                {
                    return new List<AlleleLevelMatchCriteria>
                    {
                        new AlleleLevelMatchCriteria
                        {
                            LocusCriteria = criteria.LocusCriteria.Map((l, c) => l == Locus.Drb1 ? c?.WithNoMismatches() : c),
                            SearchType = criteria.SearchType,
                            DonorMismatchCount = criteria.DonorMismatchCount,
                            ShouldIncludeBetterMatches = criteria.ShouldIncludeBetterMatches
                        },
                        new AlleleLevelMatchCriteria
                        {
                            LocusCriteria = criteria.LocusCriteria.Map((l, c) => l != Locus.Drb1 ? c?.WithNoMismatches() : c),
                            SearchType = criteria.SearchType,
                            DonorMismatchCount = criteria.DonorMismatchCount,
                            ShouldIncludeBetterMatches = criteria.ShouldIncludeBetterMatches
                        }
                    };
                }
            }

            // Only currently works for 2 allowed MM at all loci
            if (criteria.DonorMismatchCount == 2)
            {
                if (criteria.LocusCriteria.A.MismatchCount == 2 
                    && criteria.LocusCriteria.B.MismatchCount == 2 
                    && criteria.LocusCriteria.C?.MismatchCount == 2 
                    && criteria.LocusCriteria.Drb1.MismatchCount == 2 
                    && criteria.LocusCriteria.Dqb1?.MismatchCount == 2)
                {
                    var oneMismatchSearch = new AlleleLevelMatchCriteria
                    {
                        LocusCriteria = criteria.LocusCriteria.Map((l, c) => c?.WithOneMismatch()),
                        SearchType = criteria.SearchType,
                        DonorMismatchCount = criteria.DonorMismatchCount,
                        ShouldIncludeBetterMatches = criteria.ShouldIncludeBetterMatches
                    };
                    return new List<AlleleLevelMatchCriteria>
                    {
                        new AlleleLevelMatchCriteria
                        {
                            LocusCriteria = criteria.LocusCriteria.Map((l, c) => l == Locus.Drb1 ? c?.WithNoMismatches() : c),
                            SearchType = criteria.SearchType,
                            DonorMismatchCount = criteria.DonorMismatchCount,
                            ShouldIncludeBetterMatches = criteria.ShouldIncludeBetterMatches
                        },
                        new AlleleLevelMatchCriteria
                        {
                            LocusCriteria = criteria.LocusCriteria.Map((l, c) => l != Locus.Drb1 ? c?.WithNoMismatches() : c),
                            SearchType = criteria.SearchType,
                            DonorMismatchCount = criteria.DonorMismatchCount,
                            ShouldIncludeBetterMatches = criteria.ShouldIncludeBetterMatches
                        }
                    }.Concat(SplitSearch(oneMismatchSearch)).ToList();
                }

                if (criteria.LocusCriteria.A.MismatchCount == 1
                    && criteria.LocusCriteria.B.MismatchCount == 1
                    && criteria.LocusCriteria.C?.MismatchCount == 1
                    && criteria.LocusCriteria.Drb1.MismatchCount == 1 
                    && criteria.LocusCriteria.Dqb1?.MismatchCount == 1)
                {
                    return new List<AlleleLevelMatchCriteria>
                    {
                        new AlleleLevelMatchCriteria
                        {
                            LocusCriteria = criteria.LocusCriteria.Map((l, c) => l == Locus.Drb1 ? c?.WithNoMismatches() : c),
                            SearchType = criteria.SearchType,
                            DonorMismatchCount = criteria.DonorMismatchCount,
                            ShouldIncludeBetterMatches = criteria.ShouldIncludeBetterMatches
                        },
                        new AlleleLevelMatchCriteria
                        {
                            LocusCriteria = criteria.LocusCriteria.Map((l, c) => l == Locus.B ? c?.WithNoMismatches() : c),
                            SearchType = criteria.SearchType,
                            DonorMismatchCount = criteria.DonorMismatchCount,
                            ShouldIncludeBetterMatches = criteria.ShouldIncludeBetterMatches
                        },
                        new AlleleLevelMatchCriteria
                        {
                            LocusCriteria = criteria.LocusCriteria.Map((l, c) => l == Locus.A ? c?.WithNoMismatches() : c),
                            SearchType = criteria.SearchType,
                            DonorMismatchCount = criteria.DonorMismatchCount,
                            ShouldIncludeBetterMatches = criteria.ShouldIncludeBetterMatches
                        },
                    };
                }
            }
            
            return new List<AlleleLevelMatchCriteria> { criteria };
        }

        public static AlleleLevelLocusMatchCriteria WithNoMismatches(this AlleleLevelLocusMatchCriteria criteria) => criteria?.WithXMismatches(0);
        public static AlleleLevelLocusMatchCriteria WithOneMismatch(this AlleleLevelLocusMatchCriteria criteria) => criteria?.WithXMismatches(1);

        
        public static AlleleLevelLocusMatchCriteria WithXMismatches(this AlleleLevelLocusMatchCriteria criteria, int x)
        {
            if (criteria == null)
            {
                return null;
            }

            return new AlleleLevelLocusMatchCriteria
            {
                MismatchCount = x,
                PGroupsToMatchInPositionOne = criteria.PGroupsToMatchInPositionOne,
                PGroupsToMatchInPositionTwo = criteria.PGroupsToMatchInPositionTwo
            };
        }
    }
}