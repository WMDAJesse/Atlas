﻿using Nova.SearchAlgorithm.MatchingDictionary.Models.HLATypings;

namespace Nova.SearchAlgorithm.Test.MatchingDictionary.Services.HlaMatchPreCalculation.AlleleToSerology
{
    public class AlleleToSerologyMatchingTestCaseSources
    {
        public static readonly object[] ExpressingAllelesMatchingSerologies =
        {
            new object[]
            {
                // normal Allele
                MatchLocus.A, "01:01:01:01",
                new[]
                {
                    new object[] {"A", "1", SerologySubtype.NotSplit, true}
                }
            },
            new object[]
            {
                // low Allele
                MatchLocus.B, "39:01:01:02L",
                new[]
                {
                    new object[] {"B", "3901", SerologySubtype.Associated, true},
                    new object[] {"B", "39", SerologySubtype.Split, false},
                    new object[] {"B", "16", SerologySubtype.Broad, false}
                }
            },
            new object[]
            {
                // questionable Allele
                MatchLocus.C, "07:01:01:14Q",
                new[]
                {
                    new object[] {"Cw", "7", SerologySubtype.NotSplit, true}
                }

            },
            new object[]
            {
                // secreted Allele
                MatchLocus.B, "44:02:01:02S",
                new[]
                {
                    new object[] {"B", "44", SerologySubtype.Split, true},
                    new object[] {"B", "12", SerologySubtype.Broad, false}
                }
            }
        };

        public static readonly object[] DeletedAllelesMatchingSerologies =
        {
            new object[]
            {
                // deleted allele with identical hla
                MatchLocus.A, "11:53",
                new[]
                {
                    new object[] { "A", "11", SerologySubtype.NotSplit, true }
                }
            },
            new object[]
            {
                // deleted allele is null, but identical is expressing
                MatchLocus.A, "01:34N",
                new[]
                {
                    new object[] { "A", "1", SerologySubtype.NotSplit, true }
                }
            },
            new object[]
            {
                // deleted allele is expressing, but identical is null
                MatchLocus.A, "03:260",
                new object[][]{}
            }
        };

        public static readonly object[] AllelesMappedToSpecificSubtypeMatchingSerologies =
        {
            new object[]
            {
                // Broad with no Associated
                MatchLocus.A, "26:10",
                new[]
                {
                    new object[] {"A", "10", SerologySubtype.Broad, true },
                    new object[] {"A", "25", SerologySubtype.Split, false },
                    new object[] {"A", "26", SerologySubtype.Split, false },
                    new object[] {"A", "34", SerologySubtype.Split, false },
                    new object[] {"A", "66", SerologySubtype.Split, false }
                }
            },
            new object[]
            {
                // Broad With Associated
                MatchLocus.B, "40:26",
                new[]
                {
                    new object[] {"B", "21", SerologySubtype.Broad, true },
                    new object[] {"B", "4005", SerologySubtype.Associated, false },
                    new object[] {"B", "49", SerologySubtype.Split, false },
                    new object[] {"B", "50", SerologySubtype.Split, false },
                    new object[] {"B", "40", SerologySubtype.Broad, true },
                    new object[] {"B", "60", SerologySubtype.Split, false },
                    new object[] {"B", "61", SerologySubtype.Split, false }
                }
            },
            new object[]
            {
                // Split with no Associated
                MatchLocus.C, "03:02:01",
                new[]
                {
                    new object[] {"Cw", "10", SerologySubtype.Split, true },
                    new object[] {"Cw", "3", SerologySubtype.Broad, false }
                }
            },
            new object[]
            {
                // Split with Associated
                MatchLocus.Drb1, "14:01:01",
                new[]
                {
                    new object[] {"DR", "14", SerologySubtype.Split, true },
                    new object[] {"DR", "6", SerologySubtype.Broad, false },
                    new object[] {"DR", "1403", SerologySubtype.Associated, false },
                    new object[] {"DR", "1404", SerologySubtype.Associated, false }
                }
            },
            new object[]
            {
                // Associated directly to Broad
                MatchLocus.B, "40:05:01:01",
                new[]
                {
                    new object[] {"B", "40", SerologySubtype.Broad, true },
                    new object[] {"B", "60", SerologySubtype.Split, false },
                    new object[] {"B", "61", SerologySubtype.Split, false },
                    new object[] {"B", "4005", SerologySubtype.Associated, true },
                    new object[] {"B", "21", SerologySubtype.Broad, false }
                }
            },
            new object[]
            {
                // Associated directly to Split
                MatchLocus.A, "24:03:01:01",
                new[]
                {
                    new object[] {"A", "2403", SerologySubtype.Associated, true },
                    new object[] {"A", "24", SerologySubtype.Split, false },
                    new object[] {"A", "9", SerologySubtype.Broad, false }
                }
            },
            new object[]
            {
                // Associated directly to Not-Split
                MatchLocus.Drb1, "01:03:02",
                new[]
                {
                    new object[] {"DR", "103", SerologySubtype.Associated, true },
                    new object[] {"DR", "1", SerologySubtype.NotSplit, false }
                }
            },
            new object[]
            {
                // Not-Split has Associated
                MatchLocus.B, "07:02:27",
                new[]
                {
                    new object[] {"B", "7", SerologySubtype.NotSplit, true },
                    new object[] {"B", "703", SerologySubtype.Associated, false }
                }
            },
            new object[]
            {
                // Not-Split has no Associated
                MatchLocus.Dqb1, "02:02:01:01",
                new[]
                {
                    new object[] {"DQ", "2", SerologySubtype.NotSplit, true }
                }
            }
        };

