using System.Collections.Generic;

namespace Atlas.HlaMetadataDictionary.WmdaDataAccess.Models
{
    /// <summary>
    /// Container for all parsed information from published WMDA allele data
    /// We do not want the collections to be of type IEnumerable as they are accessed thousands of times during the Metadata Dictionary refresh
    /// Without enumerating them first we risk a huge performance hit due to multiple enumerations of large collections
    /// </summary>
    internal class WmdaDataset
    {
        public string HlaNomenclatureVersion { get; set; }
        public IList<HlaNom> Serologies { get; set; }
        public IList<HlaNom> Alleles { get; set; }
        public IList<HlaNomP> PGroups { get; set; }
        public IList<HlaNomG> GGroups { get; set; }
        public IList<RelSerSer> SerologyToSerologyRelationships { get; set; }
        public IList<RelDnaSer> AlleleToSerologyRelationships { get; set; }
        public IList<ConfidentialAllele> ConfidentialAlleles { get; set; }
        public IList<AlleleStatus> AlleleStatuses { get; set; }
        public IList<AlleleNameHistory> AlleleNameHistories { get; set; }
        public IList<Dpb1TceGroupAssignment> Dpb1TceGroupAssignments { get; set; }
    }
}