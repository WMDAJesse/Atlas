﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Common.ApplicationInsights;
using Atlas.Common.ApplicationInsights.Timing;
using Atlas.HlaMetadataDictionary.ExternalInterface;
using Atlas.MatchPrediction.ApplicationInsights;
using Atlas.MatchPrediction.ExternalInterface.Models;
using TypedGenotype = Atlas.Common.GeneticData.PhenotypeInfo.PhenotypeInfo<Atlas.MatchPrediction.ExternalInterface.Models.HlaAtKnownTypingCategory>;
using StringGenotype = Atlas.Common.GeneticData.PhenotypeInfo.PhenotypeInfo<string>;

namespace Atlas.MatchPrediction.Services.MatchProbability
{
    internal interface IGenotypeConverter
    {
        Task<ICollection<GenotypeAtDesiredResolutions>> ConvertGenotypes(
            ISet<TypedGenotype> genotypes,
            string subjectLogDescription,
            IReadOnlyDictionary<StringGenotype, decimal> genotypeLikelihoods,
            string hlaNomenclatureVersion);
    }

    internal class GenotypeConverter : IGenotypeConverter
    {
        private readonly ILogger logger;
        private readonly IHlaMetadataDictionaryFactory hlaMetadataDictionaryFactory;

        public GenotypeConverter(
            // ReSharper disable once SuggestBaseTypeForParameter
            IMatchPredictionLogger logger,
            IHlaMetadataDictionaryFactory hlaMetadataDictionaryFactory)
        {
            this.logger = logger;
            this.hlaMetadataDictionaryFactory = hlaMetadataDictionaryFactory;
        }
        
        public async Task<ICollection<GenotypeAtDesiredResolutions>> ConvertGenotypes(
            ISet<TypedGenotype> genotypes,
            string subjectLogDescription,
            IReadOnlyDictionary<StringGenotype, decimal> genotypeLikelihoods,
            string hlaNomenclatureVersion)
        {
            var hlaMetadataDictionary = hlaMetadataDictionaryFactory.BuildDictionary(hlaNomenclatureVersion);

            using (logger.RunTimed($"Convert genotypes for match calculation: {subjectLogDescription}", LogLevel.Verbose))
            {
                return (await Task.WhenAll(genotypes.Select(async g => await GenotypeAtDesiredResolutions.FromHaplotypeResolutions(
                    g,
                    hlaMetadataDictionary,
                    genotypeLikelihoods[g.ToHlaNames()]
                )))).ToList();
            }
        }
    }
}
