﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Atlas.Common.AzureStorage.TableStorage;
using Atlas.MultipleAlleleCodeDictionary.AzureStorage.Models;
using Atlas.MultipleAlleleCodeDictionary.ExternalInterface.Models;
using Atlas.MultipleAlleleCodeDictionary.Services;
using Atlas.MultipleAlleleCodeDictionary.Settings;
using Microsoft.Azure.Cosmos.Table;
using QueryComparisons = Microsoft.WindowsAzure.Storage.Table.QueryComparisons;

namespace Atlas.MultipleAlleleCodeDictionary.AzureStorage.Repositories
{
    internal interface IMacRepository
    {
        public Task<string> GetLastMacEntry();
        public Task InsertMacs(IEnumerable<Mac> macCodes);
        public Task<Mac> GetMac(string macCode);
        public Task<IEnumerable<Mac>> GetAllMacs();
    }

    internal class MacRepository : IMacRepository
    {
        protected readonly CloudTable Table;

        public MacRepository(MacDictionarySettings macDictionarySettings)
        {
            var connectionString = macDictionarySettings.AzureStorageConnectionString;
            var tableName = macDictionarySettings.TableName;
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            Table = tableClient.GetTableReference(tableName);
        }

        public async Task<string> GetLastMacEntry()
        {
            var query = new TableQuery<MacEntity>();
            var result = await Table.ExecuteQueryAsync(query);
            return result.InOrderOfDefinition().FirstOrDefault()?.Code;
        }

        public async Task InsertMacs(IEnumerable<Mac> macs)
        {
            var macEntities = macs.InOrderOfDefinition().Select(mac => new MacEntity(mac));
            await Table.BatchInsert(macEntities);
        }

        public async Task<Mac> GetMac(string macCode)
        {
            var query = new TableQuery<MacEntity>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, macCode.Length.ToString()), //TODO: ATLAS-488. Rationalise these.
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, macCode)
                    ));
            var result = await Table.ExecuteQueryAsync(query);
            return new Mac(result.Single());
        }

        public async Task<IEnumerable<Mac>> GetAllMacs()
        {
            var query = new TableQuery<MacEntity>();
            var result = await Table.ExecuteQueryAsync(query);
            return result.Select(x => new Mac(x));
        }
    }

    internal static class SortingExtension
    {
        /// <summary>
        /// MACs are alphabetical within a character length - any new MACs are appended to the end of the list alphabetically.
        /// Order by length ensure that longer MACs are returned later.
        /// e.g. purely alphabetically, ABC comes before ZX, but as it has fewer character, ZX is actually the earlier MAC
        /// </summary>
        public static IOrderedEnumerable<THasMacCode> InOrderOfDefinition<THasMacCode>(this IEnumerable<THasMacCode> macs)
            where THasMacCode : IHasMacCode
        {
            return macs
                .OrderByDescending(x => x.Code.Length)   //TODO: ATLAS-488. Note that this is NOT the same as the partition Key! This is an international agreement between biologists about how MAC codes are defined. The PartitionKey is our personal decision about what we think will make a DB query quick!
                .ThenByDescending(x => x.Code);
        }
    }
}