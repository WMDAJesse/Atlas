﻿using Microsoft.Extensions.Caching.Memory;
using Microsoft.WindowsAzure.Storage.Table;
using Nova.SearchAlgorithm.Common.Exceptions;
using Nova.SearchAlgorithm.Common.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.SearchAlgorithm.Common.Repositories
{
    public abstract class LookupRepositoryBase<TStorable, TTableEntity>
        where TTableEntity : TableEntity, new()
        where TStorable : IStorableInCloudTable<TTableEntity>
    {
        protected readonly IMemoryCache MemoryCache;

        private readonly ICloudTableFactory tableFactory;
        private readonly ITableReferenceRepository tableReferenceRepository;
        private readonly string tableReferencePrefix;
        private readonly string cacheKey;
        private CloudTable cloudTable;

        protected LookupRepositoryBase(
            ICloudTableFactory factory,
            ITableReferenceRepository tableReferenceRepository,
            string tableReferencePrefix,
            IMemoryCache memoryCache,
            string cacheKey)
        {
            tableFactory = factory;
            this.tableReferenceRepository = tableReferenceRepository;
            this.tableReferencePrefix = tableReferencePrefix;
            MemoryCache = memoryCache;
            this.cacheKey = cacheKey;
        }

        protected abstract IEnumerable<string> GetTablePartitions();

        protected async Task RecreateDataTable(IEnumerable<TStorable> tableContents, IEnumerable<string> partitions)
        {
            var newDataTable = CreateNewDataTable();
            InsertIntoDataTable(tableContents, partitions, newDataTable);
            await tableReferenceRepository.UpdateTableReference(tableReferencePrefix, newDataTable.Name);
            cloudTable = null;
        }

        protected async Task<TTableEntity> GetDataIfExists(string partition, string rowKey)
        {
            if (MemoryCache.TryGetValue(cacheKey, out Dictionary<string, TTableEntity> tableEntities))
            {
                return GetDataFromCache(partition, rowKey, tableEntities);
            }

            await LoadDataIntoMemory();
            if (MemoryCache.TryGetValue(cacheKey, out tableEntities))
            {
                return GetDataFromCache(partition, rowKey, tableEntities);
            }

            throw new MemoryCacheException($"Failed to load data into the {cacheKey} cache");
        }

        /// <summary>
        /// The connection to the current data table is cached so we don't open unnecessary connections
        /// If you plan to use this repository with multiple async operations, this method should be called first
        /// </summary>
        protected async Task LoadDataIntoMemory()
        {
            var currentDataTable = await GetCurrentDataTable();
            var tableResults = new CloudTableBatchQueryAsync<TTableEntity>(currentDataTable);
            var dataToLoad = new Dictionary<string, TTableEntity>();

            while (tableResults.HasMoreResults)
            {
                var results = await tableResults.RequestNextAsync();
                foreach (var result in results)
                {
                    dataToLoad.Add(result.PartitionKey + result.RowKey, result);
                }
            }

            MemoryCache.Set(cacheKey, dataToLoad);
        }

        private async Task<CloudTable> GetCurrentDataTable()
        {
            if (cloudTable == null)
            {
                var dataTableReference = await tableReferenceRepository.GetCurrentTableReference(tableReferencePrefix);
                cloudTable = tableFactory.GetTable(dataTableReference);
            }
            return cloudTable;
        }

        private static TTableEntity GetDataFromCache(
            string partition, string rowKey, IReadOnlyDictionary<string, TTableEntity> matchingDictionary)
        {
            matchingDictionary.TryGetValue(partition + rowKey, out var tableEntity);
            return tableEntity;
        }

        private CloudTable CreateNewDataTable()
        {
            var dataTableReference = tableReferenceRepository.GetNewTableReference(tableReferencePrefix);
            return tableFactory.GetTable(dataTableReference);
        }

        private static void InsertIntoDataTable(IEnumerable<TStorable> contents, IEnumerable<string> partitions, CloudTable dataTable)
        {
            var entities = contents
                .Select(data => data.ConvertToTableEntity())
                .ToList();

            foreach (var partition in partitions)
            {
                var partitionedEntities = entities
                    .Where(entity => entity.PartitionKey.Equals(partition));
                
                dataTable.BatchInsert(partitionedEntities);
            }
        }
    }
}
