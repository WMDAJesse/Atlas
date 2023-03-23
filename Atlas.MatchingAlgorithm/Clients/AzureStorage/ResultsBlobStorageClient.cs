using Atlas.Common.AzureStorage.Blob;
using Atlas.MatchingAlgorithm.Settings.Azure;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Atlas.Client.Models.Search.Results.Matching;
using Atlas.Client.Models.Search.Results.ResultSet;
using Atlas.MatchingAlgorithm.ApplicationInsights.ContextAwareLogging;

namespace Atlas.MatchingAlgorithm.Clients.AzureStorage
{
    public interface IResultsBlobStorageClient
    {
        Task UploadResults(ResultSet<MatchingAlgorithmResult> searchResultSet);
        string GetResultsContainerName();
    }

    public class ResultsBlobStorageClient : BlobUploader, IResultsBlobStorageClient
    {
        private readonly AzureStorageSettings azureStorageSettings;

        // ReSharper disable once SuggestBaseTypeForParameter
        public ResultsBlobStorageClient(AzureStorageSettings azureStorageSettings, IMatchingAlgorithmSearchLogger logger)
            : base(azureStorageSettings.ConnectionString, logger)
        {
            this.azureStorageSettings = azureStorageSettings;
        }
        public async Task UploadResults(ResultSet<MatchingAlgorithmResult> searchResultSet)
        {
            searchResultSet.BatchedResult = azureStorageSettings.ShouldBatchResults;  // Results will not be serialised if results are being batched
            var serialisedResults = JsonConvert.SerializeObject(searchResultSet);
            await Upload(azureStorageSettings.SearchResultsBlobContainer, searchResultSet.ResultsFileName, serialisedResults);

            if (azureStorageSettings.ShouldBatchResults)
            {
                await BatchUpload(searchResultSet.Results, azureStorageSettings.SearchResultsBatchSize, azureStorageSettings.SearchResultsBlobContainer, searchResultSet.SearchRequestId);
            }
        }

        public string GetResultsContainerName()
        {
            return azureStorageSettings.SearchResultsBlobContainer;
        }
    }
}