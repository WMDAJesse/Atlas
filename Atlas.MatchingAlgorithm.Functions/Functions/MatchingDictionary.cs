using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Atlas.MatchingAlgorithm.Services.ConfigurationProviders;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Atlas.MatchingAlgorithm.Services.MatchingDictionary;
using Atlas.Utils.CodeAnalysis;

namespace Atlas.MatchingAlgorithm.Functions.Functions
{
    public class MatchingDictionary
    {
        private readonly IHlaMetadataDictionary hlaMetadataDictionary;

        public MatchingDictionary(
            IHlaMetadataDictionaryFactory factory,
            IActiveHlaVersionAccessor hlaVersionAccessor)
        {
            this.hlaMetadataDictionary = factory.BuildDictionary(hlaVersionAccessor.GetActiveHlaDatabaseVersion());
        }

        [SuppressMessage(null, SuppressMessage.UnusedParameter, Justification = SuppressMessage.UsedByAzureTrigger)]
        [FunctionName("RefreshHlaMetadataDictionary")]
        public async Task Refresh([HttpTrigger] HttpRequest httpRequest)
        {
            await hlaMetadataDictionary.RecreateHlaMetadataDictionary(Services.MatchingDictionary.HlaMetadataDictionary.CreationBehaviour.Latest);
        }
    }
}