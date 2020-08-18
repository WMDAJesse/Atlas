using System.Threading.Tasks;
using Atlas.Common.ApplicationInsights;
using Atlas.Common.AzureStorage.Blob;
using Atlas.Functions.PublicApi.Models.Search.Results;
using Atlas.Functions.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Atlas.Functions.Services
{
    public interface IResultsUploader
    {
        Task UploadResults(SearchResultSet searchResultSet);
    }

    internal class ResultsUploader : BlobUploader, IResultsUploader
    {
        private readonly string resultsContainer;

        /// <inheritdoc />
        public ResultsUploader(IOptions<AzureStorageSettings> azureStorageSettings, ILogger logger)
            : base(azureStorageSettings.Value.ConnectionString, logger)
        {
            resultsContainer = azureStorageSettings.Value.SearchResultsBlobContainer;
        }

        /// <inheritdoc />
        public async Task UploadResults(SearchResultSet searchResultSet)
        {
            var serialisedResults = JsonConvert.SerializeObject(searchResultSet);
            await Upload(resultsContainer, searchResultSet.ResultsFileName, serialisedResults);
        }
    }
}