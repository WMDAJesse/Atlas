﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.MatchPrediction.Test.Verification.Data.Repositories
{
    public interface IProcessedSearchResultsRepository<in TDbModel>
    {
        Task DeleteResults(int searchRequestRecordId);
        Task BulkInsertResults(IReadOnlyCollection<TDbModel> results);
    }
}
