﻿using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Nova.SearchAlgorithm.Client.Models;
using Nova.SearchAlgorithm.Common.Models;
using Nova.SearchAlgorithm.Common.Models.Matching;
using Nova.SearchAlgorithm.Common.Repositories;
using Nova.SearchAlgorithm.Data.Entity;
using Nova.SearchAlgorithm.Data.Models;
using Nova.SearchAlgorithm.Data.Models.Extensions;
using Nova.SearchAlgorithm.Repositories.Donors;

namespace Nova.SearchAlgorithm.Data.Repositories
{
    public class SqlDonorSearchRepository : IDonorSearchRepository, IDonorImportRepository, IDonorInspectionRepository
    {
        private readonly SearchAlgorithmContext context;

        private readonly string connectionString = ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString;

        private Dictionary<string, PGroupName> pGroupDictionary;

        public SqlDonorSearchRepository(SearchAlgorithmContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<PotentialHlaMatchRelation>> GetDonorMatchesAtLocus(
            Locus locus,
            LocusSearchCriteria criteria,
            MatchingFilteringOptions filteringOptions
        )
        {
            var results = await Task.WhenAll(
                GetAllDonorsForPGroupsAtLocus(
                    locus,
                    criteria.PGroupsToMatchInPositionOne,
                    criteria.SearchType,
                    criteria.Registries,
                    filteringOptions
                ),
                GetAllDonorsForPGroupsAtLocus(
                    locus,
                    criteria.PGroupsToMatchInPositionTwo,
                    criteria.SearchType,
                    criteria.Registries,
                    filteringOptions
                )
            );

            return results[0].Select(r => r.ToPotentialHlaMatchRelation(TypePosition.One, locus))
                .Concat(results[1].Select(r => r.ToPotentialHlaMatchRelation(TypePosition.Two, locus)));
        }

        public async Task<IEnumerable<PotentialHlaMatchRelation>> GetDonorMatchesAtLocusFromDonorSelection(
            Locus locus,
            LocusSearchCriteria criteria,
            IEnumerable<int> donorIds
        )
        {
            donorIds = donorIds.ToList();

            var matchingPGroupResults = await Task.WhenAll(
                GetDonorsForPGroupsAtLocusFromDonorSelection(locus, criteria.PGroupsToMatchInPositionOne, donorIds),
                GetDonorsForPGroupsAtLocusFromDonorSelection(locus, criteria.PGroupsToMatchInPositionTwo, donorIds)
            );

            var untypedDonorIds = await GetUntypedDonorsAtLocus(locus, donorIds);
            var untypedDonorResults = untypedDonorIds.SelectMany(id => new[] {TypePosition.One, TypePosition.Two}.Select(position =>
                new PotentialHlaMatchRelation
                {
                    DonorId = id,
                    Locus = locus,
                    SearchTypePosition = position,
                    MatchingTypePosition = position
                }));

            return matchingPGroupResults[0].Select(r => r.ToPotentialHlaMatchRelation(TypePosition.One, locus))
                .Concat(matchingPGroupResults[1].Select(r => r.ToPotentialHlaMatchRelation(TypePosition.Two, locus)))
                .Concat(untypedDonorResults);
        }

        public Task<int> HighestDonorId()
        {
            return context.Donors.OrderByDescending(d => d.DonorId).Take(1).Select(d => d.DonorId).FirstOrDefaultAsync();
        }

        public IBatchQueryAsync<DonorResult> AllDonors()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var donors = conn.Query<Donor>("SELECT * FROM donors");
                return new SqlDonorBatchQueryAsync(donors);
            }
        }

