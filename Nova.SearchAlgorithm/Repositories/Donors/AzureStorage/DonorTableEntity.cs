using Microsoft.WindowsAzure.Storage.Table;
using Nova.SearchAlgorithm.Client.Models;
using Nova.SearchAlgorithm.Models;

namespace Nova.SearchAlgorithm.Repositories.Donors.AzureStorage
{
    public class DonorTableEntity : TableEntity
    {
        public string SerialisedDonor { get; set; }

        public int DonorId { get; set; }
        public string DonorType { get; set; }
        public RegistryCode RegistryCode { get; set; }
        // TODO:NOVA-919 Rename
        // TODO:NOVA-919 expand into concrete type with Locus, Value
        //
        // This field will potentially store both serologies and pgroups, to simplify querying by match.
        // TODO:NOVA-919 consider splitting into two tables/fields
        public PhenotypeInfo<MatchingHla> MatchingHla { get; set; }

        public DonorTableEntity() { }

        public DonorTableEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }
    }
}