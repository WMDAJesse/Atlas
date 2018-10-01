﻿using FluentAssertions;
using Nova.SearchAlgorithm.Common.Models;
using Nova.SearchAlgorithm.Test.Validation.TestData.Builders;
using Nova.SearchAlgorithm.Test.Validation.TestData.Exceptions;
using Nova.SearchAlgorithm.Test.Validation.TestData.Models;
using Nova.SearchAlgorithm.Test.Validation.TestData.Models.Hla;
using Nova.SearchAlgorithm.Test.Validation.TestData.Models.PatientDataSelection;
using Nova.SearchAlgorithm.Test.Validation.TestData.Repositories;
using Nova.SearchAlgorithm.Test.Validation.TestData.Services.PatientDataSelection;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Locus = Nova.SearchAlgorithm.Common.Models.Locus;

namespace Nova.SearchAlgorithm.Test.Validation.ValidationFrameworkUnitTests.PatientDataSelection.DataSelectors
{
    [TestFixture]
    public class PatientHlaSelectorTests
    {
        private IPatientHlaSelector patientHlaSelector;
        private IAlleleRepository alleleRepository;

        private int alleleNumber;
        private PhenotypeInfo<List<AlleleTestData>> alleles;
        private LocusInfo<AlleleTestData> patientPGroupAlleles;
        private LocusInfo<List<AlleleTestData>> donorPGroupAlleles;
        private PhenotypeInfo<List<AlleleTestData>> gGroupAlleles;

        [SetUp]
        public void SetUp()
        {
            alleleRepository = Substitute.For<IAlleleRepository>();

            alleles = new PhenotypeInfo<bool>().Map((l, p, noop) => new List<AlleleTestData>
            {
                GetTestAllele(l, p, MatchLevel.Allele),
                GetTestAllele(l, p, MatchLevel.Allele),
            });

            patientPGroupAlleles = new LocusInfo<bool>().Map((l, noop) => GetTestAllele(l, TypePositions.Both, MatchLevel.PGroup));

            donorPGroupAlleles = new LocusInfo<bool>().Map((l, noop) => new List<AlleleTestData>
            {
                GetTestAllele(l, TypePositions.Both, MatchLevel.PGroup),
                GetTestAllele(l, TypePositions.Both, MatchLevel.PGroup),
            });

            gGroupAlleles = new LocusInfo<bool>().Map((l, noop) => new List<AlleleTestData>
            {
                GetTestAllele(l, TypePositions.Both, MatchLevel.GGroup),
                GetTestAllele(l, TypePositions.Both, MatchLevel.GGroup),
            }).ToPhenotypeInfo((l, a) => a);

            alleleRepository.AllelesForGGroupMatching().Returns(gGroupAlleles);
            alleleRepository.PatientAllelesForPGroupMatching().Returns(patientPGroupAlleles);
            alleleRepository.AllTgsAlleles().Returns(alleles);
            alleleRepository.DonorAllelesForPGroupMatching().Returns(donorPGroupAlleles);

            patientHlaSelector = new PatientHlaSelector(alleleRepository);
        }

        [Test]
        public void GetPatientHla_ReturnsMatchingAlleleFromMetaDonorGenotype()
        {
            var criteria = new PatientHlaSelectionCriteriaBuilder().WithMatchOrientationAtLocus(Locus.A, MatchOrientation.Direct).Build();

            var metaDonor = new MetaDonor
            {
                Genotype =
                {
                    Hla = alleles.Map((locus, p, all) => TgsAllele.FromTestDataAllele(all.First()))
                }
            };

            var patientHla = patientHlaSelector.GetPatientHla(metaDonor, criteria);

            patientHla.A_1.Should().Be(metaDonor.Genotype.Hla.A_1.TgsTypedAllele);
        }

        [Test]
        public void GetPatientHla_ForPGroupMatchLevel_DoesNotSelectExactAlleleMatch()
        {
            var criteria = new PatientHlaSelectionCriteriaBuilder()
                .WithMatchOrientationAtLocus(Locus.A, MatchOrientation.Direct)
                .WithMatchLevelAtAllLoci(MatchLevel.PGroup)
                .Build();

            var metaDonor = new MetaDonor
            {
                Genotype =
                {
                    Hla = donorPGroupAlleles.Map((locus, all) =>
                            TgsAllele.FromTestDataAllele(all.First()))
                        .ToPhenotypeInfo(((locus, allele) => new Tuple<TgsAllele, TgsAllele>(allele, allele))
                        )
                }
            };

            var patientHla = patientHlaSelector.GetPatientHla(metaDonor, criteria);

            patientHla.A_1.Should().NotBe(metaDonor.Genotype.Hla.A_1.TgsTypedAllele);
        }

