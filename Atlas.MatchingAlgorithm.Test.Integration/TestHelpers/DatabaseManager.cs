using System;
using Atlas.MatchingAlgorithm.Data.Context;
using Atlas.MatchingAlgorithm.Data.Persistent.Context;
using Atlas.MatchingAlgorithm.Data.Persistent.Models;
using Atlas.MatchingAlgorithm.Services.ConfigurationProviders.TransientSqlDatabase.ConnectionStringProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ContextFactory = Atlas.MatchingAlgorithm.Data.Context.ContextFactory;

namespace Atlas.MatchingAlgorithm.Test.Integration.TestHelpers
{
    internal class DatabaseManager
    {
        /// <summary>
        /// Creates if necessary, and runs migrations on both a transient and persistent database
        /// </summary>
        public static void MigrateDatabases()
        {
            var transientContext = DependencyInjection.DependencyInjection.Provider.GetService<SearchAlgorithmContext>();
            var persistentContext = DependencyInjection.DependencyInjection.Provider.GetService<SearchAlgorithmPersistentContext>();

            var connectionStringB =
                DependencyInjection.DependencyInjection.Provider.GetService<IConfiguration>().GetSection("ConnectionStrings")["SqlB"];
            var transientContextB = new ContextFactory().Create(connectionStringB);

            if (transientContext == null || persistentContext == null)
            {
                throw new Exception("Database context could resolved - DI has not been correctly configured.");
            }

            transientContext.Database.Migrate();
            transientContextB?.Database.Migrate();
            persistentContext.Database.Migrate();
        }

        /// <summary>
        /// Clears the test database of data. Can be accessed by fixtures to run after each fixture, but not after each test.
        /// </summary>
        public static void ClearDatabases()
        {
            var connStringFactory = DependencyInjection.DependencyInjection.Provider.GetService<StaticallyChosenTransientSqlConnectionStringProviderFactory>();
            var connStringA = connStringFactory.GenerateConnectionStringProvider(TransientDatabase.DatabaseA).GetConnectionString();
            var connStringB = connStringFactory.GenerateConnectionStringProvider(TransientDatabase.DatabaseB).GetConnectionString();

            var transientContextA = new ContextFactory().Create(connStringA);
            var transientContextB = new ContextFactory().Create(connStringB);
            ClearTransientDatabase(transientContextA);
            ClearTransientDatabase(transientContextB);

            var persistentContext = DependencyInjection.DependencyInjection.Provider.GetService<SearchAlgorithmPersistentContext>();
            ClearPersistentDatabase(persistentContext);
        }

        private static void ClearTransientDatabase(DbContext context)
        {
            if (context != null)
            {
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE [DonorManagementLogs]");
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE [Donors]");
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE [MatchingHlaAtA]");
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE [MatchingHlaAtB]");
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE [MatchingHlaAtC]");
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE [MatchingHlaAtDrb1]");
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE [MatchingHlaAtDqb1]");
                context.Database.ExecuteSqlRaw("DELETE FROM PGroupNames");
            }
        }

        private static void ClearPersistentDatabase(SearchAlgorithmPersistentContext context)
        {
            context?.Database.ExecuteSqlRaw("TRUNCATE TABLE [DataRefreshHistory]");
        }
    }
}