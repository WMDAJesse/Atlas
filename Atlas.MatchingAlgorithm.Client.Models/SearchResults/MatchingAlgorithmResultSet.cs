﻿using System.Collections.Generic;

namespace Atlas.MatchingAlgorithm.Client.Models.SearchResults
{
    public class MatchingAlgorithmResultSet
    {
        public string SearchRequestId { get; set; }
        public int ResultCount { get; set; }
        public IEnumerable<MatchingAlgorithmResult> MatchingAlgorithmResults { get; set; }

        public string HlaNomenclatureVersion { get; set; }
        public string BlobStorageContainerName { get; set; }
        public string ResultsFileName => $"{SearchRequestId}.json";
    }
}