        public async Task<DonorResult> GetDonor(int donorId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var donor = await conn.QuerySingleOrDefaultAsync<Donor>($"SELECT * FROM Donors WHERE DonorId = {donorId}");
                return donor?.ToDonorResult();
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
            using (var conn = new SqlConnection(connectionString))
            {
                // TODO NOVA-1427: Do not fetch PGroups for loci that have already been matched at the DB level
                foreach (var locus in LocusHelpers.AllLoci().Except(new[] {Locus.Dpb1}))
                {
                    var sql = $@"
SELECT m.DonorId, m.TypePosition, p.Name as PGroupName FROM {MatchingTableName(locus)} m
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

        public async Task<IEnumerable<DonorResult>> GetDonors(IEnumerable<int> donorIds)
        {
            donorIds = donorIds.ToList();
            if (!donorIds.Any())
            {
                return new List<DonorResult>();
            }

            using (var conn = new SqlConnection(connectionString))
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
                return donors.Select(d => d.ToDonorResult());
            }
        }

        public async Task InsertBatchOfDonors(IEnumerable<RawInputDonor> donors)
        {
            var rawInputDonors = donors.ToList();

            if (!rawInputDonors.Any())
            {
                return;
            }

            var dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("DonorId");
            dt.Columns.Add("DonorType");
            dt.Columns.Add("RegistryCode");
            dt.Columns.Add("A_1");
            dt.Columns.Add("A_2");
            dt.Columns.Add("B_1");
            dt.Columns.Add("B_2");
            dt.Columns.Add("C_1");
            dt.Columns.Add("C_2");
            dt.Columns.Add("DPB1_1");
            dt.Columns.Add("DPB1_2");
            dt.Columns.Add("DQB1_1");
            dt.Columns.Add("DQB1_2");
            dt.Columns.Add("DRB1_1");
            dt.Columns.Add("DRB1_2");

            foreach (var donor in rawInputDonors)
            {
                dt.Rows.Add(0,
                    donor.DonorId,
                    (int) donor.DonorType,
                    (int) donor.RegistryCode,
                    donor.HlaNames.A_1, donor.HlaNames.A_2,
                    donor.HlaNames.B_1, donor.HlaNames.B_2,
                    donor.HlaNames.C_1, donor.HlaNames.C_2,
                    donor.HlaNames.DPB1_1, donor.HlaNames.DPB1_2,
                    donor.HlaNames.DQB1_1, donor.HlaNames.DQB1_2,
                    donor.HlaNames.DRB1_1, donor.HlaNames.DRB1_2);
            }

            using (var sqlBulk = new SqlBulkCopy(connectionString))
            {
                sqlBulk.BatchSize = 10000;
                sqlBulk.DestinationTableName = "Donors";
                sqlBulk.WriteToServer(dt);
            }
        }

        public async Task AddOrUpdateDonorWithHla(InputDonor donor)
        {
            var result = await context.Donors.FirstOrDefaultAsync(d => d.DonorId == donor.DonorId);
            if (result == null)
            {
                context.Donors.Add(donor.ToDonorEntity());
            }
            else
            {
                result.CopyRawHlaFrom(donor);
            }

            await RefreshMatchingGroupsForExistingDonorBatch(new List<InputDonor> {donor});

            await context.SaveChangesAsync();
        }

        public async Task RefreshMatchingGroupsForExistingDonorBatch(IEnumerable<InputDonor> inputDonors)
        {
            await Task.WhenAll(LocusHelpers.AllLoci().Select(l => RefreshMatchingGroupsForExistingDonorBatchAtLocus(inputDonors, l)));
        }

        public void InsertPGroups(IEnumerable<string> pGroups)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var existingPGroups = conn.Query<PGroupName>("SELECT * FROM PGroupNames").Select(p => p.Name);

                var dt = new DataTable();
                dt.Columns.Add("Id");
                dt.Columns.Add("Name");

                foreach (var pg in pGroups.Distinct().Except(existingPGroups))
                {
                    dt.Rows.Add(0, pg);
                }

                var transaction = conn.BeginTransaction();
                using (var sqlBulk = new SqlBulkCopy(conn, SqlBulkCopyOptions.TableLock, transaction))
                {
                    sqlBulk.BatchSize = 10000;
                    sqlBulk.DestinationTableName = "PGroupNames";
                    sqlBulk.WriteToServer(dt);
                }

                transaction.Commit();
                conn.Close();
            }

            CachePGroupDictionary();
        }

        public void SetupForHlaRefresh()
        {
            // Do nothing
        }

        private async Task<IEnumerable<int>> GetUntypedDonorsAtLocus(Locus locus, IEnumerable<int> donorIds)
        {
            donorIds = donorIds.ToList();

            var sql = $@"
SELECT DonorId FROM Donors 
INNER JOIN (
    SELECT '{donorIds.FirstOrDefault()}' AS Id
    UNION ALL SELECT '{string.Join("' UNION ALL SELECT '", donorIds.Skip(1))}'
)
AS DonorIds 
ON DonorId = DonorIds.Id 
WHERE {DonorHlaColumnAtLocus(locus, TypePosition.One)} IS NULL
AND {DonorHlaColumnAtLocus(locus, TypePosition.Two)} IS NULL
";

            using (var conn = new SqlConnection(connectionString))
            {
                return await conn.QueryAsync<int>(sql);
            }
        }

        private async Task<IEnumerable<DonorMatch>> GetDonorsForPGroupsAtLocusFromDonorSelection(
            Locus locus,
            IEnumerable<string> pGroups,
            IEnumerable<int> donorIds
        )
        {
            donorIds = donorIds.ToList();
            pGroups = pGroups.ToList();

            var sql = $@"
SELECT InnerDonorId as DonorId, TypePosition FROM {MatchingTableName(locus)} m

RIGHT JOIN (
    SELECT '{donorIds.FirstOrDefault()}' AS InnerDonorId
    UNION ALL SELECT '{string.Join("' UNION ALL SELECT '", donorIds.Skip(1))}'
)
AS InnerDonors 
ON m.DonorId = InnerDonors.InnerDonorId

LEFT JOIN PGroupNames p 
ON m.PGroup_Id = p.Id

INNER JOIN (
    SELECT '{pGroups.FirstOrDefault()}' AS PGroupName
    UNION ALL SELECT '{string.Join("' UNION ALL SELECT '", pGroups.Skip(1))}'
)
AS PGroupNames 
ON (p.Name = PGroupNames.PGroupName)

GROUP BY InnerDonorId, TypePosition";

            using (var conn = new SqlConnection(connectionString))
            {
                return await conn.QueryAsync<DonorMatch>(sql, commandTimeout: 300);
            }
        }

