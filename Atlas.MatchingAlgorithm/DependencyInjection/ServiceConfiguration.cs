using System;
using Atlas.Common.ApplicationInsights;
using Atlas.Common.Caching;
using Atlas.Common.GeneticData.Hla.Services;
using Atlas.Common.Matching.Services;
using Atlas.Common.Notifications;
using Atlas.Common.ServiceBus.BatchReceiving;
using Atlas.DonorImport.ExternalInterface.DependencyInjection;
using Atlas.HlaMetadataDictionary.ExternalInterface.DependencyInjection;
using Atlas.HlaMetadataDictionary.ExternalInterface.Settings;
using Atlas.MatchingAlgorithm.ApplicationInsights.SearchRequests;
using Atlas.MatchingAlgorithm.Client.Models.Donors;
using Atlas.MatchingAlgorithm.Clients.AzureManagement;
using Atlas.MatchingAlgorithm.Clients.AzureStorage;
using Atlas.MatchingAlgorithm.Clients.ServiceBus;
using Atlas.MatchingAlgorithm.Config;
using Atlas.MatchingAlgorithm.Data.Persistent.Context;
using Atlas.MatchingAlgorithm.Data.Persistent.Repositories;
using Atlas.MatchingAlgorithm.Services.AzureManagement;
using Atlas.MatchingAlgorithm.Services.ConfigurationProviders;
using Atlas.MatchingAlgorithm.Services.ConfigurationProviders.TransientSqlDatabase;
using Atlas.MatchingAlgorithm.Services.ConfigurationProviders.TransientSqlDatabase.ConnectionStringProviders;
using Atlas.MatchingAlgorithm.Services.ConfigurationProviders.TransientSqlDatabase.RepositoryFactories;
using Atlas.MatchingAlgorithm.Services.DataRefresh;
using Atlas.MatchingAlgorithm.Services.DataRefresh.DonorImport;
using Atlas.MatchingAlgorithm.Services.DataRefresh.HlaProcessing;
using Atlas.MatchingAlgorithm.Services.DonorManagement;
using Atlas.MatchingAlgorithm.Services.Donors;
using Atlas.MatchingAlgorithm.Services.Search;
using Atlas.MatchingAlgorithm.Services.Search.Matching;
using Atlas.MatchingAlgorithm.Services.Search.Scoring;
using Atlas.MatchingAlgorithm.Services.Search.Scoring.Aggregation;
using Atlas.MatchingAlgorithm.Services.Search.Scoring.Confidence;
using Atlas.MatchingAlgorithm.Services.Search.Scoring.Grading;
using Atlas.MatchingAlgorithm.Services.Search.Scoring.Grading.GradingCalculators;
using Atlas.MatchingAlgorithm.Services.Search.Scoring.Ranking;
using Atlas.MatchingAlgorithm.Services.Utility;
using Atlas.MatchingAlgorithm.Settings;
using Atlas.MatchingAlgorithm.Settings.Azure;
using Atlas.MatchingAlgorithm.Settings.ServiceBus;
using Atlas.MultipleAlleleCodeDictionary.Settings;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Atlas.Common.Utils.Extensions.DependencyInjectionUtils;

namespace Atlas.MatchingAlgorithm.DependencyInjection
{
    public static class ServiceConfiguration
    {
        public static void RegisterMatchingAlgorithm(
            this IServiceCollection services,
            Func<IServiceProvider, ApplicationInsightsSettings> fetchApplicationInsightsSettings,
            Func<IServiceProvider, AzureAuthenticationSettings> fetchAzureAuthenticationSettings,
            Func<IServiceProvider, AzureAppServiceManagementSettings> fetchAzureAppServiceManagementSettings,
            Func<IServiceProvider, AzureDatabaseManagementSettings> fetchAzureDatabaseManagementSettings,
            Func<IServiceProvider, AzureStorageSettings> fetchAzureStorageSettings,
            Func<IServiceProvider, DataRefreshSettings> fetchDataRefreshSettings,
            Func<IServiceProvider, HlaMetadataDictionarySettings> fetchHlaMetadataDictionarySettings,
            Func<IServiceProvider, MacDictionarySettings> fetchMacDictionarySettings,
            Func<IServiceProvider, MessagingServiceBusSettings> fetchMessagingServiceBusSettings,
            Func<IServiceProvider, NotificationsServiceBusSettings> fetchNotificationsServiceBusSettings)
        {
            services.RegisterSettingsForMatchingAlgorithm(
                fetchApplicationInsightsSettings,
                fetchAzureAuthenticationSettings,
                fetchAzureAppServiceManagementSettings,
                fetchAzureDatabaseManagementSettings,
                fetchAzureStorageSettings,
                fetchDataRefreshSettings,
                fetchMessagingServiceBusSettings,
                fetchNotificationsServiceBusSettings
            );
            services.RegisterMatchingAlgorithmServices();
            services.RegisterDataServices();
            services.RegisterHlaMetadataDictionary(fetchHlaMetadataDictionarySettings, fetchApplicationInsightsSettings, fetchMacDictionarySettings);

            // TODO: ATLAS-327: Inject settings
            services.RegisterDonorReader(sp => sp.GetService<IConfiguration>().GetSection("ConnectionStrings")["DonorImportSql"]);
        }

