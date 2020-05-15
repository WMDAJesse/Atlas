﻿using Atlas.MatchingAlgorithm.Common.Models;
using Atlas.HlaMetadataDictionary.Models.HLATypings;
using Atlas.HlaMetadataDictionary.Repositories.AzureStorage;
using Atlas.Utils.Models;

namespace Atlas.HlaMetadataDictionary.Models.Lookups.Dpb1TceGroupLookup
{
    public interface IDpb1TceGroupsLookupResult : IHlaLookupResult
    {
        string TceGroup { get; }
    }

    public class Dpb1TceGroupsLookupResult : IDpb1TceGroupsLookupResult
    {
        public Locus Locus => Locus.Dpb1;
        public string LookupName { get; }
        public TypingMethod TypingMethod => TypingMethod.Molecular;
        public string TceGroup { get; }
        public object HlaInfoToSerialise => TceGroup;

        public Dpb1TceGroupsLookupResult(
            string lookupName,
            string tceGroup)
        {
            LookupName = lookupName;
            TceGroup = tceGroup;
        }

        public HlaLookupTableEntity ConvertToTableEntity()
        {
            return new HlaLookupTableEntity(this);
        }
    }
}
