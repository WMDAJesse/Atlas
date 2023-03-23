﻿namespace Atlas.RepeatSearch.Settings.Azure
{
    public class AzureStorageSettings
    {
        public string ConnectionString { get; set; }
        public string MatchingResultsBlobContainer { get; set; }
        public int SearchResultsBatchSize { get; set; }

        public bool ShouldBatchResults => SearchResultsBatchSize > 0;
    }
}
