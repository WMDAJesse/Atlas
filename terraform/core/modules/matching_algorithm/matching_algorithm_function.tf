locals {
  matching_func_app_settings = {
    "ApplicationInsights:LogLevel" = var.APPLICATION_INSIGHTS_LOG_LEVEL

    "AzureFunctionsJobHost__extensions__serviceBus__messageHandlerOptions__maxConcurrentCalls" = var.MAX_CONCURRENT_SERVICEBUS_FUNCTIONS

    "AzureManagement:Authentication:ClientId"                   = var.AZURE_CLIENT_ID
    "AzureManagement:Authentication:ClientSecret"               = var.AZURE_CLIENT_SECRET
    "AzureManagement:Authentication:OAuthBaseUrl"               = var.AZURE_OAUTH_BASEURL
    "AzureManagement:Database:ServerName"                       = var.sql_server.name
    "AzureManagement:Database:PollingRetryIntervalMilliseconds" = var.DATABASE_OPERATION_POLLING_INTERVAL_MILLISECONDS
    "AzureManagement:Database:ResourceGroupName"                = var.resource_group.name
    "AzureManagement:Database:SubscriptionId"                   = var.general.subscription_id

    "AzureStorage:ConnectionString"           = var.azure_storage.primary_connection_string
    "AzureStorage:SearchResultsBlobContainer" = azurerm_storage_container.search_matching_results_blob_container.name
    "AzureStorage:BatchSize"                  = "0"

    "DataRefresh:ActiveDatabaseAutoPauseTimeout"                                            = var.DATA_REFRESH_DB_AUTO_PAUSE_ACTIVE
    "DataRefresh:ActiveDatabaseSize"                                                        = var.DATA_REFRESH_DB_SIZE_ACTIVE
    "DataRefresh:AutoRunDataRefresh"                                                        = var.DATA_REFRESH_AUTO_RUN
    "DataRefresh:RequestsTopic"                                                             = azurerm_servicebus_topic.data-refresh-requests.name
    "DataRefresh:RequestsTopicSubscription"                                                 = azurerm_servicebus_subscription.matching-algorithm-data-refresh-requests.name
    "DataRefresh:CompletionTopic"                                                           = azurerm_servicebus_topic.completed-data-refresh-jobs.name
    "DataRefresh:CronTab"                                                                   = var.DATA_REFRESH_CRONTAB
    "DataRefresh:DatabaseAName"                                                             = azurerm_mssql_database.atlas-matching-transient-a.name
    "DataRefresh:DatabaseBName"                                                             = azurerm_mssql_database.atlas-matching-transient-b.name
    "DataRefresh:DataRefreshDonorUpdatesShouldBeFullyTransactional"                         = var.DONOR_WRITE_TRANSACTIONALITY__DATA_REFRESH
    "DataRefresh:DonorManagement:BatchSize"                                                 = var.MESSAGING_BUS_DONOR_BATCH_SIZE
    "DataRefresh:DonorManagement:CronSchedule"                                              = "NotActuallyUsedInThisFunction"
    "DataRefresh:DonorManagement:OngoingDifferentialDonorUpdatesShouldBeFullyTransactional" = var.DONOR_WRITE_TRANSACTIONALITY__DONOR_UPDATES
    "DataRefresh:DonorManagement:SubscriptionForDbA"                                        = azurerm_servicebus_subscription.matching_transient_a.name
    "DataRefresh:DonorManagement:SubscriptionForDbB"                                        = azurerm_servicebus_subscription.matching_transient_b.name
    "DataRefresh:DonorManagement:Topic"                                                     = var.servicebus_topics.updated-searchable-donors.name
    "DataRefresh:DormantDatabaseAutoPauseTimeout"                                           = var.DATA_REFRESH_DB_AUTO_PAUSE_DORMANT
    "DataRefresh:DormantDatabaseSize"                                                       = var.DATA_REFRESH_DB_SIZE_DORMANT
    "DataRefresh:RefreshDatabaseSize"                                                       = var.DATA_REFRESH_DB_SIZE_REFRESH

    "HlaMetadataDictionary:AzureStorageConnectionString" = var.azure_storage.primary_connection_string,
    "HlaMetadataDictionary:HlaNomenclatureSourceUrl"     = var.WMDA_FILE_URL,

    "MacDictionary:AzureStorageConnectionString" = var.azure_storage.primary_connection_string
    "MacDictionary:TableName"                    = var.mac_import_table.name,

    "MatchingConfiguration:MatchingBatchSize" = var.MATCHING_BATCH_SIZE,

    "MessagingServiceBus:ConnectionString"               = var.servicebus_namespace_authorization_rules.read-write.primary_connection_string
    "MessagingServiceBus:SearchRequestsMaxDeliveryCount" = azurerm_servicebus_subscription.matching-requests-matching-algorithm.max_delivery_count
    "MessagingServiceBus:SearchRequestsSubscription"     = azurerm_servicebus_subscription.matching-requests-matching-algorithm.name
    "MessagingServiceBus:SearchRequestsTopic"            = azurerm_servicebus_topic.matching-requests.name
    "MessagingServiceBus:SearchResultsTopic"             = azurerm_servicebus_topic.matching-results-ready.name

    "NotificationsServiceBus:ConnectionString"   = var.servicebus_namespace_authorization_rules.write-only.primary_connection_string
    "NotificationsServiceBus:AlertsTopic"        = var.servicebus_topics.alerts.name
    "NotificationsServiceBus:NotificationsTopic" = var.servicebus_topics.notifications.name

    "Wmda:WmdaFileUri" = var.WMDA_FILE_URL

    // maximum running instances of the algorithm = maximum_worker_count * maxConcurrentCalls (in host.json).
    // together these must ensure that the number of allowed concurrent SQL connections to the matching SQL DB is not exceeded.
    // See README_Integration.md for more details on concurrency configuration.
    "WEBSITE_MAX_DYNAMIC_APPLICATION_SCALE_OUT" = var.MAX_SCALE_OUT
    "WEBSITE_RUN_FROM_PACKAGE"                  = var.WEBSITE_RUN_FROM_PACKAGE
  }
  matching_algorithm_function_app_name = "${var.general.environment}-ATLAS-MATCHING-ALGORITHM-FUNCTIONS"
}