        [Test]
        public void GetPatientHla_ForGGroupMatchLevel_DoesNotSelectExactAlleleMatch()
        {
            var criteria = new PatientHlaSelectionCriteriaBuilder()
                .WithMatchOrientationAtLocus(Locus.A, MatchOrientation.Direct)
                .WithMatchLevelAtAllLoci(MatchLevel.GGroup)
                .Build();

            var metaDonor = new MetaDonor
            {
                Genotype =
                {
                    Hla = gGroupAlleles.Map((locus, p, all) => TgsAllele.FromTestDataAllele(all.First()))
                }
            };

            var patientHla = patientHlaSelector.GetPatientHla(metaDonor, criteria);

            patientHla.A_1.Should().NotBe(metaDonor.Genotype.Hla.A_1.TgsTypedAllele);
        }

        [Test]
        public void GetPatientHla_ForUntypedLocus_ReturnsNull()
        {
            var criteria = new PatientHlaSelectionCriteria
            {
                PatientTypingResolutions = new PhenotypeInfo<HlaTypingResolution>(HlaTypingResolution.Untyped),
                Orientations = new LocusInfo<MatchOrientation>(MatchOrientation.Direct)
            };

            var metaDonor = new MetaDonor
            {
                Genotype =
                {
                    Hla = alleles.Map((locus, p, all) => TgsAllele.FromTestDataAllele(all.First()))
                }
            };

            var patientHla = patientHlaSelector.GetPatientHla(metaDonor, criteria);

            patientHla.ToEnumerable().All(x => x == null).Should().BeTrue();
        }
        
        [TestCase(TypePositions.One)]
        [TestCase(TypePositions.Two)]
        public void GetPatientHla_ForHomozygousLocus_WhenShouldMatchAtOnePosition_ReturnsMatchingAlleleAtEachPosition(TypePositions matchingPosition)
        {
            const Locus locus = Locus.A;
            var criteria = new PatientHlaSelectionCriteriaBuilder()
                .MatchingAtPosition(locus, matchingPosition)
                .NotMatchingAtPosition(locus, matchingPosition.Other())
                .WithMatchOrientationAtLocus(locus, MatchOrientation.Direct)
                .HomozygousAtLocus(locus)
                .Build();

            var metaDonor = new MetaDonor
            {
                Genotype =
                {
                    Hla = alleles.Map((l, p, all) => TgsAllele.FromTestDataAllele(all.First()))
                }
            };

            var patientHla = patientHlaSelector.GetPatientHla(metaDonor, criteria);
            var hlaAtLocus = patientHla.DataAtLocus(locus);

            hlaAtLocus.Item1.Should().Be(metaDonor.Genotype.Hla.DataAtPosition(locus, matchingPosition).TgsTypedAllele);
            hlaAtLocus.Item2.Should().Be(metaDonor.Genotype.Hla.DataAtPosition(locus, matchingPosition).TgsTypedAllele);
        }        
        
        [Test]
        public void GetPatientHla_ForHomozygousLocus_WhenShouldMatchAtNeitherPosition_ReturnsSameAlleleAtEachPosition()
        {
            const Locus locus = Locus.A;
            var criteria = new PatientHlaSelectionCriteriaBuilder()
                .NotMatchingAtPosition(locus, TypePositions.Both)
                .WithMatchOrientationAtLocus(locus, MatchOrientation.Direct)
                .HomozygousAtLocus(locus)
                .Build();

            var metaDonor = new MetaDonor
            {
                Genotype =
                {
                    Hla = alleles.Map((l, p, all) => TgsAllele.FromTestDataAllele(all.First()))
                }
            };

            var patientHla = patientHlaSelector.GetPatientHla(metaDonor, criteria);
            var hlaAtLocus = patientHla.DataAtLocus(locus);            

            hlaAtLocus.Item1.Should().Be(hlaAtLocus.Item2);
        }   
        
        [Test]
        public void GetPatientHla_ForHomozygousLocus_WhenShouldMatchAtBothPositions_AndDonorNotHomozygous_ThrowsException()
        {
            const Locus locus = Locus.A;
            var criteria = new PatientHlaSelectionCriteriaBuilder()
                .MatchingAtPosition(locus, TypePositions.Both)
                .WithMatchOrientationAtLocus(locus, MatchOrientation.Direct)
                .HomozygousAtLocus(locus)
                .Build();

            var metaDonor = new MetaDonor
            {
                Genotype =
                {
                    Hla = alleles.Map((l, p, all) => TgsAllele.FromTestDataAllele(all.First()))
                }
            };

            Assert.Throws<HlaSelectionException>(() => patientHlaSelector.GetPatientHla(metaDonor, criteria));
        }    
        
