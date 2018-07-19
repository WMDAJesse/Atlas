﻿using Microsoft.Extensions.Caching.Memory;
using Nova.SearchAlgorithm.Common.Exceptions;
using Nova.SearchAlgorithm.Common.Repositories;
using Nova.SearchAlgorithm.MatchingDictionary.Repositories.AzureStorage;
using System.Collections.Generic;
using System.Linq;

namespace Nova.SearchAlgorithm.MatchingDictionary.Repositories
{
    public interface IHlaMatchingLookupRepository : IHlaLookupRepository
    {
        IEnumerable<string> GetAllPGroups();
    }

    public class HlaMatchingLookupRepository : HlaLookupRepositoryBase, IHlaMatchingLookupRepository
    {
        private const string DataTableReferencePrefix = "HlaMatchingLookupData";
        private const string CacheKey = "HlaMatchingLookup";

        public HlaMatchingLookupRepository(
            ICloudTableFactory factory, 
            ITableReferenceRepository tableReferenceRepository,
            IMemoryCache memoryCache)
            : base(factory, tableReferenceRepository, DataTableReferencePrefix, memoryCache, CacheKey)
        {
        }

        public IEnumerable<string> GetAllPGroups()
        {
            if (MemoryCache.TryGetValue(CacheKey, out Dictionary<string, HlaLookupTableEntity> matchingDictionary))
            {
                return matchingDictionary.Values.SelectMany(v => v.ToHlaMatchingLookupResult().MatchingPGroups);
            }
            throw new MemoryCacheException($"{CacheKey} table not cached!");
        }            
    }
}