resource "azurerm_windows_function_app" "atlas_matching_algorithm_function" {
  name                        = local.matching_algorithm_function_app_name
  resource_group_name         = var.resource_group.name
  location                    = var.general.location
  service_plan_id             = var.elastic_app_service_plan.id
  client_certificate_mode     = "Required"
  https_only                  = true
  functions_extension_version = "~4"
  storage_account_access_key  = azurerm_storage_account.matching_function_storage.primary_access_key
  storage_account_name        = azurerm_storage_account.matching_function_storage.name

  site_config {
    application_insights_key = var.application_insights.instrumentation_key
    application_stack {
      dotnet_version = "6"
    }
    cors {
      allowed_origins     = []
      support_credentials = false
    }
    pre_warmed_instance_count = 1
    use_32_bit_worker         = false
    ip_restriction = [for ip in var.IP_RESTRICTION_SETTINGS : {
      ip_address = ip
      subnet_id  = null
    }]
    ftps_state              = "AllAllowed"
    scm_minimum_tls_version = "1.0"
  }

  tags = var.general.common_tags

  app_settings = local.matching_func_app_settings

  connection_string {
    name  = "SqlA"
    type  = "SQLAzure"
    value = local.matching_transient_database_a_connection_string
  }
  connection_string {
    name  = "SqlB"
    type  = "SQLAzure"
    value = local.matching_transient_database_b_connection_string
  }
  connection_string {
    name  = "PersistentSql"
    type  = "SQLAzure"
    value = local.matching_persistent_database_connection_string
  }

  connection_string {
    name  = "DonorSql"
    type  = "SQLAzure"
    value = "Server=tcp:${var.sql_server.fully_qualified_domain_name},1433;Initial Catalog=${var.donor_import_sql_database.name};Persist Security Info=False;User ID=${var.DONOR_IMPORT_DATABASE_USERNAME};Password=${var.DONOR_IMPORT_DATABASE_PASSWORD};MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=1800;"
  }
}
