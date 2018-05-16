﻿using Nova.SearchAlgorithm.MatchingDictionary.Models.Wmda.Filters;
using Nova.SearchAlgorithm.MatchingDictionary.Repositories;
using Nova.SearchAlgorithm.MatchingDictionary.Services.Dictionary;

namespace Nova.SearchAlgorithm.MatchingDictionary.Services
{
    public interface IManageDictionaryService
    {
        void RecreateDictionary();
    }
    public class ManageDictionaryService : IManageDictionaryService
    {
        private readonly IHlaMatchingService matchingService;
        private readonly IMatchedHlaRepository dictionaryRepository;

        public ManageDictionaryService(IHlaMatchingService matchingService, IMatchedHlaRepository dictionaryRepository)
        {
            this.matchingService = matchingService;
            this.dictionaryRepository = dictionaryRepository;
        }

        public void RecreateDictionary()
        {
            var allMatchedHla = matchingService.GetMatchedHla(SerologyFilter.Instance.Filter, MolecularFilter.Instance.Filter);
            var entries = allMatchedHla.ToMatchingDictionaryEntries();
            dictionaryRepository.RecreateDictionaryTable(entries);
        }
    }
}