        [Test]
        public void GetPatientHla_ForHomozygousLocus_WhenShouldMatchAtBothPositions_AndDonorHomozygous_ReturnsDonorHla()
        {
            const Locus locus = Locus.A;
            var criteria = new PatientHlaSelectionCriteriaBuilder()
                .MatchingAtPosition(locus, TypePositions.Both)
                .WithMatchOrientationAtLocus(locus, MatchOrientation.Direct)
                .HomozygousAtLocus(locus)
                .Build();

            var metaDonor = new MetaDonor
            {
                Genotype =
                {
                    Hla = alleles.MapByLocus((l, all1, all2) =>
                    {
                        var allele = TgsAllele.FromTestDataAllele(all1.First());
                        return new Tuple<TgsAllele, TgsAllele>(allele, allele);
                    }),
                }
            };

            var patientHla = patientHlaSelector.GetPatientHla(metaDonor, criteria);
            var hlaAtLocus = patientHla.DataAtLocus(locus);
            var donorHla1 = metaDonor.Genotype.Hla.DataAtPosition(locus, TypePositions.One).TgsTypedAllele;
            var donorHla2 = metaDonor.Genotype.Hla.DataAtPosition(locus, TypePositions.Two).TgsTypedAllele;
            
            hlaAtLocus.Item1.Should().Be(donorHla1);
            hlaAtLocus.Item2.Should().Be(donorHla2);
            donorHla1.Should().Be(donorHla2);
        }

        [Test]
        public void GetPatientHla_ForExpressingMismatch_ReturnsHlaNotMatchingDonor()
        {
            var criteria = new PatientHlaSelectionCriteriaBuilder()
                .WithHlaSourceAtPosition(Locus.A, TypePositions.One, PatientHlaSource.ExpressingAlleleMismatch)
                .Build();

            var metaDonor = new MetaDonor
            {
                Genotype =
                {
                    Hla = alleles.Map((locus, p, all) => TgsAllele.FromTestDataAllele(all.First()))
                }
            };

            var patientHla = patientHlaSelector.GetPatientHla(metaDonor, criteria);

            patientHla.A_1.Should().NotBe(metaDonor.Genotype.Hla.A_1.TgsTypedAllele);
        }
        
        [Test]
        public void GetPatientHla_ForNullMismatch_ReturnsAlleleNotMatchingDonor()
        {
            var criteria = new PatientHlaSelectionCriteriaBuilder()
                .WithHlaSourceAtPosition(Locus.A, TypePositions.One, PatientHlaSource.NullAlleleMismatch)
                .Build();

            var metaDonor = new MetaDonor
            {
                Genotype =
                {
                    Hla = alleles.Map((locus, p, all) => TgsAllele.FromTestDataAllele(all.First()))
                }
            };

            var patientHla = patientHlaSelector.GetPatientHla(metaDonor, criteria);

            patientHla.A_1.Should().NotBe(metaDonor.Genotype.Hla.A_1.TgsTypedAllele);
        }
        
        [Test]
        public void GetPatientHla_ForNullMismatch_ReturnsNullAllele()
        {
            var criteria = new PatientHlaSelectionCriteriaBuilder()
                .WithHlaSourceAtPosition(Locus.A, TypePositions.One, PatientHlaSource.NullAlleleMismatch)
                .Build();

            var metaDonor = new MetaDonor
            {
                Genotype =
                {
                    Hla = alleles.Map((locus, p, all) => TgsAllele.FromTestDataAllele(all.First()))
                }
            };

            var patientHla = patientHlaSelector.GetPatientHla(metaDonor, criteria);

            patientHla.A_1.Should().Contain("N");
        }
        
        [Test]
        public void GetPatientHla_IncludingNullMismatch_NeverReturnsCrossMatchWhenArbitraryMatchOrientationRequested()
        {
            var criteria = new PatientHlaSelectionCriteriaBuilder()
                .WithMatchOrientationAtLocus(Locus.A, MatchOrientation.Arbitrary)
                .WithHlaSourceAtPosition(Locus.A, TypePositions.One, PatientHlaSource.NullAlleleMismatch)
                .Build();

            var metaDonor = new MetaDonor
            {
                Genotype =
                {
                    Hla = alleles.Map((locus, p, all) => TgsAllele.FromTestDataAllele(all.First()))
                }
            };

            // There is a random element to the orientation selection
            // To avoid false positives, we repeat this test multiple time to ensure that the result does not change
            for (var i = 0; i < 10; i++)
            {
                var patientHla = patientHlaSelector.GetPatientHla(metaDonor, criteria);

                patientHla.A_1.Should().Contain("N");
                patientHla.A_2.Should().NotContain("N");
            }
        }
        
        private AlleleTestData GetTestAllele(Locus locus, TypePositions positions, MatchLevel matchLevel)
        {
            string positionString;
            switch (positions)
            {
                case TypePositions.One:
                    positionString = "1";
                    break;
                case TypePositions.Two:
                    positionString = "2";
                    break;
                case TypePositions.Both:
                    positionString = "1&2";
                    break;
                case TypePositions.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(positions), positions, null);
            }

            alleleNumber++;

            return new AlleleTestData
            {
                AlleleName = $"01:01:{alleleNumber}:{locus}-{positionString} matchlevel={matchLevel}",
                PGroup = "p-1",
                GGroup = "g-1",
                NmdpCode = "nmdp-1",
                Serology = "s-1"
            };
        }
    }
}