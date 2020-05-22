﻿using Atlas.MatchPrediction.Data.Context;
using Atlas.MatchPrediction.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas.MatchPrediction.Data.Repositories
{
    public interface IHaplotypeFrequencySetRepository
    {
        Task<HaplotypeFrequencySet> GetActiveSet(string registry, string ethnicity);
        Task DeactivateSet(HaplotypeFrequencySet set);
        Task<HaplotypeFrequencySet> AddSet(HaplotypeFrequencySet set);
        Task ActivateSet(int setId);
    }

    public class HaplotypeFrequencySetRepository : IHaplotypeFrequencySetRepository
    {
        private readonly MatchPredictionContext context;

        public HaplotypeFrequencySetRepository(MatchPredictionContext context)
        {
            this.context = context;
        }

        public async Task<HaplotypeFrequencySet> GetActiveSet(string registry, string ethnicity)
        {
            return await context.HaplotypeFrequencySets
                .Where(set => set.Active == true && set.Registry == registry && set.Ethnicity == ethnicity)
                .SingleOrDefaultAsync();
        }

        public async Task DeactivateSet(HaplotypeFrequencySet set)
        {
            set.Active = false;
            context.HaplotypeFrequencySets.Update(set);
            await context.SaveChangesAsync();
        }

        public async Task<HaplotypeFrequencySet> AddSet(HaplotypeFrequencySet set)
        {
            await context.HaplotypeFrequencySets.AddAsync(set);
            await context.SaveChangesAsync();
            return set;
        }

        // TODO: Integration tests for this
        public async Task ActivateSet(int setId)
        {
            var set = await context.HaplotypeFrequencySets.SingleAsync(s => s.Id == setId);
            set.Active = true;
            var otherMatchingSets = context.HaplotypeFrequencySets.Where(s => s.Ethnicity == set.Ethnicity && s.Registry == set.Registry);
            foreach (var otherMatchingSet in otherMatchingSets)
            {
                otherMatchingSet.Active = false;
            }

            await context.SaveChangesAsync();
        }
    }
}