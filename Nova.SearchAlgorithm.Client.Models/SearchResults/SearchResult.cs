﻿using System;

namespace Nova.SearchAlgorithm.Client.Models.SearchResults
{
    public class SearchResult
    {
        /// <summary>
        /// The ID of the donor for lookup in donor registries.
        /// </summary>
        public int DonorId { get; set; }
        
        /// <summary>
        /// The type of donor, for example Adult or Cord.
        /// </summary>
        public DonorType DonorType { get; set; }

        /// <summary>
        /// The code of the donor registry which this donor originates from.
        /// </summary>
        public RegistryCode Registry { get; set; }

        /// <summary>
        /// The number of loci matched, down to the type.
        /// Out of a maximum of 10.
        /// Should some loci be untyped, then this field reflects the potential match count,
        /// rather than the actual known match count.
        /// </summary>
        public int TotalMatchCount { get; set; }

        /// <summary>
        /// The number of the total potential matches.
        /// The TotalMatchCount is a sum of potential and exact matches, so an exact match count can be calculated as the difference of these values
        /// </summary>
        public int PotentialMatchCount { get; set; }

        /// <summary>
        /// The number of the total exact matches.
        /// The TotalMatchCount is a sum of potential and exact matches, so an exact match count can be calculated as the difference of these values
        /// </summary>
        public int ExactMatchCount => TotalMatchCount - PotentialMatchCount;

        /// <summary>
        /// The number of loci which are typed for this donor.
        /// Loci excluded from scoring aggregation will not be included, regardless of whether they are typed.
        /// </summary>
        public int TypedLociCount { get; set; }

        /// <summary>
        /// A numeric value representing the aggregate relative match grade across all loci, according to the scoring algorithm
        /// </summary>
        public int GradeScore { get; set; }

        /// <summary>
        /// A numeric value representing the aggregate relative match confidence across all loci, according to the scoring algorithm
        /// </summary>
        public int ConfidenceScore { get; set; }

        /// <summary>
        /// The overall confidence for the match.
        /// Calculated as the worst across all per-position match confidences.
        /// </summary>
        [Obsolete("Use MatchCategory instead.")]
        public MatchConfidence OverallMatchConfidence { get; set; }
        
        /// <summary>
        /// The overall quality of the match. An aggregate of the per-locus grades and confidences. 
        /// </summary>
        public MatchCategory MatchCategory { get; set; }
        
        /// <summary>
        /// The details of the match at locus A.
        /// </summary>
        public LocusSearchResult SearchResultAtLocusA { get; set; }

        /// <summary>
        /// The details of the match at locus B.
        /// </summary>
        public LocusSearchResult SearchResultAtLocusB { get; set; }

        /// <summary>
        /// The details of the match at locus C.
        /// </summary>
        public LocusSearchResult SearchResultAtLocusC { get; set; }

        /// <summary>
        /// The details of the match at locus DPB1.
        /// </summary>
        public LocusSearchResult SearchResultAtLocusDpb1 { get; set; }

        /// <summary>
        /// The details of the match at locus DQB1.
        /// </summary>
        public LocusSearchResult SearchResultAtLocusDqb1 { get; set; }

        /// <summary>
        /// The details of the match at locus DRB1.
        /// </summary>
        public LocusSearchResult SearchResultAtLocusDrb1 { get; set; }
    }
}