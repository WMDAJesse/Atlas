﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nova.SearchAlgorithm.Data.Entity
{
    public class ScoringWeighting
    {
        public int Id { get; set; }
        
        /// <summary>
        /// The name corresponding to an enum value (e.g. Grade, Confidence) in the codebase
        /// </summary>
        [Index(IsUnique = true)]
        [StringLength(100)]
        public string Name { get; set; }
        
        /// <summary>
        /// An integer weight used for relative ordering of grades/confidences in results
        /// </summary>
        public int Weight { get; set; }
    }
}