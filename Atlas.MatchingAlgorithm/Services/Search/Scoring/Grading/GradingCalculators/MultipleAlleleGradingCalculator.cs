﻿using System.Linq;
using Atlas.HlaMetadataDictionary.ExternalInterface.Models.Metadata.ScoringMetadata;
using Atlas.MatchingAlgorithm.Client.Models.SearchResults.PerLocus;

namespace Atlas.MatchingAlgorithm.Services.Search.Scoring.Grading.GradingCalculators
{
    /// <summary>
    /// To be used when both typings represent multiple alleles,
    /// or when one is multiple and the other is a single allele.
    /// </summary>
    public interface IMultipleAlleleGradingCalculator : IGradingCalculator
    {
    }

    public class MultipleAlleleGradingCalculator :
        GradingCalculatorBase,
        IMultipleAlleleGradingCalculator
    {
        private readonly IPermissiveMismatchCalculator permissiveMismatchCalculator;

        public MultipleAlleleGradingCalculator(IPermissiveMismatchCalculator permissiveMismatchCalculator)
        {
            this.permissiveMismatchCalculator = permissiveMismatchCalculator;
        }

        protected override bool ScoringInfosAreOfPermittedTypes(
            IHlaScoringInfo patientInfo,
            IHlaScoringInfo donorInfo)
        {
            var bothAreMultiple =
                patientInfo is MultipleAlleleScoringInfo &&
                donorInfo is MultipleAlleleScoringInfo;

            var patientIsMultipleDonorIsSingle =
                patientInfo is MultipleAlleleScoringInfo &&
                donorInfo is SingleAlleleScoringInfo;

            var patientIsSingleDonorIsMultiple =
                patientInfo is SingleAlleleScoringInfo &&
                donorInfo is MultipleAlleleScoringInfo;

            return bothAreMultiple || patientIsMultipleDonorIsSingle || patientIsSingleDonorIsMultiple;
        }

        /// <summary>
        /// Returns the maximum grade possible after grading every combination of patient and donor allele.
        /// </summary>
        protected override MatchGrade GetMatchGrade(
            IHlaScoringMetadata patientMetadata,
            IHlaScoringMetadata donorMetadata)
        {
            var patientAlleles = patientMetadata.GetInTermsOfSingleAlleleScoringMetadata();
            var donorAlleles = donorMetadata.GetInTermsOfSingleAlleleScoringMetadata();

            var allGrades = patientAlleles.SelectMany(patientAllele => donorAlleles, GetSingleAlleleMatchGrade);

            return allGrades.Max();
        }

        private MatchGrade GetSingleAlleleMatchGrade(
            IHlaScoringMetadata patientMetadata,
            IHlaScoringMetadata donorMetadata)
        {
            var calculator = GradingCalculatorFactory.GetGradingCalculator(
                permissiveMismatchCalculator,
                patientMetadata.HlaScoringInfo,
                donorMetadata.HlaScoringInfo);

            return calculator.CalculateGrade(patientMetadata, donorMetadata);
        }
    }
}