﻿using Dapper;
using Nova.SearchAlgorithm.Common.Config;
using Nova.SearchAlgorithm.Common.Models;
using Nova.SearchAlgorithm.Common.Models.Matching;
using Nova.SearchAlgorithm.Data.Helpers;
using Nova.SearchAlgorithm.Data.Models;
using Nova.SearchAlgorithm.Data.Models.DonorInfo;
using Nova.SearchAlgorithm.Data.Models.Entities;
using Nova.SearchAlgorithm.Data.Services;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.SearchAlgorithm.Data.Repositories.DonorRetrieval
{
    public class DonorInspectionRepository : Repository, IDonorInspectionRepository
    {
        public DonorInspectionRepository(IConnectionStringProvider connectionStringProvider) : base(connectionStringProvider)
        {
        }

        public async Task<InputDonor> GetDonor(int donorId)
        {
            using (var conn = new SqlConnection(ConnectionStringProvider.GetConnectionString()))
            {
                var donor = await conn.QuerySingleOrDefaultAsync<Donor>($"SELECT * FROM Donors WHERE DonorId = {donorId}", commandTimeout: 300);
                return donor?.ToInputDonor();
            }
        }

        /// <summary>
        /// Fetches all PGroups for a batch of donors from the MatchingHlaAt$Locus tables
        /// </summary>
        public async Task<IEnumerable<DonorIdWithPGroupNames>> GetPGroupsForDonors(IEnumerable<int> donorIds)
        {
            donorIds = donorIds.ToList();
            if (!donorIds.Any())
            {
                return new List<DonorIdWithPGroupNames>();
            }

            var results = donorIds
                .Select(id => new DonorIdWithPGroupNames {DonorId = id, PGroupNames = new PhenotypeInfo<IEnumerable<string>>()})
                .ToList();
            using (var conn = new SqlConnection(ConnectionStringProvider.GetConnectionString()))
            {
                // TODO NOVA-1427: Do not fetch PGroups for loci that have already been matched at the DB level
                foreach (var locus in LocusSettings.MatchingOnlyLoci)
                {
                    var sql = $@"
SELECT m.DonorId, m.TypePosition, p.Name as PGroupName FROM {MatchingTableNameHelper.MatchingTableName(locus)} m
JOIN PGroupNames p 
ON m.PGroup_Id = p.Id
INNER JOIN (
    SELECT '{donorIds.FirstOrDefault()}' AS Id
    UNION ALL SELECT '{string.Join("' UNION ALL SELECT '", donorIds.Skip(1))}'
)
AS DonorIds 
ON m.DonorId = DonorIds.Id
";
                    var pGroups = await conn.QueryAsync<DonorMatchWithName>(sql, commandTimeout: 300);
                    foreach (var donorGroups in pGroups.GroupBy(p => p.DonorId))
                    {
                        foreach (var pGroupGroup in donorGroups.GroupBy(p => (TypePosition) p.TypePosition))
                        {
                            var donorResult = results.Single(r => r.DonorId == donorGroups.Key);
                            donorResult.PGroupNames.SetAtPosition(locus, pGroupGroup.Key, pGroupGroup.Select(p => p.PGroupName));
                        }
                    }
                }
            }

            return results;
        }

        public async Task<Dictionary<int, InputDonor>> GetDonors(IEnumerable<int> donorIds)
        {
            donorIds = donorIds.ToList();
            if (!donorIds.Any())
            {
                return new Dictionary<int, InputDonor>();
            }

            using (var conn = new SqlConnection(ConnectionStringProvider.GetConnectionString()))
            {
                var sql = $@"
SELECT * FROM Donors 
INNER JOIN (
    SELECT '{donorIds.FirstOrDefault()}' AS Id
    UNION ALL SELECT '{string.Join("' UNION ALL SELECT '", donorIds.Skip(1))}'
)
AS DonorIds 
ON DonorId = DonorIds.Id
";
                var donors = await conn.QueryAsync<Donor>(sql, commandTimeout: 300);
                return donors.Select(d => d.ToInputDonor()).ToDictionary(d => d.DonorId, d => d);
            }
        }
    }
}