        private async Task<IEnumerable<DonorMatch>> GetAllDonorsForPGroupsAtLocus(
            Locus locus,
            IEnumerable<string> pGroups,
            DonorType donorType,
            IEnumerable<RegistryCode> registryCodes,
            MatchingFilteringOptions filteringOptions
        )
        {
            pGroups = pGroups.ToList();

            var filterQuery = "";

            if (filteringOptions.ShouldFilterOnDonorType || filteringOptions.ShouldFilterOnRegistry)
            {
                var donorTypeClause = filteringOptions.ShouldFilterOnDonorType ? $"AND d.DonorType = {(int) donorType}" : "";
                var registryClause = filteringOptions.ShouldFilterOnRegistry 
                    ? $"AND d.RegistryCode IN ({string.Join(",", registryCodes.Select(id => (int) id))})" 
                    : "";

                filterQuery = $@"
INNER JOIN Donors d
ON m.DonorId = d.DonorId
{donorTypeClause}
{registryClause}
";
            }
            

            var sql = $@"
SELECT m.DonorId, TypePosition FROM {MatchingTableName(locus)} m

{filterQuery}

LEFT JOIN PGroupNames p 
ON m.PGroup_Id = p.Id

INNER JOIN (
    SELECT '{pGroups.FirstOrDefault()}' AS PGroupName
    UNION ALL SELECT '{string.Join("' UNION ALL SELECT '", pGroups.Skip(1))}'
)
AS PGroupNames 
ON (p.Name = PGroupNames.PGroupName)

GROUP BY m.DonorId, TypePosition";

            using (var conn = new SqlConnection(connectionString))
            {
                return await conn.QueryAsync<DonorMatch>(sql, commandTimeout: 300);
            }
        }

        private async Task RefreshMatchingGroupsForExistingDonorBatchAtLocus(IEnumerable<InputDonor> donors, Locus locus)
        {
            if (locus == Locus.Dpb1)
            {
                return;
            }

            var tableName = MatchingTableName(locus);

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var transaction = conn.BeginTransaction();

                var dataTableGenerationTask = Task.Run(() =>
                {
                    var dt = new DataTable();
                    dt.Columns.Add("Id");
                    dt.Columns.Add("DonorId");
                    dt.Columns.Add("TypePosition");
                    dt.Columns.Add("PGroup_Id");

                    foreach (var donor in donors)
                    {
                        donor.MatchingHla.EachPosition((l, p, h) =>
                        {
                            if (h == null || l != locus)
                            {
                                return;
                            }

                            foreach (var pGroup in h.PGroups)
                            {
                                dt.Rows.Add(0, donor.DonorId, (int) p, FindOrCreatePGroup(pGroup));
                            }
                        });
                    }

                    return dt;
                });

                var dataTable = await dataTableGenerationTask;

                using (var sqlBulk = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                {
                    sqlBulk.BatchSize = 10000;
                    sqlBulk.DestinationTableName = tableName;
                    sqlBulk.WriteToServer(dataTable);
                }

                transaction.Commit();
                conn.Close();
            }
        }

        private static string MatchingTableName(Locus locus)
        {
            return "MatchingHlaAt" + locus;
        }

        private static string DonorHlaColumnAtLocus(Locus locus, TypePosition positions)
        {
            var positionString = positions == TypePosition.One ? "1" : "2";
            return $"{locus.ToString().ToUpper()}_{positionString}";
        }

        private void CachePGroupDictionary()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var innerPGroups = conn.Query<PGroupName>("SELECT * FROM PGroupNames");
                pGroupDictionary = innerPGroups.Distinct(new DistinctPGroupNameComparer()).ToDictionary(p => p.Name);
            }
        }

        private int FindOrCreatePGroup(string pGroupName)
        {
            if (pGroupDictionary == null)
            {
                CachePGroupDictionary();
            }

            pGroupDictionary.TryGetValue(pGroupName, out var existing);

            if (existing != null)
            {
                return existing.Id;
            }

            string sql = @"
INSERT INTO PGroupNames (Name) VALUES (@PGroupName);
SELECT CAST(SCOPE_IDENTITY() as int)";

            int newId;

            using (var conn = new SqlConnection(connectionString))
            {
                newId = conn.Query<int>(sql, new {PGroupName = pGroupName}).Single();
            }

            CachePGroupDictionary();
            return newId;
        }
    }

    internal class DistinctPGroupNameComparer : IEqualityComparer<PGroupName>
    {
        public bool Equals(PGroupName x, PGroupName y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(PGroupName obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}