        public static void RegisterMatchingAlgorithmDonorManagement(
            this IServiceCollection services,
            Func<IServiceProvider, ApplicationInsightsSettings> fetchApplicationInsightsSettings,
            Func<IServiceProvider, AzureStorageSettings> fetchAzureStorageSettings,
            Func<IServiceProvider, DonorManagementSettings> fetchDonorManagementSettings,
            Func<IServiceProvider, HlaMetadataDictionarySettings> fetchHlaMetadataDictionarySettings,
            Func<IServiceProvider, MacDictionarySettings> fetchMacDictionarySettings,
            Func<IServiceProvider, MessagingServiceBusSettings> fetchMessagingServiceBusSettings,
            Func<IServiceProvider, NotificationsServiceBusSettings> fetchNotificationsServiceBusSettings
        )
        {
            services.RegisterSettingsForMatchingDonorManagement(
                fetchApplicationInsightsSettings,
                fetchAzureStorageSettings,
                fetchDonorManagementSettings,
                fetchMessagingServiceBusSettings,
                fetchNotificationsServiceBusSettings
            );
            services.RegisterMatchingAlgorithmServices();
            services.RegisterDataServices();
            services.RegisterHlaMetadataDictionary(fetchHlaMetadataDictionarySettings, fetchApplicationInsightsSettings, fetchMacDictionarySettings);
            services.RegisterDonorManagementServices();
        }

