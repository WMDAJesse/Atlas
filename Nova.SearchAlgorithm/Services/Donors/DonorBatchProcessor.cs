﻿using Nova.SearchAlgorithm.ApplicationInsights.DonorProcessing;
using Nova.SearchAlgorithm.Exceptions;
using Nova.SearchAlgorithm.Models;
using Nova.Utils.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.SearchAlgorithm.Services.Donors
{
    public interface IDonorBatchProcessor<TDonor, TResult>
    {
        /// <summary>
        /// Sequentially processes a batch of donors.
        /// </summary>
        /// <param name="donorInfo">Batch of donor info of type TDonor to be processed.</param>
        /// <param name="processDonorInfoFuncAsync">Function to be run on each donor info, that generates an object of type TResult.</param>
        /// <param name="getFailedDonorInfo">Function to select failed donor info.</param>
        /// <param name="failureEventName">Name to use when logging the processing failure event.</param>
        /// <returns>Results from processing of the donor batch.</returns>
        Task<DonorBatchProcessingResult<TResult>> ProcessBatch(
            IEnumerable<TDonor> donorInfo,
            Func<TDonor, Task<TResult>> processDonorInfoFuncAsync,
            Func<TDonor, FailedDonorInfo> getFailedDonorInfo,
            string failureEventName);

        /// <summary>
        /// Processing of the donor batch is performed in parallel.
        /// Caution: do not use if the batch size is large and
        /// there are one or more downstream dependencies on http service clients.
        /// </summary>
        Task<DonorBatchProcessingResult<TResult>> ProcessBatchAsParallel(
            IEnumerable<TDonor> donorInfo,
            Func<TDonor, Task<TResult>> processDonorInfoFuncAsync,
            Func<TDonor, FailedDonorInfo> getFailedDonorInfo,
            string failureEventName);
    }

    public abstract class DonorBatchProcessor<TDonor, TResult, TException> : IDonorBatchProcessor<TDonor, TResult>
        where TException : Exception
    {
        private readonly ILogger logger;

        protected DonorBatchProcessor(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task<DonorBatchProcessingResult<TResult>> ProcessBatch(
            IEnumerable<TDonor> donorInfo,
            Func<TDonor, Task<TResult>> processDonorInfoFuncAsync,
            Func<TDonor, FailedDonorInfo> getFailedDonorInfo,
            string failureEventName)
        {
            donorInfo = donorInfo.ToList();

            if (!donorInfo.Any())
            {
                return new DonorBatchProcessingResult<TResult>();
            }

            var results = new List<TResult>();
            var failedDonors = new List<FailedDonorInfo>();

            foreach (var d in donorInfo)
            {
                var result = await ProcessDonorInfo(
                        processDonorInfoFuncAsync,
                        getFailedDonorInfo,
                        failureEventName,
                        d,
                        failedDonors);

                if (result != null)
                {
                    results.Add(result);
                }
            }

            return new DonorBatchProcessingResult<TResult>
            {
                ProcessingResults = results,
                FailedDonors = failedDonors
            };
        }

        public async Task<DonorBatchProcessingResult<TResult>> ProcessBatchAsParallel(
            IEnumerable<TDonor> donorInfo,
            Func<TDonor, Task<TResult>> processDonorInfoFuncAsync,
            Func<TDonor, FailedDonorInfo> getFailedDonorInfo,
            string failureEventName)
        {
            donorInfo = donorInfo.ToList();

            if (!donorInfo.Any())
            {
                return new DonorBatchProcessingResult<TResult>();
            }

            var failedDonors = new List<FailedDonorInfo>();

            var results = await Task.WhenAll(donorInfo.Select(async donor =>
                await ProcessDonorInfo(
                    processDonorInfoFuncAsync,
                    getFailedDonorInfo,
                    failureEventName,
                    donor,
                    failedDonors)));

            return new DonorBatchProcessingResult<TResult>
            {
                ProcessingResults = results.Where(d => d != null),
                FailedDonors = failedDonors
            };
        }

        private async Task<TResult> ProcessDonorInfo(
            Func<TDonor, Task<TResult>> processDonorInfoFuncAsync,
            Func<TDonor, FailedDonorInfo> getFailedDonorInfo,
            string failureEventName,
            TDonor d,
            ICollection<FailedDonorInfo> failedDonors)
        {
            try
            {
                return await processDonorInfoFuncAsync(d);
            }
            catch (TException e)
            {
                var failedDonorInfo = getFailedDonorInfo(d);
                failedDonors.Add(failedDonorInfo);

                var eventModel = DonorProcessingFailureEventModelFactory<TException>.GetEventModel(
                    failureEventName,
                    new DonorProcessingException<TException>(failedDonorInfo, e));
                logger.SendEvent(eventModel);

                return default;
            }
        }
    }
}
