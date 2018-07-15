﻿using Nova.SearchAlgorithm.MatchingDictionary.Models.HLATypings;
using Nova.SearchAlgorithm.MatchingDictionary.Models.Lookups;
using Nova.SearchAlgorithm.MatchingDictionary.Models.Lookups.MatchingLookup;
using System.Collections.Generic;
using System.Linq;

namespace Nova.SearchAlgorithm.MatchingDictionary.Services.HlaMatchPreCalculation
{
    public interface IHlaMatchingLookupResultGenerator : IHlaLookupResultGenerator
    {
        
    }

    public class HlaMatchingLookupResultGenerator : 
        HlaLookupResultGeneratorBase<HlaMatchingLookupResult>, IHlaMatchingLookupResultGenerator
    {
        protected override IEnumerable<IHlaLookupResult> GetHlaLookupResultsFromMatchedSerologies(
            IEnumerable<IHlaLookupResultSource<SerologyTyping>> matchedSerology)
        {
            return matchedSerology.Select(serology => new HlaMatchingLookupResult(serology));
        }

        protected override IEnumerable<HlaMatchingLookupResult> GetHlaMatchingLookupResultsForEachAlleleLookupName(
            IHlaLookupResultSource<AlleleTyping> matchedAllele)
        {
            var lookupNames = GetAlleleLookupNames(matchedAllele.TypingForMatchingDictionary);

            return lookupNames.Select(lookupName => new HlaMatchingLookupResult(matchedAllele, lookupName));
        }

        protected override IEnumerable<IHlaLookupResult> GroupResultsToMergeDuplicatesCausedByAlleleNameTruncation(
            IEnumerable<HlaMatchingLookupResult> hlaLookupResults)
        {
            return hlaLookupResults
                .GroupBy(e => new { e.MatchLocus, e.LookupName })
                .Select(e => new HlaMatchingLookupResult(
                    e.Key.MatchLocus,
                    e.Key.LookupName,
                    TypingMethod.Molecular,
                    e.SelectMany(p => p.MatchingPGroups).Distinct()
                ));
        }
   }
}
