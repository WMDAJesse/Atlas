using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Nova.SearchAlgorithm.Data.Persistent
{
    public class ContextFactory : IDesignTimeDbContextFactory<SearchAlgorithmPersistentContext>
    {
        // This method is called by entity framework to create a context when generating/running migrations
        public SearchAlgorithmPersistentContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json");

            var config = builder.Build();

            var connectionString = config.GetConnectionString("PersistentSql");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Could not find a default connection string..");
            }

            return Create(connectionString);
        }

        public SearchAlgorithmPersistentContext Create(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException($"{nameof(connectionString)} is null or empty.", nameof(connectionString));
            }

            var optionsBuilder = new DbContextOptionsBuilder<SearchAlgorithmPersistentContext>();

            optionsBuilder.UseSqlServer(connectionString);

            return new SearchAlgorithmPersistentContext(optionsBuilder.Options);
        }
    }
}