﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nova.SearchAlgorithm.Common.Models;
using Nova.SearchAlgorithm.Data.Models;
using Nova.SearchAlgorithm.Data.Repositories;

namespace Nova.SearchAlgorithm.Repositories.Donors
{
    public class CloudStorageDonorSearchRepository : IDonorSearchRepository, IDonorImportRepository, IDonorInspectionRepository
    {
        private readonly IDonorDocumentStorage donorBlobRepository;

        public CloudStorageDonorSearchRepository(IDonorDocumentStorage donorBlobRepository)
        {
            this.donorBlobRepository = donorBlobRepository;
        }

        public Task<int> HighestDonorId()
        {
            return Task.FromResult(donorBlobRepository.HighestDonorId());
        }

        public IEnumerable<PotentialSearchResult> Search(DonorMatchCriteria matchRequest)
        {
            var matchesAtA = FindMatchesAtLocus(matchRequest.SearchType, matchRequest.RegistriesToSearch, Locus.A, matchRequest.LocusMismatchA);
            var matchesAtB = FindMatchesAtLocus(matchRequest.SearchType, matchRequest.RegistriesToSearch, Locus.B, matchRequest.LocusMismatchB);
            var matchesAtDrb1 = FindMatchesAtLocus(matchRequest.SearchType, matchRequest.RegistriesToSearch, Locus.Drb1, matchRequest.LocusMismatchDRB1);

            var matches = matchesAtA.Union(matchesAtB).Union(matchesAtDrb1)
                .GroupBy(m => m.Key)
                .Select(g => new PotentialSearchResult
                {
                    // TODO: can this extra query per donor be sped up?
                    Donor = GetDonor(g.Key),
                    TotalMatchCount = g.Sum(m => m.Value.MatchCount ?? 0),
                    MatchDetailsAtLocusA = matchesAtA.ContainsKey(g.Key) ? matchesAtA[g.Key] : new LocusMatchDetails { MatchCount = 0 },
                    MatchDetailsAtLocusB = matchesAtB.ContainsKey(g.Key) ? matchesAtB[g.Key] : new LocusMatchDetails { MatchCount = 0 },
                    MatchDetailsAtLocusDrb1 = matchesAtDrb1.ContainsKey(g.Key) ? matchesAtDrb1[g.Key] : new LocusMatchDetails { MatchCount = 0 },
                })
                .Where(m => m.TotalMatchCount >= 6 - matchRequest.DonorMismatchCount)
                .Where(m => m.MatchDetailsAtLocusA.MatchCount >= 2 - matchRequest.LocusMismatchA.MismatchCount)
                .Where(m => m.MatchDetailsAtLocusB.MatchCount >= 2 - matchRequest.LocusMismatchB.MismatchCount)
                .Where(m => m.MatchDetailsAtLocusDrb1.MatchCount >= 2 - matchRequest.LocusMismatchDRB1.MismatchCount);
            
            return matches;
        }

        private async Task<IDictionary<int, LocusMatchDetails>> FindMatchesAtLocus(DonorType searchType, IEnumerable<RegistryCode> registriesToSearch, Locus locus, DonorLocusMatchCriteria criteria)
        {
            LocusSearchCriteria repoCriteria = new LocusSearchCriteria
            {
                SearchType = searchType,
                Registries = registriesToSearch,
                HlaNamesToMatchInPositionOne = criteria.HlaNamesToMatchInPositionOne,
                HlaNamesToMatchInPositionTwo = criteria.HlaNamesToMatchInPositionTwo,
            };

            var matches = (await donorBlobRepository.GetDonorMatchesAtLocus(locus, repoCriteria))
                .GroupBy(m => m.DonorId)
                .ToDictionary(g => g.Key, LocusMatchFromGroup);

            return matches;
        }

        private bool DirectMatch(IEnumerable<PotentialHlaMatchRelation> matches)
        {
            return matches.Where(m => m.SearchTypePosition == TypePositions.One && m.MatchingTypePositions.HasFlag(TypePositions.One)).Any()
                && matches.Where(m => m.SearchTypePosition == TypePositions.Two && m.MatchingTypePositions.HasFlag(TypePositions.Two)).Any();
        }

        private bool CrossMatch(IEnumerable<PotentialHlaMatchRelation> matches)
        {
            return matches.Where(m => m.SearchTypePosition == TypePositions.One && m.MatchingTypePositions.HasFlag(TypePositions.Two)).Any()
                && matches.Where(m => m.SearchTypePosition == TypePositions.Two && m.MatchingTypePositions.HasFlag(TypePositions.One)).Any();
        }

        private LocusMatchDetails LocusMatchFromGroup(IGrouping<int, PotentialHlaMatchRelation> group)
        {
            return new LocusMatchDetails
            {
                MatchCount = DirectMatch(group) || CrossMatch(group) ? 2 : 1
            };
        }

        public DonorResult GetDonor(int donorId)
        {
            return donorBlobRepository.GetDonor(donorId).Wait();
        }

        public void AddOrUpdateDonor(InputDonor donor)
        {
            donorBlobRepository.InsertDonor(donor);
            return Task.CompletedTask;
        }

        // TODO:NOVA-937 This will be too many donors
        // Can we stream them in batches with IEnumerable?
        public IEnumerable<DonorResult> AllDonors()
        {
            return donorBlobRepository.AllDonors();
        }

        public Task RefreshMatchingGroupsForExistingDonor(InputDonor donor)
        {
            donorBlobRepository.UpdateDonorWithNewHla(donor);
            return Task.CompletedTask;
        }
    }
}
