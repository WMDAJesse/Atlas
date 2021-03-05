using System.Threading.Tasks;
using Atlas.Client.Models.Search.Results.Matching;
using Atlas.Client.Models.Search.Results.Matching.ResultSet;
using Atlas.Common.ApplicationInsights;
using Atlas.Common.ApplicationInsights.Timing;
using Atlas.Common.AzureStorage.Blob;
using Atlas.Functions.Settings;
using Microsoft.Extensions.Options;

namespace Atlas.Functions.Services.BlobStorageClients
{
    public interface IMatchingResultsDownloader
    {
        public Task<MatchingAlgorithmResultSet> Download(string blobName, bool isRepeatSearch);
    }

    internal class MatchingResultsDownloader : IMatchingResultsDownloader
    {
        private readonly AzureStorageSettings azureStorageSettings;
        private readonly IBlobDownloader blobDownloader;
        private readonly ILogger logger;

        public MatchingResultsDownloader(IOptions<AzureStorageSettings> azureStorageSettings, IBlobDownloader blobDownloader, ILogger logger)
        {
            this.azureStorageSettings = azureStorageSettings.Value;
            this.blobDownloader = blobDownloader;
            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task<MatchingAlgorithmResultSet> Download(string blobName, bool isRepeatSearch)
        {
            using (logger.RunTimed($"Downloading matching results: {blobName}"))
            {
                var matchingResultsBlobContainer = isRepeatSearch
                    ? azureStorageSettings.RepeatSearchMatchingResultsBlobContainer
                    : azureStorageSettings.MatchingResultsBlobContainer;
                return await blobDownloader.Download<MatchingAlgorithmResultSet>(matchingResultsBlobContainer, blobName);
            }
        }
    }
}