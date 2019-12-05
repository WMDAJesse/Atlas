﻿using Nova.SearchAlgorithm.Common.Models;

namespace Nova.SearchAlgorithm.Data.Models
{
    public class DonorMatch
    {
        public int DonorId { get; set; }
        public int TypePosition { get; set; }
        
        public PotentialHlaMatchRelation ToPotentialHlaMatchRelation(TypePosition searchTypePosition, Locus locus)
        {
            return new PotentialHlaMatchRelation()
            {
                Locus = locus,
                Name = "Unknown",
                SearchTypePosition = searchTypePosition,
                MatchingTypePosition = (TypePosition) TypePosition,
                DonorId = DonorId
            };
        }
    }

}