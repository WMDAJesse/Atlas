﻿using System;
using Nova.SearchAlgorithm.Client.Models;
using Nova.SearchAlgorithm.Common.Models;

namespace Nova.SearchAlgorithm.Test.Integration.TestHelpers.Builders
{
    public class InputDonorBuilder
    {
        private readonly InputDonor donor;
        
        public InputDonorBuilder(int donorId)
        {
            donor = new InputDonor
            {
                RegistryCode = RegistryCode.AN,
                DonorType = DonorType.Adult,
                DonorId = donorId,
                MatchingHla = new PhenotypeInfo<ExpandedHla>()
            };
        }

        public InputDonorBuilder WithMatchingHla(PhenotypeInfo<ExpandedHla> matchingHla)
        {
            donor.MatchingHla = matchingHla;
            return this;
        }

        public InputDonorBuilder WithMatchingHlaAtLocus(Locus locus, ExpandedHla hla1, ExpandedHla hla2)
        {
            switch (locus)
            {
                case Locus.A:
                    donor.MatchingHla.A_1 = hla1;
                    donor.MatchingHla.A_2 = hla2;
                    break;
                case Locus.B:
                    donor.MatchingHla.B_1 = hla1;
                    donor.MatchingHla.B_2 = hla2;
                    break;
                case Locus.C:
                    donor.MatchingHla.C_1 = hla1;
                    donor.MatchingHla.C_2 = hla2;
                    break;
                case Locus.Dpb1:
                    donor.MatchingHla.Dpb1_1 = hla1;
                    donor.MatchingHla.Dpb1_2 = hla2;
                    break;
                case Locus.Dqb1:
                    donor.MatchingHla.Dqb1_1 = hla1;
                    donor.MatchingHla.Dqb1_2 = hla2;
                    break;
                case Locus.Drb1:
                    donor.MatchingHla.Drb1_1 = hla1;
                    donor.MatchingHla.Drb1_2 = hla2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(locus), locus, null);
            }
            return this;
        }

        // Populates all null required hla positions (A, B, Drb1) with given hla values
        public InputDonorBuilder WithDefaultRequiredHla(ExpandedHla hla)
        {
            donor.MatchingHla.A_1 = donor.MatchingHla.A_1 ?? hla;
            donor.MatchingHla.A_2 = donor.MatchingHla.A_2 ?? hla;
            donor.MatchingHla.B_1 = donor.MatchingHla.B_1 ?? hla;
            donor.MatchingHla.B_2 = donor.MatchingHla.B_2 ?? hla;
            donor.MatchingHla.Drb1_1 = donor.MatchingHla.Drb1_1 ?? hla;
            donor.MatchingHla.Drb1_2 = donor.MatchingHla.Drb1_2 ?? hla;
            return this;
        }

        public InputDonorBuilder WithRegistryCode(RegistryCode registryCode)
        {
            donor.RegistryCode = registryCode;
            return this;
        }
        
        public InputDonorBuilder WithDonorType(DonorType donorType)
        {
            donor.DonorType = donorType;
            return this;
        }
        
        public InputDonor Build()
        {
            return donor;
        }
    }
}