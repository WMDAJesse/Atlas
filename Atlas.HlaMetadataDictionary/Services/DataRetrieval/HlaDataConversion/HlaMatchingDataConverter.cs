﻿using Atlas.Common.GeneticData.Hla.Models;
using Atlas.HlaMetadataDictionary.Extensions;
using Atlas.HlaMetadataDictionary.Models.HLATypings;
using Atlas.HlaMetadataDictionary.Models.Lookups;
using Atlas.HlaMetadataDictionary.Models.Lookups.MatchingLookup;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Atlas.HlaMetadataDictionary.Services.DataRetrieval.HlaDataConversion
{
    /// <summary>
    /// Converts Matched HLA to model optimised for HLA Scoring lookups.
    /// </summary>
    internal interface IHlaMatchingDataConverter : IMatchedHlaDataConverterBase
    {
    }

    internal class HlaMatchingDataConverter :
        MatchedHlaDataConverterBase,
        IHlaMatchingDataConverter
    {
        protected override ISerialisableHlaMetadata GetSerologyLookupResult(IHlaLookupResultSource<SerologyTyping> lookupResultSource)
        {
            return new HlaMatchingLookupResult(
                    lookupResultSource.TypingForHlaLookupResult.Locus,
                    lookupResultSource.TypingForHlaLookupResult.Name,
                    TypingMethod.Serology,
                    lookupResultSource.MatchingPGroups);
        }

        protected override ISerialisableHlaMetadata GetSingleAlleleLookupResult(
            IHlaLookupResultSource<AlleleTyping> lookupResultSource)
        {
            return GetMolecularLookupResult(
                new[] { lookupResultSource },
                allele => allele.Name
            );
        }

        protected override ISerialisableHlaMetadata GetNmdpCodeAlleleLookupResult(
            IEnumerable<IHlaLookupResultSource<AlleleTyping>> lookupResultSources,
            string nmdpLookupName)
        {
            return GetMolecularLookupResult(
                lookupResultSources,
                allele => nmdpLookupName
            );
        }

        protected override ISerialisableHlaMetadata GetXxCodeLookupResult(
            IEnumerable<IHlaLookupResultSource<AlleleTyping>> lookupResultSources)
        {
            return GetMolecularLookupResult(
                lookupResultSources,
                allele => allele.ToXxCodeLookupName()
            );
        }
        
        private static ISerialisableHlaMetadata GetMolecularLookupResult(
            IEnumerable<IHlaLookupResultSource<AlleleTyping>> lookupResultSources,
            Func<AlleleTyping, string> getLookupName)
        {
            var sources = lookupResultSources.ToList();

            var firstAllele = sources
                .First()
                .TypingForHlaLookupResult;

            var pGroups = sources
                .SelectMany(resultSource => resultSource.MatchingPGroups)
                .Distinct();

            return new HlaMatchingLookupResult(
                firstAllele.Locus,
                getLookupName(firstAllele),
                TypingMethod.Molecular,
                pGroups
                );
        }
    }
}
