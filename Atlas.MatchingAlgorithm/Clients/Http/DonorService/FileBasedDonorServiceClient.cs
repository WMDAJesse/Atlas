using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Atlas.Common.ApplicationInsights;
using Atlas.MatchingAlgorithm.Client.Models.Donors;
using Atlas.MatchingAlgorithm.Models;
using CsvHelper;
using CsvHelper.Configuration;

namespace Atlas.MatchingAlgorithm.Clients.Http.DonorService
{
    public class FileBasedDonorServiceClient : IDonorServiceClient
    {
        private const string EmbeddedDonorsFile = "Atlas.MatchingAlgorithm.Resources.InitialDonors.csv";
        private readonly ILogger logger;

        public FileBasedDonorServiceClient(ILogger logger = null)
        {
            this.logger = logger;
        }

        public Task<SearchableDonorInformationPage> GetDonorsInfoForSearchAlgorithm(int resultsPerPage, int lastId)
        {
            var allDonors = ReadAllDonors();
            logger.SendTrace($"Read {allDonors.Count} donor records from file, rather than contacting remote service.",
                LogLevel.Trace);

            var lastDonorOnRecord = allDonors.Last().DonorId;

            if (lastId == lastDonorOnRecord)
            {
                return Task.FromResult(new SearchableDonorInformationPage
                {
                    DonorsInfo = new List<SearchableDonorInformation>(),
                    ResultsPerPage = resultsPerPage,
                    LastId = -1,
                });
            }

            var donorsToReturn =
                (lastId <= 0 ? allDonors : allDonors.SkipWhile(donor => donor.DonorId != lastId).Skip(1))
                .Take(resultsPerPage)
                .ToList();

            if (!donorsToReturn.Any())
            {
                throw new ArgumentException(
                    "A positive LastId was provided, but did not match any any Donor in the list of DonorIds. Unclear how this could occur.",
                    nameof(lastId));
            }

            return Task.FromResult(new SearchableDonorInformationPage
            {
                DonorsInfo = donorsToReturn,
                ResultsPerPage = donorsToReturn.Count,
                LastId = donorsToReturn.Last().DonorId,
            });
        }

        private static List<SearchableDonorInformation> ReadAllDonors()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(EmbeddedDonorsFile))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, new Configuration {Quote = '\''}))
            {
                return csv.GetRecords<SearchableDonorInformation>().ToList();
            }
        }
    }
}