        private static void RegisterMatchingAlgorithmServices(this IServiceCollection services)
        {
            services.AddScoped(sp => new ConnectionStrings
            {
                Persistent = sp.GetService<IConfiguration>().GetSection("ConnectionStrings")["PersistentSql"],
                TransientA = sp.GetService<IConfiguration>().GetSection("ConnectionStrings")["SqlA"],
                TransientB = sp.GetService<IConfiguration>().GetSection("ConnectionStrings")["SqlB"],
            });

            services.AddSingleton<IMemoryCache, MemoryCache>(sp => new MemoryCache(new MemoryCacheOptions()));

            services.AddSingleton(sp => AutomapperConfig.CreateMapper());

            services.AddScoped<IThreadSleeper, ThreadSleeper>();

            services.AddScoped<ISearchRequestContext, SearchRequestContext>();
            services.AddApplicationInsightsTelemetryWorkerService();
            services.AddScoped<ILogger>(sp => new SearchRequestAwareLogger(
                sp.GetService<ISearchRequestContext>(),
                sp.GetService<TelemetryClient>(),
                sp.GetService<IOptions<ApplicationInsightsSettings>>().Value.LogLevel.ToLogLevel()));

            services.RegisterLifeTimeScopedCacheTypes();

            services.AddScoped<ActiveTransientSqlConnectionStringProvider>();
            services.AddScoped<DormantTransientSqlConnectionStringProvider>();
            services.AddScoped<IActiveDatabaseProvider, ActiveDatabaseProvider>();
            services.AddScoped<IAzureDatabaseNameProvider, AzureDatabaseNameProvider>();

            services.AddScoped<IDonorScoringService, DonorScoringService>();
            services.AddScoped<IDonorService, DonorService>();
            services.AddScoped<IDonorHlaExpanderFactory, DonorHlaExpanderFactory>();

            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<IFailedDonorsNotificationSender, FailedDonorsNotificationSender>();
            services.AddScoped<IDonorInfoConverter, DonorInfoConverter>();
            services.AddScoped<IDonorImporter, DonorImporter>();
            services.AddScoped<IHlaProcessor, HlaProcessor>();
            services.AddScoped<IDataRefreshOrchestrator, DataRefreshOrchestrator>();
            services.AddScoped<IDataRefreshRunner, DataRefreshRunner>();
            services.AddScoped<IDataRefreshNotificationSender, DataRefreshNotificationSender>();
            services.AddScoped<IDataRefreshCleanupService, DataRefreshCleanupService>();

            // Matching Services
            services.AddScoped<IMatchingService, MatchingService>();
            services.AddScoped<IDonorMatchingService, DonorMatchingService>();
            services.AddScoped<IPreFilteredDonorMatchingService, PreFilteredDonorMatchingService>();
            services.AddScoped<IMatchFilteringService, MatchFilteringService>();
            services.AddScoped<IMatchCriteriaAnalyser, MatchCriteriaAnalyser>();
            services.AddScoped<IDatabaseFilteringAnalyser, DatabaseFilteringAnalyser>();

            // Scoring Services
            services.AddScoped<IDonorScoringService, DonorScoringService>();
            services.AddScoped<IGradingService, GradingService>();
            services.AddScoped<IConfidenceService, ConfidenceService>();
            services.AddScoped<IConfidenceCalculator, ConfidenceCalculator>();
            services.AddScoped<IRankingService, RankingService>();
            services.AddScoped<IMatchScoreCalculator, MatchScoreCalculator>();
            services.AddScoped<IScoringRequestService, ScoringRequestService>();
            services.AddScoped<IPermissiveMismatchCalculator, PermissiveMismatchCalculator>();
            services.AddScoped<IScoreResultAggregator, ScoreResultAggregator>();

            services.RegisterCommonGeneticServices();
            services.RegisterCommonMatchingServices();

            services.AddScoped<IActiveHlaNomenclatureVersionAccessor, ActiveHlaNomenclatureVersionAccessor>();

            services.AddScoped<ISearchServiceBusClient, SearchServiceBusClient>(sp =>
            {
                var serviceBusSettings = sp.GetService<IOptions<MessagingServiceBusSettings>>().Value;
                return new SearchServiceBusClient(
                    serviceBusSettings.ConnectionString,
                    serviceBusSettings.SearchRequestsQueue,
                    serviceBusSettings.SearchResultsTopic
                );
            });
            services.AddScoped<ISearchDispatcher, SearchDispatcher>();
            services.AddScoped<ISearchRunner, SearchRunner>();
            services.AddScoped<IResultsBlobStorageClient, ResultsBlobStorageClient>();

            services.AddScoped<IAzureDatabaseManagementClient, AzureDatabaseManagementClient>();
            services.AddScoped<IAzureAppServiceManagementClient, AzureAppServiceManagementClient>();
            services.AddScoped<IAzureAuthenticationClient, AzureAuthenticationClient>();
            services.AddScoped<IAzureFunctionManager, AzureFunctionManager>();
            services.AddScoped<IAzureDatabaseManager, AzureDatabaseManager>();

            services.RegisterNotificationSender(
                OptionsReaderFor<NotificationsServiceBusSettings>(),
                OptionsReaderFor<ApplicationInsightsSettings>()
            );

            services.AddScoped<IScoringCache, ScoringCache>();
        }

        private static void RegisterDataServices(this IServiceCollection services)
        {
            services.AddScoped<IActiveRepositoryFactory, ActiveRepositoryFactory>();
            services.AddScoped<IDormantRepositoryFactory, DormantRepositoryFactory>();
            // Persistent storage
            services.AddScoped(sp =>
            {
                var persistentConnectionString = sp.GetService<IConfiguration>().GetSection("ConnectionStrings")["PersistentSql"];
                return new ContextFactory().Create(persistentConnectionString);
            });
            services.AddScoped<IScoringWeightingRepository, ScoringWeightingRepository>();
            services.AddScoped<IDataRefreshHistoryRepository, DataRefreshHistoryRepository>();
        }

