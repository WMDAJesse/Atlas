﻿using Atlas.Common.GeneticData.PhenotypeInfo;
using Atlas.Common.GeneticData.PhenotypeInfo.TransferModels;

namespace Atlas.MatchingAlgorithm.Client.Models.SearchRequests
{
    public class MismatchCriteria
    {
        /// <summary>
        /// Number of mismatches permitted per donor.
        /// Required.
        /// </summary>
        public int DonorMismatchCount { get; set; }

        /// <summary>
        /// Mismatch preferences per locus.
        /// A, B, DRB1 required.
        /// Others optional. Null = any number of mismatches allowed  
        /// </summary>
        public LociInfoTransfer<int?> LocusMismatchCounts { get; set; }
    }
}