﻿namespace Atlas.MatchingAlgorithm.Test.Validation.TestData.Models.Hla
{
    /// <summary>
    /// Determines which dataset the genotype generator should use when deciding alleles
    /// Some datasets are computed from others, so not all will correspond to a file in Resources
    /// </summary>
    public enum Dataset
    {
        FourFieldTgsAlleles,
        ThreeFieldTgsAlleles,
        TwoFieldTgsAlleles,
        // Includes all 2/3/4 field alleles, when the field number is not specified
        TgsAlleles,
        // p-group (not g-group) match possible
        PGroupMatchPossible,
        // g-group (not allele) match possible
        GGroupMatchPossible,
        // cdna (not gdna) match possible
        CDnaMatchPossible,
        // protein level match possible
        ProteinMatchPossible,
        // Three field (but not fourth field) match possible. AKA fourth field difference
        FourFieldAllelesWithThreeFieldMatchPossible,
        // Two field (but not third field) match possible. AKA third field difference
        ThreeFieldAllelesWithTwoFieldMatchPossible,
        // Allele string of subtypes can be created
        // i.e. multiple alleles with same first field, and within that group multiple second fields exist
        AlleleStringOfSubtypesPossible,
        // Null expressing single alleles. (AKA the allele expression suffix is 'N')
        NullAlleles,
        // Alleles with an expression suffix that is not 'N'
        AllelesWithNonNullExpressionSuffix,
        // For some test cases, it must be possible to generate both an allele string with the same p group as the selected allele,
        // and one with at least one different p-group
        AllelesWithStringsOfSingleAndMultiplePGroupsPossible
    }
}