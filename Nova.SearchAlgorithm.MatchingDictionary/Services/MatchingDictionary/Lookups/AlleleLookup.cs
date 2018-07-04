﻿using Nova.SearchAlgorithm.MatchingDictionary.Models.HLATypings;
using Nova.SearchAlgorithm.MatchingDictionary.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nova.SearchAlgorithm.MatchingDictionary.Services.MatchingDictionary.Lookups
{
    internal class AlleleLookup : AlleleNameBasedLookup
    {
        public AlleleLookup(
            IMatchingDictionaryRepository dictionaryRepository,
            IAlleleNamesLookupService alleleNamesLookupService)
            : base(dictionaryRepository, alleleNamesLookupService)
        {
        }

        protected override async Task<IEnumerable<string>> GetAllelesNames(MatchLocus matchLocus, string lookupName)
        {
            return await Task.FromResult((IEnumerable<string>)new[] { lookupName });
        }
    }
}