using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CsvHelper;
using Nova.DonorService.Client.Models.SearchableDonors;
using Nova.Utils.ApplicationInsights;
using Nova.Utils.Client;
using Nova.Utils.Http;

namespace Atlas.MatchingAlgorithm.Clients.Http
{
    public interface IDonorServiceClient
    {
        /// <summary>
        /// Returns a page of donors information which is required for the new search algorithm.
        /// These donors Info only contain the information required to do a search with the new algorithm and not for display in the frontend.
        /// Useful for any client wishing to process all donors information one page at a time.
        /// </summary>
        /// <param name="resultsPerPage">The number of donors required per page</param>
        /// <param name="lastId">The last ID of the previous page. This pagination system is to make sure
        /// that any client paging through donors won't miss out if donors are inserted or deleted in-between page requests.
        /// If null or omitted, the first page of results will be returned.</param>
        /// <returns>A page of donors Info for search algorithm</returns>
        Task<SearchableDonorInformationPage> GetDonorsInfoForSearchAlgorithm(int resultsPerPage, int? lastId = null);
    }

    public class FileBasedDonorServiceClient : IDonorServiceClient
    {
        private readonly string filePath;
        private readonly ILogger logger;

        public FileBasedDonorServiceClient(string filePath, ILogger logger = null)
        {
            this.filePath = filePath;
            this.logger = logger;
        }

        public async Task<SearchableDonorInformationPage> GetDonorsInfoForSearchAlgorithm(int resultsPerPage, int? lastId = null)
        {
            var allDonors = ReadAllDonors();
            
            var donorsToReturn =
                (lastId == null ? allDonors : allDonors.SkipWhile(donor => donor.DonorId != lastId.Value).Skip(1) )
                .Take(resultsPerPage)
                .ToList();
            
            return new SearchableDonorInformationPage
            {
                DonorsInfo = donorsToReturn,
                ResultsPerPage = allDonors.Count,
                LastId = donorsToReturn.Last().DonorId
            };
        }

        public List<SearchableDonorInformation> ReadAllDonors()
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Unable to file DonorOverride file.", filePath);
            }

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader))
            {
                return csv.GetRecords<SearchableDonorInformation>().ToList();
            }
        }
    }

    public class DonorServiceClient : ClientBase, IDonorServiceClient
    {
        public DonorServiceClient(ClientSettings settings, ILogger logger = null, HttpMessageHandler handler = null, IErrorsParser errorsParser = null) : base(settings, logger, handler, errorsParser)
        {
        }

        public async Task<SearchableDonorInformationPage> GetDonorsInfoForSearchAlgorithm(int resultsPerPage, int? lastId = null)
        {
            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("resultsPerPage", resultsPerPage.ToString()),
            };
            if (lastId.HasValue)
            {
                parameters.Add(new KeyValuePair<string, string>("lastId", lastId.Value.ToString()));
            }

            var request = GetRequest(HttpMethod.Get, "donors-info-for-search-algorithm", parameters);
            return await MakeRequestAsync<SearchableDonorInformationPage>(request);
        }
    }
}