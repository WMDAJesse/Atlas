﻿using Atlas.Common.GeneticData.PhenotypeInfo;
using Atlas.Common.GeneticData.PhenotypeInfo.TransferModels;
using Atlas.MatchingAlgorithm.Client.Models.Donors;
using Atlas.MatchingAlgorithm.Data.Models.DonorInfo;

namespace Atlas.MatchingAlgorithm.Extensions
{
    internal static class SearchableDonorInformationExtensions
    {
        public static DonorInfo ToDonorInfo(this SearchableDonorInformation donor)
        {
            return new DonorInfo
            {
                DonorId = donor.DonorId,
                DonorType = donor.DonorType,
                HlaNames = donor.HlaAsPhenotype()
            };
        }

        public static PhenotypeInfo<string> HlaAsPhenotype(this SearchableDonorInformation donor)
        {
            return new PhenotypeInfo<string>
            (
                valueA: new LocusInfo<string>(donor.A_1, donor.A_2),
                valueB: new LocusInfo<string>(donor.B_1, donor.B_2),
                valueC: new LocusInfo<string>(donor.C_1, donor.C_2),
                valueDpb1: new LocusInfo<string>(donor.DPB1_1, donor.DPB1_2),
                valueDqb1: new LocusInfo<string>(donor.DQB1_1, donor.DQB1_2),
                valueDrb1: new LocusInfo<string>(donor.DRB1_1, donor.DRB1_2)
            );
        }

        public static PhenotypeInfoTransfer<string> HlaAsPhenotypeInfoTransfer(this SearchableDonorInformation donor) =>
            donor.HlaAsPhenotype().ToPhenotypeInfoTransfer();
    }
}