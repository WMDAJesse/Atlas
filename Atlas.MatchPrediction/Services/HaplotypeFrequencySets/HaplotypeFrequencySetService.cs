﻿using System;
using System.Threading.Tasks;
using Atlas.MatchPrediction.Data.Models;
using Atlas.MatchPrediction.Data.Repositories;
using Atlas.MatchPrediction.Models;

namespace Atlas.MatchPrediction.Services.HaplotypeFrequencySets
{
    public interface IHaplotypeFrequencySetService
    {
        Task<HaplotypeFrequencySet> GetHaplotypeFrequencySetId(IndividualPopulationData donorInfo, IndividualPopulationData patientInfo);
    }
    
    internal class HaplotypeFrequencySetService : IHaplotypeFrequencySetService
    {
        private readonly IHaplotypeFrequencySetRepository repository;

        public HaplotypeFrequencySetService(IHaplotypeFrequencySetRepository repository)
        {
            this.repository = repository;
        }
        
        public async Task<HaplotypeFrequencySet> GetHaplotypeFrequencySetId(IndividualPopulationData donorInfo, IndividualPopulationData patientInfo)
        {
            return await repository.GetActiveSet(donorInfo.RegistryId, donorInfo.EthnicityId);
        }
    }
}