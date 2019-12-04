﻿using Dapper;
using Nova.SearchAlgorithm.Data.Models;
using Nova.SearchAlgorithm.Data.Models.Entities;
using Nova.SearchAlgorithm.Data.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nova.SearchAlgorithm.Data.Repositories
{
    public interface IDonorManagementLogRepository
    {
        Task<IEnumerable<DonorManagementLog>> GetDonorManagementLogBatch(IEnumerable<int> donorIds);
        Task CreateOrUpdateDonorManagementLogBatch(IEnumerable<DonorManagementInfo> donorManagementInfos);
    }

    public class DonorManagementLogRepository : Repository, IDonorManagementLogRepository
    {
        private const string LogTableName = "DonorManagementLogs";
        private const string DonorIdColumnName = "DonorId";
        private const string SequenceNumberColumnName = "SequenceNumberOfLastUpdate";
        private const string UpdateDateTimeColumnName = "LastUpdateDateTime";

        public DonorManagementLogRepository(IConnectionStringProvider connectionStringProvider) : base(connectionStringProvider)
        {
        }

        public async Task<IEnumerable<DonorManagementLog>> GetDonorManagementLogBatch(IEnumerable<int> donorIds)
        {
            var sql = $@"
                SELECT * FROM {LogTableName}
                WHERE {DonorIdColumnName} IN ({string.Join(",", donorIds)})
                ";

            using (var conn = new SqlConnection(ConnectionStringProvider.GetConnectionString()))
            {
                return await conn.QueryAsync<DonorManagementLog>(sql, commandTimeout: 300);
            }
        }

        public async Task CreateOrUpdateDonorManagementLogBatch(IEnumerable<DonorManagementInfo> donorManagementInfos)
        {
            var infos = donorManagementInfos.ToList();

            if (!infos.Any())
            {
                return;
            }

            var donorIdsWithLogs = (await GetDonorIdsWithExistingLogs(infos.Select(i => i.DonorId))).ToList();

            var logsToUpdate = infos.Where(i => donorIdsWithLogs.Contains(i.DonorId));
            var logsToCreate = infos.Where(i => !donorIdsWithLogs.Contains(i.DonorId));

            await UpdateLogBatch(logsToUpdate);
            await CreateLogBatch(logsToCreate);
        }

        private async Task<IEnumerable<int>> GetDonorIdsWithExistingLogs(IEnumerable<int> donorIdsToCheck)
        {
            var existingLogs = await GetDonorManagementLogBatch(donorIdsToCheck);

            return existingLogs.Select(l => l.DonorId);
        }

        private async Task UpdateLogBatch(IEnumerable<DonorManagementInfo> donorManagementInfos)
        {
            var infos = donorManagementInfos.ToList();

            if (!infos.Any())
            {
                return;
            }

            using (var conn = new SqlConnection(ConnectionStringProvider.GetConnectionString()))
            {
                // This UNION ALL based strategy seems sufficiently performant when bulk updating 100s of rows
                // If row count increases to the 1000s, it may be better to use a temp table instead
                var infosSelectStatement = BuildUnionAllSelectStatement(infos);
                var dateTimeNow = DateTimeOffset.UtcNow.ToString("u", CultureInfo.InvariantCulture);
                var sql = $@"
                        UPDATE {LogTableName} 
                        SET 
                            {SequenceNumberColumnName} = infos.{SequenceNumberColumnName},
                            {UpdateDateTimeColumnName} = '{dateTimeNow}'
                        FROM {LogTableName} logs
                        JOIN ({infosSelectStatement}) infos
                        ON logs.{DonorIdColumnName} = infos.{DonorIdColumnName}
                        ";

                await conn.ExecuteAsync(sql, commandTimeout: 600);
            }
        }

        private static string BuildUnionAllSelectStatement(IEnumerable<DonorManagementInfo> donorManagementInfos)
        {
            var selectStatements = donorManagementInfos
                .Select(i => $"SELECT {i.DonorId} AS {DonorIdColumnName}, {i.UpdateSequenceNumber} AS {SequenceNumberColumnName}")
                .ToList();

            if (!selectStatements.Any())
            {
                return string.Empty;
            }

            var builder = new StringBuilder(selectStatements.First() + Environment.NewLine);

            foreach (var selectStatement in selectStatements.Skip(1))
            {
                builder.AppendLine("UNION ALL");
                builder.AppendLine(selectStatement);
            }

            return builder.ToString();
        }

        private async Task CreateLogBatch(IEnumerable<DonorManagementInfo> donorManagementInfos)
        {
            var infos = donorManagementInfos.ToList();

            if (!infos.Any())
            {
                return;
            }

            var dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add(DonorIdColumnName);
            dt.Columns.Add(SequenceNumberColumnName);
            dt.Columns.Add(UpdateDateTimeColumnName);

            var dateTimeNow = DateTimeOffset.UtcNow;

            foreach (var info in infos)
            {
                dt.Rows.Add(0,
                    info.DonorId,
                    info.UpdateSequenceNumber,
                    dateTimeNow
                    );
            }

            using (var sqlBulk = new SqlBulkCopy(ConnectionStringProvider.GetConnectionString()))
            {
                sqlBulk.BulkCopyTimeout = 600;
                sqlBulk.BatchSize = 1000;
                sqlBulk.DestinationTableName = LogTableName;
                await sqlBulk.WriteToServerAsync(dt);
            }
        }
    }
}
