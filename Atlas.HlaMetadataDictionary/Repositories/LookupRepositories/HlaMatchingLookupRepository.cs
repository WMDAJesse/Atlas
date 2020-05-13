﻿using System.Collections.Generic;
using System.Linq;
using LazyCache;
using Atlas.HlaMetadataDictionary.Exceptions;
using Atlas.HlaMetadataDictionary.Repositories.AzureStorage;

namespace Atlas.HlaMetadataDictionary.Repositories
{
    public interface IHlaMatchingLookupRepository : IHlaLookupRepository
    {
        IEnumerable<string> GetAllPGroups(string hlaDatabaseVersion);
    }

    public class HlaMatchingLookupRepository : HlaLookupRepositoryBase, IHlaMatchingLookupRepository
    {
        private const string DataTableReferencePrefix = "HlaMatchingLookupData";
        private const string CacheKey = "HlaMatchingLookup";

        public HlaMatchingLookupRepository(
            ICloudTableFactory factory, 
            ITableReferenceRepository tableReferenceRepository,
            IAppCache cache)
            : base(factory, tableReferenceRepository, DataTableReferencePrefix, cache, CacheKey)
        {
        }

        public IEnumerable<string> GetAllPGroups(string hlaDatabaseVersion)
        {
            var versionedCacheKey = VersionedCacheKey(hlaDatabaseVersion);
            var matchingDictionary = cache.Get<Dictionary<string, HlaLookupTableEntity>>(versionedCacheKey);
            if (matchingDictionary != null)
            {
                return matchingDictionary.Values.SelectMany(v => v.ToHlaMatchingLookupResult()?.MatchingPGroups);
            }
            throw new MemoryCacheException($"{versionedCacheKey} table not cached!");
        }            
    }
}
