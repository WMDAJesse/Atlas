﻿using System;
using Nova.SearchAlgorithm.Client.Models;
using Nova.SearchAlgorithm.Common.Models;
using Nova.SearchAlgorithm.Data.Entity;
using Nova.SearchAlgorithm.Test.Validation.TestData.Models;

namespace Nova.SearchAlgorithm.Test.Validation.TestData.Builders
{
    public class DonorBuilder
    {
        private readonly Donor donor;
        private readonly Genotype genotype;

        public DonorBuilder(Genotype genotype)
        {
            this.genotype = genotype;
            donor = new Donor {DonorId = DonorIdGenerator.NextId()};
        }

        public DonorBuilder WithFullTypingCategory(HlaTypingCategory category)
        {
            switch (category)
            {
                case HlaTypingCategory.Tgs:
                    AdornDonorWithHla(genotype.Hla.Map((l, p, tgsAllele) => tgsAllele.TgsTypedAllele));
                    break;
                case HlaTypingCategory.ThreeField:
                    AdornDonorWithHla(genotype.Hla.Map((l, p, tgsAllele) => tgsAllele.ThreeFieldAllele));
                    break;
                case HlaTypingCategory.TwoField:
                    AdornDonorWithHla(genotype.Hla.Map((l, p, tgsAllele) => tgsAllele.TwoFieldAllele));
                    break;
                case HlaTypingCategory.XxCode:
                    AdornDonorWithHla(genotype.Hla.Map((l, p, tgsAllele) => tgsAllele.XxCode));
                    break;
                case HlaTypingCategory.NmdpCode:
                    AdornDonorWithHla(genotype.Hla.Map((l, p, tgsAllele) => tgsAllele.NmdpCode));
                    break;
                case HlaTypingCategory.Serology:
                    AdornDonorWithHla(genotype.Hla.Map((l, p, tgsAllele) => tgsAllele.Serology));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), category, null);
            }

            return this;
        }

        public DonorBuilder OfType(DonorType donorType)
        {
            donor.DonorType = donorType;
            return this;
        }
        
        public DonorBuilder AtRegistry(RegistryCode registryCode)
        {
            donor.RegistryCode = registryCode;
            return this;
        }
                
        public Donor Build()
        {
            return donor;
        }

        private void AdornDonorWithHla(PhenotypeInfo<string> hla)
        {
            donor.A_1 = hla.A_1;
            donor.A_2 = hla.A_2;
            donor.B_1 = hla.B_1;
            donor.B_2 = hla.B_2;
            donor.DRB1_1 = hla.DRB1_1;
            donor.DRB1_2 = hla.DRB1_2;
            donor.C_1 = hla.C_1;
            donor.C_2 = hla.C_2;
            donor.DQB1_1 = hla.DQB1_1;
            donor.DQB1_2 = hla.DQB1_2;
        }
    }
}