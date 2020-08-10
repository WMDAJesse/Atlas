using Atlas.Common.GeneticData.PhenotypeInfo;
using Atlas.Common.GeneticData.PhenotypeInfo.TransferModels;
using Atlas.MatchPrediction.ExternalInterface.Models.HaplotypeFrequencySet;
using Atlas.MatchPrediction.ExternalInterface.Models.MatchProbability;
using LochNessBuilder;
using Builder = LochNessBuilder.Builder<Atlas.MatchPrediction.ExternalInterface.Models.MatchProbability.SingleDonorMatchProbabilityInput>;

namespace Atlas.MatchPrediction.Test.TestHelpers.Builders.MatchProbabilityInputs
{
    [Builder]
    public static class SingleDonorMatchProbabilityInputBuilder
    {
        public static Builder Default => Builder.New
            .With(i => i.DonorInput, new DonorInput())
            .WithPatientHla(new PhenotypeInfo<string>("hla"))
            .WithDonorHla(new PhenotypeInfo<string>("hla"))
            .WithHlaNomenclature("nomenclature-version");

        public static Builder WithDonorHla(this Builder builder, PhenotypeInfo<string> donorHla)
        {
            var donorInput = builder.Build().DonorInput;
            donorInput.DonorHla = donorHla?.ToPhenotypeInfoTransfer();
            return builder.With(i => i.DonorInput, donorInput);
        }

        public static Builder WithDonorMetadata(this Builder builder, FrequencySetMetadata frequencySetMetadata)
        {
            var donorInput = builder.Build().DonorInput;
            donorInput.DonorFrequencySetMetadata = frequencySetMetadata;
            return builder.With(i => i.DonorInput, donorInput);
        }

        public static Builder WithPatientHla(this Builder builder, PhenotypeInfo<string> patientHla) =>
            builder.With(i => i.PatientHla, patientHla?.ToPhenotypeInfoTransfer());

        public static Builder WithPatientMetadata(this Builder builder, FrequencySetMetadata frequencySetMetadata) =>
            builder.With(i => i.PatientFrequencySetMetadata, frequencySetMetadata);

        public static Builder WithHlaNomenclature(this Builder builder, string nomenclatureVersion) =>
            builder.With(i => i.HlaNomenclatureVersion, nomenclatureVersion);
    }
}