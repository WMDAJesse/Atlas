﻿using System.Collections.Generic;
using System.Linq;
using Nova.SearchAlgorithm.Client.Models;
using Nova.SearchAlgorithm.Repositories.Hla;
using Nova.SearchAlgorithm.Data.Repositories;
using Nova.SearchAlgorithm.Data.Models;

namespace Nova.SearchAlgorithm.Services
{
    public interface ISearchService
    {
        IEnumerable<PotentialMatch> Search(SearchRequest searchRequest);
    }

    public class SearchService : ISearchService
    {
        private readonly IDonorMatchRepository donorRepository;
        private readonly IHlaRepository hlaRepository;

        public SearchService(IDonorMatchRepository donorRepository, IHlaRepository hlaRepository)
        {
            this.donorRepository = donorRepository;
            this.hlaRepository = hlaRepository;
        }

        public IEnumerable<PotentialMatch> Search(SearchRequest searchRequest)
        {
            DonorMatchCriteria criteria = new DonorMatchCriteria
            {
                SearchType = searchRequest.SearchType,
                RegistriesToSearch = searchRequest.RegistriesToSearch,
                DonorMismatchCount = searchRequest.MatchCriteria.DonorMismatchCount,
                LocusMismatchA = MapMismatchToMatchCriteria(Locus.A, searchRequest.MatchCriteria.LocusMismatchA),
                LocusMismatchB = MapMismatchToMatchCriteria(Locus.B, searchRequest.MatchCriteria.LocusMismatchB),
                LocusMismatchC = MapMismatchToMatchCriteria(Locus.C, searchRequest.MatchCriteria.LocusMismatchC),
                LocusMismatchDRB1 = MapMismatchToMatchCriteria(Locus.Dqb1, searchRequest.MatchCriteria.LocusMismatchDRB1),
                LocusMismatchDQB1 = MapMismatchToMatchCriteria(Locus.Drb1, searchRequest.MatchCriteria.LocusMismatchDQB1),
            };

            return donorRepository.Search(criteria);
        }

        private DonorLocusMatchCriteria MapMismatchToMatchCriteria(Locus locus, LocusMismatchCriteria mismatch)
        {
            if (mismatch == null)
            {
                return null;
            }

            var hla1 = hlaRepository.RetrieveHlaMatches(locus, mismatch.SearchHla1);
            var hla2 = hlaRepository.RetrieveHlaMatches(locus, mismatch.SearchHla2);

            return new DonorLocusMatchCriteria
            {
                MismatchCount = mismatch.MismatchCount,
                HlaNamesToMatchInPositionOne = hla1.PGroups,
                HlaNamesToMatchInPositionTwo = hla2.PGroups,
            };
        }
    }
}