        private static readonly object[] B15BroadAllele = { MatchLocus.B, "15:33" };
        private static readonly object[] B15SplitAllele = { MatchLocus.B, "15:01:01:01" };
        private static readonly object[] B70BroadAllele = { MatchLocus.B, "15:09:01" };
        private static readonly object[] B70SplitAllele = { MatchLocus.B, "15:03:01:01" };
        private static readonly object[] B15And70BroadAllele = { MatchLocus.B, "15:36" };

        public static readonly object[] B15AllelesMatchingSerologies =
        {
            new object[]
            {
                B15BroadAllele,
                new[]
                {
                    new object[] {"B", "15", SerologySubtype.Broad, true },
                    new object[] {"B", "62", SerologySubtype.Split, false },
                    new object[] {"B", "63", SerologySubtype.Split, false },
                    new object[] {"B", "75", SerologySubtype.Split, false },
                    new object[] {"B", "76", SerologySubtype.Split, false },
                    new object[] {"B", "77", SerologySubtype.Split, false }
                }
            },
            new object[]
            {
                B15SplitAllele,
                new[]
                {
                    new object[] {"B", "62", SerologySubtype.Split, true },
                    new object[] {"B", "15", SerologySubtype.Broad, false }
                }
            },
            new object[]
            {
                B70BroadAllele,
                new[]
                {
                    new object[] {"B", "70", SerologySubtype.Broad, true },
                    new object[] {"B", "71", SerologySubtype.Split, false },
                    new object[] {"B", "72", SerologySubtype.Split, false }
                }
            },
            new object[]
            {
                B70SplitAllele,
                new[]
                {
                    new object[] {"B", "72", SerologySubtype.Split, true },
                    new object[] {"B", "70", SerologySubtype.Broad, false }
                }
            },
            new object[]
            {
                B15And70BroadAllele,
                new[]
                {
                    new object[] {"B", "15", SerologySubtype.Broad, true },
                    new object[] {"B", "62", SerologySubtype.Split, false },
                    new object[] {"B", "63", SerologySubtype.Split, false },
                    new object[] {"B", "75", SerologySubtype.Split, false },
                    new object[] {"B", "76", SerologySubtype.Split, false },
                    new object[] {"B", "77", SerologySubtype.Split, false },
                    new object[] {"B", "70", SerologySubtype.Broad, true },
                    new object[] {"B", "71", SerologySubtype.Split, false },
                    new object[] {"B", "72", SerologySubtype.Split, false }
                }
            }
        };

        public static readonly object[] AllelesOfUnknownSerology =
        {
            new object[]
            {
                // No assignments
                MatchLocus.C, "12:02:02:01",
                new object[][]{}
            },
            new object[]
            {
                // No assignments
                MatchLocus.Dpb1, "01:01:01:01",
                new object[][]{}
            },
            new object[]
            {
                // Only has expert assignment
                MatchLocus.C, "15:07",
                new[]
                {
                    new object[] {"Cw", "3", SerologySubtype.Broad, true},
                    new object[] {"Cw", "9", SerologySubtype.Split, false},
                    new object[] {"Cw", "10", SerologySubtype.Split, false}
                }
            }
        };
    }
}