﻿using Atlas.Common.GeneticData;
using System.Collections.Generic;

namespace Atlas.MatchingAlgorithm.Client.Models.SearchRequests
{
    public class ScoringCriteria
    {
        /// <summary>
        /// By default, scoring is not performed on matched donor HLA, except on the loci specified here.
        /// </summary>
        public IEnumerable<Locus> LociToScore { get; set; }

        /// <summary>
        /// By default, the algorithm will use scoring information available at loci defined in <see cref="LociToScore"/>
        /// to aggregate into some overall values to use for ranking. e.g. MatchCategory, GradeScore, ConfidenceScore
        /// Any loci specified here can be excluded from these aggregates.
        /// </summary>
        public IEnumerable<Locus> LociToExcludeFromAggregateScore { get; set; }
    }
}