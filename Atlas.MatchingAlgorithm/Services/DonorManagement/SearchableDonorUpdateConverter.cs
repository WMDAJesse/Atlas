﻿using FluentValidation;
using Atlas.MatchingAlgorithm.Extensions;
using Atlas.MatchingAlgorithm.Models;
using Atlas.MatchingAlgorithm.Services.Donors;
using Atlas.MatchingAlgorithm.Validators.DonorInfo;
using Nova.Utils.ApplicationInsights;
using Nova.Utils.ServiceBus.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.MatchingAlgorithm.Services.DonorManagement
{
    public interface ISearchableDonorUpdateConverter
    {
        Task<DonorBatchProcessingResult<DonorAvailabilityUpdate>> ConvertSearchableDonorUpdatesAsync(
            IEnumerable<ServiceBusMessage<SearchableDonorUpdate>> updates);
    }

    public class SearchableDonorUpdateConverter :
        DonorBatchProcessor<ServiceBusMessage<SearchableDonorUpdate>, DonorAvailabilityUpdate, ValidationException>,
        ISearchableDonorUpdateConverter
    {
        private const string UpdateFailureEventName = "Searchable Donor Update Failure(s) in the Search Algorithm";

        public SearchableDonorUpdateConverter(ILogger logger) : base(logger)
        {
        }

        public async Task<DonorBatchProcessingResult<DonorAvailabilityUpdate>> ConvertSearchableDonorUpdatesAsync(
            IEnumerable<ServiceBusMessage<SearchableDonorUpdate>> updates)
        {
            return await ProcessBatchAsync(
                updates,
                async update => await GetDonorAvailabilityUpdate(update),
                update => new FailedDonorInfo(update)
                {
                    DonorId = update.DeserializedBody?.DonorId
                },
                UpdateFailureEventName);
        }

        private static async Task<DonorAvailabilityUpdate> GetDonorAvailabilityUpdate(ServiceBusMessage<SearchableDonorUpdate> update)
        {
            await new DonorUpdateMessageValidator().ValidateAndThrowAsync(update);

            var body = update.DeserializedBody;

            return new DonorAvailabilityUpdate
            {
                UpdateSequenceNumber = update.SequenceNumber,
                UpdateDateTime = body.PublishedDateTime.Value,
                DonorId = int.Parse(body.DonorId),
                DonorInfo = body.SearchableDonorInformation?.ToDonorInfo(),
                IsAvailableForSearch = body.IsAvailableForSearch
            };
        }
    }
}