        private static void RegisterDonorManagementServices(this IServiceCollection services)
        {
            services.AddScoped<IDonorManagementService, DonorManagementService>();
            services.AddScoped<ISearchableDonorUpdateConverter, SearchableDonorUpdateConverter>();

            services.AddSingleton<IMessageReceiverFactory, MessageReceiverFactory>(sp =>
                new MessageReceiverFactory(sp.GetService<IOptions<MessagingServiceBusSettings>>().Value.ConnectionString)
            );

            services.AddScoped<IServiceBusMessageReceiver<SearchableDonorUpdate>, ServiceBusMessageReceiver<SearchableDonorUpdate>>(sp =>
            {
                var settings = sp.GetService<IOptions<DonorManagementSettings>>().Value;
                var factory = sp.GetService<IMessageReceiverFactory>();
                return new ServiceBusMessageReceiver<SearchableDonorUpdate>(factory, settings.Topic, settings.Subscription);
            });

            services.AddScoped<IMessageProcessor<SearchableDonorUpdate>, MessageProcessor<SearchableDonorUpdate>>(sp =>
            {
                var messageReceiver = sp.GetService<IServiceBusMessageReceiver<SearchableDonorUpdate>>();
                return new MessageProcessor<SearchableDonorUpdate>(messageReceiver);
            });

            services.AddScoped<IDonorUpdateProcessor, DonorUpdateProcessor>(sp =>
            {
                var messageReceiverService = sp.GetService<IMessageProcessor<SearchableDonorUpdate>>();
                var managementService = sp.GetService<IDonorManagementService>();
                var updateConverter = sp.GetService<ISearchableDonorUpdateConverter>();
                var logger = sp.GetService<ILogger>();
                var settings = sp.GetService<IOptions<DonorManagementSettings>>().Value;

                return new DonorUpdateProcessor(
                    messageReceiverService,
                    managementService,
                    updateConverter,
                    logger,
                    int.Parse(settings.BatchSize));
            });
        }

        private static void RegisterSharedSettings(
            this IServiceCollection services,
            Func<IServiceProvider, ApplicationInsightsSettings> fetchApplicationInsightsSettings,
            Func<IServiceProvider, AzureStorageSettings> fetchAzureStorageSettings,
            Func<IServiceProvider, MessagingServiceBusSettings> fetchMessagingServiceBusSettings,
            Func<IServiceProvider, NotificationsServiceBusSettings> fetchNotificationsServiceBusSettings
        )
        {
            services.AddScoped(fetchApplicationInsightsSettings);
            services.AddScoped(fetchAzureStorageSettings);
            services.AddScoped(fetchMessagingServiceBusSettings);
            services.AddScoped(fetchNotificationsServiceBusSettings);
        }

        private static void RegisterSettingsForMatchingDonorManagement(
            this IServiceCollection services,
            Func<IServiceProvider, ApplicationInsightsSettings> fetchApplicationInsightsSettings,
            Func<IServiceProvider, AzureStorageSettings> fetchAzureStorageSettings,
            Func<IServiceProvider, DonorManagementSettings> fetchDonorManagementSettings,
            Func<IServiceProvider, MessagingServiceBusSettings> fetchMessagingServiceBusSettings,
            Func<IServiceProvider, NotificationsServiceBusSettings> fetchNotificationsServiceBusSettings
        )
        {
            services.RegisterSharedSettings(
                fetchApplicationInsightsSettings,
                fetchAzureStorageSettings,
                fetchMessagingServiceBusSettings,
                fetchNotificationsServiceBusSettings
            );

            services.AddScoped(fetchDonorManagementSettings);
        }

        private static void RegisterSettingsForMatchingAlgorithm(
            this IServiceCollection services,
            Func<IServiceProvider, ApplicationInsightsSettings> fetchApplicationInsightsSettings,
            Func<IServiceProvider, AzureAuthenticationSettings> fetchAzureAuthenticationSettings,
            Func<IServiceProvider, AzureAppServiceManagementSettings> fetchAzureAppServiceManagementSettings,
            Func<IServiceProvider, AzureDatabaseManagementSettings> fetchAzureDatabaseManagementSettings,
            Func<IServiceProvider, AzureStorageSettings> fetchAzureStorageSettings,
            Func<IServiceProvider, DataRefreshSettings> fetchDataRefreshSettings,
            Func<IServiceProvider, MessagingServiceBusSettings> fetchMessagingServiceBusSettings,
            Func<IServiceProvider, NotificationsServiceBusSettings> fetchNotificationsServiceBusSettings)
        {
            services.RegisterSharedSettings(
                fetchApplicationInsightsSettings,
                fetchAzureStorageSettings,
                fetchMessagingServiceBusSettings,
                fetchNotificationsServiceBusSettings
            );

            services.AddScoped(fetchAzureAuthenticationSettings);
            services.AddScoped(fetchAzureAppServiceManagementSettings);
            services.AddScoped(fetchAzureDatabaseManagementSettings);
            services.AddScoped(fetchDataRefreshSettings);
        }
    }
}