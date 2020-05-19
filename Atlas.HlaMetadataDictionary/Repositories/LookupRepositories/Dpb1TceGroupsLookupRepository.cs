﻿using Atlas.Common.Caching;
using Atlas.HlaMetadataDictionary.Repositories.AzureStorage;

namespace Atlas.HlaMetadataDictionary.Repositories.LookupRepositories
{
    internal interface IDpb1TceGroupsLookupRepository : IHlaLookupRepository
    {
    }

    internal class Dpb1TceGroupsLookupRepository : HlaLookupRepositoryBase, IDpb1TceGroupsLookupRepository
    {
        private const string DataTableReferencePrefix = "Dpb1TceGroupsLookupData";
        private const string CacheKey = "Dpb1TceGroupsLookup";

        public Dpb1TceGroupsLookupRepository(
            ICloudTableFactory factory,
            ITableReferenceRepository tableReferenceRepository,
            IPersistentCacheProvider cacheProvider)
            : base(factory, tableReferenceRepository, DataTableReferencePrefix, cacheProvider, CacheKey)
        {
        }
    }
}