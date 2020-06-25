﻿using Atlas.Common.GeneticData;
using Atlas.Common.GeneticData.Hla.Services;
using Atlas.HlaMetadataDictionary.ExternalInterface.Models.Metadata;
using Atlas.HlaMetadataDictionary.InternalModels.MetadataTableRows;
using Atlas.HlaMetadataDictionary.Repositories.MetadataRepositories;
using Atlas.HlaMetadataDictionary.Services.DataRetrieval.Lookups;
using Atlas.MultipleAlleleCodeDictionary.ExternalInterface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas.HlaMetadataDictionary.Services.DataRetrieval
{
    internal interface IHlaSearchingMetadataService<THlaMetadata> where THlaMetadata : IHlaMetadata
    {
        Task<THlaMetadata> GetHlaMetadata(Locus locus, string hlaName, string hlaNomenclatureVersion);
    }

    /// <summary>
    /// Common functionality used when querying a HLA 'searching' 
    /// (i.e., matching or scoring) lookup repository.
    /// </summary>
    internal abstract class HlaSearchingMetadataServiceBase<THlaMetadata> :
        MetadataServiceBase<THlaMetadata>,
        IHlaSearchingMetadataService<THlaMetadata>
        where THlaMetadata : IHlaMetadata
    {
        protected readonly IHlaCategorisationService HlaCategorisationService;

        private readonly IHlaMetadataRepository hlaMetadataRepository;
        private readonly IAlleleNamesMetadataService alleleNamesMetadataService;
        private readonly IAlleleStringSplitterService alleleSplitter;
        private readonly IMacDictionary macDictionary;
        private readonly IAlleleGroupMetadataService alleleGroupMetadataService;

        protected HlaSearchingMetadataServiceBase(
            IHlaMetadataRepository hlaMetadataRepository,
            IAlleleNamesMetadataService alleleNamesMetadataService,
            IHlaCategorisationService hlaCategorisationService,
            IAlleleStringSplitterService alleleSplitter,
            IMacDictionary macDictionary,
            IAlleleGroupMetadataService alleleGroupMetadataService)
        {
            this.hlaMetadataRepository = hlaMetadataRepository;
            this.alleleNamesMetadataService = alleleNamesMetadataService;
            HlaCategorisationService = hlaCategorisationService;
            this.alleleSplitter = alleleSplitter;
            this.macDictionary = macDictionary;
            this.alleleGroupMetadataService = alleleGroupMetadataService;
        }

        public async Task<THlaMetadata> GetHlaMetadata(Locus locus, string hlaName, string hlaNomenclatureVersion)
        {
            return await GetMetadata(locus, hlaName, hlaNomenclatureVersion);
        }

        protected override bool LookupNameIsValid(string lookupName)
        {
            return !string.IsNullOrEmpty(lookupName);
        }

        protected override async Task<THlaMetadata> PerformLookup(Locus locus, string lookupName, string hlaNomenclatureVersion)
        {
            var dictionaryLookup = GetHlaLookup(lookupName);
            var metadataRows = await dictionaryLookup.PerformLookupAsync(locus, lookupName, hlaNomenclatureVersion);
            var metadata = ConvertMetadataRowsToMetadata(metadataRows).ToList();

            return ConsolidateHlaMetadata(locus, lookupName, metadata);
        }

        private HlaLookupBase GetHlaLookup(string lookupName)
        {
            var hlaTypingCategory = HlaCategorisationService.GetHlaTypingCategory(lookupName);

            return HlaLookupFactory
                .GetLookupByHlaTypingCategory(
                    hlaTypingCategory,
                    hlaMetadataRepository,
                    alleleNamesMetadataService,
                    alleleSplitter,
                    macDictionary,
                    alleleGroupMetadataService);
        }

        protected abstract IEnumerable<THlaMetadata> ConvertMetadataRowsToMetadata(IEnumerable<HlaMetadataTableRow> rows);

        protected abstract THlaMetadata ConsolidateHlaMetadata(Locus locus, string lookupName, List<THlaMetadata> metadata);
    }
}