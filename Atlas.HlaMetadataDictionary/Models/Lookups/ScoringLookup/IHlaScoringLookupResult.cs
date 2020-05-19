﻿using System.Collections.Generic;

namespace Atlas.HlaMetadataDictionary.Models.Lookups.ScoringLookup
{
    /// <summary>
    /// Lookup result with data required to score HLA pairings.
    /// </summary>
    internal interface IHlaScoringLookupResult : IHlaLookupResult
    {
        IHlaScoringInfo HlaScoringInfo { get; }
        IEnumerable<IHlaScoringLookupResult> GetInTermsOfSingleAlleleScoringMetadata();
    }
}
