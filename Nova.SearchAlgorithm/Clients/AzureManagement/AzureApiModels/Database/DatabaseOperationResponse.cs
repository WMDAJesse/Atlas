// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nova.SearchAlgorithm.Models.AzureManagement;

namespace Nova.SearchAlgorithm.Clients.AzureManagement.AzureApiModels.Database
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DatabaseOperationResponse
    {
        [JsonProperty("value")]
        public IEnumerable<DatabaseOperationValue> Value { get; set; }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DatabaseOperationValue
    {
        [JsonProperty("properties")]
        public DatabaseOperationProperties Properties { get; set; }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DatabaseOperationProperties
    {
        [JsonProperty("percentComplete")]
        public int PercentComplete { get; set; }
        
        [JsonProperty("operation")]
        public string Operation { get; set; }
        
        [JsonProperty("state")]
        public AzureDatabaseOperationState State { get; set; }
        
        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }
        
        [JsonProperty("databaseName")]
        public string DatabaseName { get; set; }
    }
}