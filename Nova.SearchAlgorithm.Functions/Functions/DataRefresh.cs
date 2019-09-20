using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Nova.SearchAlgorithm.Data.Persistent.Models;
using Nova.SearchAlgorithm.Services.DataRefresh;

namespace Nova.SearchAlgorithm.Functions.Functions
{
    public class DataRefresh
    {
        private readonly IDataRefreshOrchestrator dataRefreshOrchestrator;
        private readonly IDataRefreshCleanupService dataRefreshCleanupService;

        public DataRefresh(IDataRefreshOrchestrator dataRefreshOrchestrator, IDataRefreshCleanupService dataRefreshCleanupService)
        {
            this.dataRefreshOrchestrator = dataRefreshOrchestrator;
            this.dataRefreshCleanupService = dataRefreshCleanupService;
        }

        /// <summary>
        /// Runs a full data refresh, regardless of whether the existing database is using the latest version of the hla database
        /// </summary>
        [FunctionName("ForceDataRefresh")]
        public async Task ForceDataRefresh([HttpTrigger] HttpRequest httpRequest)
        {
            await dataRefreshOrchestrator.RefreshDataIfNecessary(shouldForceRefresh: true);
        }

        /// <summary>
        /// Runs a full data refresh, if necessary
        /// </summary>
        [FunctionName("RunDataRefreshManual")]
        public async Task RunDataRefreshManual([HttpTrigger] HttpRequest httpRequest)
        {
            await dataRefreshOrchestrator.RefreshDataIfNecessary();
        }

        /// <summary>
        /// Runs a full data refresh, without clearing out old data - can be used when an in progress job failed near the end of the job,
        /// To avoid importing / processing donors more than necessary
        /// </summary>
        [FunctionName("ContinueDataRefreshManual")]
        public async Task ContinueDataRefreshManual([HttpTrigger] HttpRequest httpRequest)
        {
            await dataRefreshOrchestrator.RefreshDataIfNecessary(isContinuedRefresh: true);
        }

        /// <summary>
        /// Runs a full data refresh, if necessary
        /// </summary>
        [FunctionName("RunDataRefresh")]
        public async Task RunDataRefresh([TimerTrigger("%DataRefresh.CronTab%")] TimerInfo timerInfo)
        {
            await dataRefreshOrchestrator.RefreshDataIfNecessary();
        }

        /// <summary>
        /// Manually triggers cleanup after the data refresh.
        /// This clean up covers scaling down the database that was scaled up for the refresh, and re-enabling donor update functions.
        /// Clean up should have been run if the job completed, whether successfully or not.
        /// The only time this should be triggered is if the server running the data refresh was restarted while the job was in progress, causing it to skip tear-down.
        /// </summary>
        [FunctionName("RunDataRefreshCleanup")]
        public async Task RunDataRefreshCleanup([HttpTrigger] HttpRequest httpRequest)
        {
            await dataRefreshCleanupService.RunDataRefreshCleanup();
        }
    }
}