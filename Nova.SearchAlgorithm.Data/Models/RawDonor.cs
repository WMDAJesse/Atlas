﻿using Nova.SearchAlgorithm.Client.Models;

namespace Nova.SearchAlgorithm.Data.Models
{
    /// <summary>
    /// A donor from our data source along with the donor's raw hla data.
    /// </summary>
    public class RawDonor
    {
        public int DonorId { get; set; }
        public DonorType DonorType { get; set; }
        public RegistryCode RegistryCode { get; set; }

        public PhenotypeInfo<string> HlaNames { get; set; }
    }
}