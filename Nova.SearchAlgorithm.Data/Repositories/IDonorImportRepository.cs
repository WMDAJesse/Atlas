﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Nova.SearchAlgorithm.Data.Models;

namespace Nova.SearchAlgorithm.Data.Repositories
{
    public interface IDonorImportRepository
    {
        /// <summary>
        /// Insert a donor into the database.
        /// This does _not_ refresh or create the hla matches.
        /// </summary>
        Task InsertDonor(RawInputDonor donor);

        /// <summary>
        /// Insert a donor into the database.
        /// This does _not_ refresh or create the hla matches.
        /// </summary>
        Task InsertBatchOfDonors(IEnumerable<RawInputDonor> donors);

        /// <summary>
        /// If a donor with the given DonorId already exists, update the HLA and refresh the pre-processed matching groups.
        /// Otherwise, insert the donor and generate the matching groups.
        /// </summary>
        Task AddOrUpdateDonorWithHla(InputDonor donor);

        /// <summary>
        /// Performs one time set up before the Refresh Hla function is run
        /// e.g. generating a new data table in the azure table storage implementation
        /// </summary>
        void SetupForHlaRefresh();
        
        /// <summary>
        /// Refreshes the pre-processed matching groups for a single donor, for example if the HLA matching dictionary has been updated.
        /// </summary>
        Task RefreshMatchingGroupsForExistingDonor(InputDonor donor);
        
        Task RefreshMatchingGroupsForExistingDonorBatch(IEnumerable<InputDonor> donors);

        void InsertPGroups(IEnumerable<string> pGroups);
    }
}