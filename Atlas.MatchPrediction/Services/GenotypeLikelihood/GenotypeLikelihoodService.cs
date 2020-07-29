using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Common.GeneticData;
using Atlas.Common.GeneticData.PhenotypeInfo;
using Atlas.Common.Utils.Extensions;
using Atlas.MatchPrediction.Config;
using Atlas.MatchPrediction.Data.Models;
using Atlas.MatchPrediction.Models;
using Atlas.MatchPrediction.Services.HaplotypeFrequencies;
using HaplotypeFrequencySet = Atlas.MatchPrediction.ExternalInterface.Models.HaplotypeFrequencySet.HaplotypeFrequencySet;
using HaplotypeHla = Atlas.Common.GeneticData.PhenotypeInfo.LociInfo<string>;

namespace Atlas.MatchPrediction.Services.GenotypeLikelihood
{
    public interface IGenotypeLikelihoodService
    {
        public Task<decimal> CalculateLikelihood(PhenotypeInfo<string> genotype, HaplotypeFrequencySet frequencySet, ISet<Locus> allowedLoci);
    }

    internal class GenotypeLikelihoodService : IGenotypeLikelihoodService
    {
        private readonly IUnambiguousGenotypeExpander unambiguousGenotypeExpander;
        private readonly IGenotypeLikelihoodCalculator likelihoodCalculator;
        private readonly IHaplotypeFrequencyService haplotypeFrequencyService;

        public GenotypeLikelihoodService(
            IUnambiguousGenotypeExpander unambiguousGenotypeExpander,
            IGenotypeLikelihoodCalculator likelihoodCalculator,
            IHaplotypeFrequencyService haplotypeFrequencyService
        )
        {
            this.unambiguousGenotypeExpander = unambiguousGenotypeExpander;
            this.likelihoodCalculator = likelihoodCalculator;
            this.haplotypeFrequencyService = haplotypeFrequencyService;
        }

        public async Task<decimal> CalculateLikelihood(PhenotypeInfo<string> genotype, HaplotypeFrequencySet frequencySet, ISet<Locus> allowedLoci)
        {
            var expandedGenotype = unambiguousGenotypeExpander.ExpandGenotype(genotype, allowedLoci);
            var haplotypesWithFrequencies = await haplotypeFrequencyService.GetAllHaplotypeFrequencies(frequencySet.Id);

            UpdateFrequenciesForDiplotype(haplotypesWithFrequencies, expandedGenotype.Diplotypes, allowedLoci);
            return likelihoodCalculator.CalculateLikelihood(expandedGenotype);
        }

        private static void UpdateFrequenciesForDiplotype(
            Dictionary<HaplotypeHla, HaplotypeFrequency> haplotypesWithFrequencies,
            IEnumerable<Diplotype> diplotypes,
            ISet<Locus> allowedLoci)
        {
            // Unrepresented haplotypes are assigned default value for decimal, 0 - which is what we want here.
            foreach (var diplotype in diplotypes)
            {
                diplotype.Item1.Frequency = GetFrequencyForHla(haplotypesWithFrequencies, diplotype.Item1.Hla, allowedLoci);
                diplotype.Item2.Frequency = GetFrequencyForHla(haplotypesWithFrequencies, diplotype.Item2.Hla, allowedLoci);
            }
        }

        private static decimal GetFrequencyForHla(
            Dictionary<HaplotypeHla, HaplotypeFrequency> haplotypesWithFrequencies,
            HaplotypeHla hla,
            ISet<Locus> allowedLoci)
        {
            if (!haplotypesWithFrequencies.TryGetValue(hla, out var hf))
            {
                hf = new HaplotypeFrequency();
                if (!allowedLoci.SetEquals(LocusSettings.MatchPredictionLoci))
                {
                    //This can get called in parallel (see MPS.CalculateGenotypeLikelihoods) so this .Where() would get "Collection was modified" errors.
                    var isolatedHaplotypesWithFrequencies = haplotypesWithFrequencies.ToList();
                    hf.Frequency = isolatedHaplotypesWithFrequencies
                        .Where(kvp => kvp.Key.EqualsAtLoci(hla, allowedLoci))
                        .Select(kvp => kvp.Value.Frequency)
                        .DefaultIfEmpty(0m)
                        .SumDecimals();

                    haplotypesWithFrequencies.Add(hla, hf);
                }
            }

            return hf?.Frequency ?? 0;
        }
    }
}