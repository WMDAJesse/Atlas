﻿using AutoMapper;
using Microsoft.WindowsAzure.Storage.Table;
using Nova.SearchAlgorithm.Models;
using Nova.SearchAlgorithm.Client.Models;
using Nova.SearchAlgorithm.Repositories.Donors.AzureStorage;
using System.Collections.Generic;
using System.Linq;


namespace Nova.SearchAlgorithm.Repositories.Donors
{
    public interface IDonorRepository
    {
        SearchableDonor GetDonor(int donorId);
        IEnumerable<HlaMatchTableEntity> MatchDonors(SearchCriteria criteria);
        void InsertDonor(ImportDonor donor);
    }

    static class TableQueryExtensions
    {
        public static TableQuery<TElement> AndWhere<TElement>(this TableQuery<TElement> @this, string filter)
        {
            @this.FilterString = TableQuery.CombineFilters(@this.FilterString, TableOperators.And, filter);
            return @this;
        }

        public static TableQuery<TElement> OrWhere<TElement>(this TableQuery<TElement> @this, string filter)
        {
            @this.FilterString = @this.FilterString == null ? filter : TableQuery.CombineFilters(@this.FilterString, TableOperators.Or, filter);
            return @this;
        }

        public static TableQuery<TElement> NotWhere<TElement>(this TableQuery<TElement> @this, string filter)
        {
            @this.FilterString = TableQuery.CombineFilters(@this.FilterString, TableOperators.Not, filter);
            return @this;
        }
    }

    public class DonorRepository : IDonorRepository
    {
        private const string TableReference = "Donors";
        private readonly CloudTable donorTable;
        private readonly IMapper mapper;

        public DonorRepository(IMapper mapper, ICloudTableFactory cloudTableFactory)
        {
            donorTable = cloudTableFactory.GetTable(TableReference);
            this.mapper = mapper;
        }
        
        public IEnumerable<HlaMatchTableEntity> MatchDonors(SearchCriteria criteria)
        {
            // TODO:NOVA-931 extend to other loci
            IEnumerable<string> hlaNamesToMatchInLocusA = criteria.LocusMatchCriteria.A_1.MatchingProteinGroups
                .Union(criteria.LocusMatchCriteria.A_1.MatchingSerologyNames)
                .Union(criteria.LocusMatchCriteria.A_2.MatchingProteinGroups)
                .Union(criteria.LocusMatchCriteria.A_2.MatchingSerologyNames);
            var matchesQuery = new TableQuery<HlaMatchTableEntity>();
            foreach (string name in hlaNamesToMatchInLocusA)
            {
                matchesQuery = matchesQuery.OrWhere(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, HlaMatchTableEntity.GeneratePartitionKey("A", name)));
            }

            return donorTable.ExecuteQuery(matchesQuery);
        }

        public SearchableDonor GetDonor(int donorId)
        {
            var donorQuery = new TableQuery<DonorTableEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, donorId.ToString()));
            return donorTable.ExecuteQuery(donorQuery).Select(dte => dte.ToSearchableDonor(mapper)).FirstOrDefault();
        }

        public void InsertDonor(ImportDonor donor)
        {
            var insertDonor = TableOperation.Insert(donor.ToTableEntity(mapper));
            donorTable.Execute(insertDonor);

            donor.MatchingHla.Each((locusName, position, matchingHla) => InsertLocusMatch(locusName, position, matchingHla, donor.DonorId));

            // TODO:NOVA-929 if this method stays, sort out a return value
        }

        public void UpdateDonorWithNewHla(ImportDonor donor)
        {
            // TODO:NOVA-929 implment for the (daily?) donor update process
            // It should include removing any HlaMatchTableEntities which no longer apply, by searching for them by donor_id (row key)
        }

        private void InsertLocusMatch(string locusName, int typePosition, MatchingHla matchingHla, int donorId)
        {
            foreach (string matchName in matchingHla.MatchingProteinGroups.Union(matchingHla.MatchingSerologyNames))
            {
                var insertMatch = TableOperation.Insert(new HlaMatchTableEntity(locusName, typePosition, matchName, donorId));
                donorTable.Execute(insertMatch);
            }
        }
    }
}
