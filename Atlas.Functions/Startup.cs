using Atlas.Common.ApplicationInsights;
using Atlas.Common.AzureStorage.Blob;
using Atlas.Common.Notifications;
using Atlas.DonorImport.ExternalInterface.DependencyInjection;
using Atlas.Functions;
using Atlas.Functions.Services;
using Atlas.HlaMetadataDictionary.ExternalInterface.Settings;
using Atlas.MatchingAlgorithm.Settings.Azure;
using Atlas.MatchingAlgorithm.Settings.ServiceBus;
using Atlas.MatchPrediction.ExternalInterface.DependencyInjection;
using Atlas.MultipleAlleleCodeDictionary.ExternalInterface.DependencyInjection;
using Atlas.MultipleAlleleCodeDictionary.Settings;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Atlas.Common.Utils.Extensions.DependencyInjectionUtils;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Atlas.Functions
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            RegisterSettings(builder.Services);
            RegisterTopLevelFunctionServices(builder.Services);

            builder.Services.RegisterDonorReader(ConnectionStringReader("DonorImport:Sql"));

            builder.Services.RegisterMacDictionary(
                OptionsReaderFor<ApplicationInsightsSettings>(),
                OptionsReaderFor<MacDictionarySettings>()
            );

            builder.Services.RegisterMacImport(
                OptionsReaderFor<ApplicationInsightsSettings>(),
                OptionsReaderFor<MacDictionarySettings>(),
                OptionsReaderFor<MacDownloadSettings>()
            );

            builder.Services.RegisterMatchPredictionAlgorithm(
                OptionsReaderFor<ApplicationInsightsSettings>(),
                OptionsReaderFor<HlaMetadataDictionarySettings>(),
                OptionsReaderFor<MacDictionarySettings>(),
                OptionsReaderFor<NotificationsServiceBusSettings>(),
                OptionsReaderFor<MatchPrediction.ExternalInterface.Settings.AzureStorageSettings>(),
                ConnectionStringReader("MatchPrediction:Sql")
            );
        }

        private static void RegisterSettings(IServiceCollection services)
        {
            // Atlas Function settings
            services.RegisterAsOptions<Settings.AzureStorageSettings>("AtlasFunction:AzureStorage");
            services.RegisterAsOptions<Settings.MessagingServiceBusSettings>("AtlasFunction:MessagingServiceBus");
            services.RegisterAsOptions<Settings.OrchestrationSettings>("AtlasFunction:Orchestration");

            // Shared settings
            services.RegisterAsOptions<ApplicationInsightsSettings>("ApplicationInsights");
            services.RegisterAsOptions<NotificationsServiceBusSettings>("NotificationsServiceBus");

            // Dictionary components
            services.RegisterAsOptions<HlaMetadataDictionarySettings>("HlaMetadataDictionary");
            services.RegisterAsOptions<MacDictionarySettings>("MacDictionary");
            services.RegisterAsOptions<MacDownloadSettings>("MacDictionary:Download");

            // Matching Algorithm
            services.RegisterAsOptions<AzureStorageSettings>("Matching:AzureStorage");
            services.RegisterAsOptions<MessagingServiceBusSettings>("Matching:MessagingServiceBus");

            // Match Prediction Algorithm
            services.RegisterAsOptions<MatchPrediction.ExternalInterface.Settings.AzureStorageSettings>("MatchPrediction:AzureStorage");
        }

        private static void RegisterTopLevelFunctionServices(IServiceCollection services)
        {
            services.RegisterAtlasLogger(OptionsReaderFor<ApplicationInsightsSettings>());
            services.AddScoped<IMatchPredictionInputBuilder, MatchPredictionInputBuilder>();
            services.AddScoped<IResultsCombiner, ResultsCombiner>();
            services.AddScoped<IResultsUploader, ResultsUploader>();
            services.AddScoped<ISearchCompletionMessageSender, SearchCompletionMessageSender>();
            services.AddScoped<IMatchingResultsDownloader, MatchingResultsDownloader>();
            services.AddScoped<IBlobDownloader, BlobDownloader>(sp =>
            {
                var logger = sp.GetService<ILogger>();
                var connectionString = sp.GetService<IOptions<Settings.AzureStorageSettings>>().Value.ConnectionString;
                return new BlobDownloader(connectionString, logger);
            });
        }
    }
}