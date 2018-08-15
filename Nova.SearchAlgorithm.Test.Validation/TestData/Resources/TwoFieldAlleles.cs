﻿using System.Collections.Generic;
using Nova.SearchAlgorithm.Common.Models;
using Nova.SearchAlgorithm.Test.Validation.TestData.Models;
using Nova.SearchAlgorithm.Test.Validation.TestData.Models.Hla;

namespace Nova.SearchAlgorithm.Test.Validation.TestData.Resources
{
    /// <summary>
    /// This test data was manually curated from 2-field TGS typed alleles found in the SOLAR database
    /// A corresponding NMDP code was selected for each allele from the DR_ANTIGENS table
    /// (most alleles will correspond to multiple NMDP codes - only one is necessary for testing purposes)
    /// The corresponding serology, p-group, and g-group data was retrieved from the wmda files: hla_nom_g, hla_nom_p, rel_dna_ser (v3330)
    ///
    /// This dataset may be amended, provided all data:
    /// (1) Is a 2-field, TGS typed allele
    /// (2) Has the correct p-group, g-group, and serology associations
    /// (3) Has a valid corresponding nmdp code
    /// </summary>
    public static class TwoFieldAlleles
    {
        public static readonly PhenotypeInfo<List<AlleleTestData>> Alleles = new PhenotypeInfo<List<AlleleTestData>>
        {
            A_1 =
                new List<AlleleTestData>
                {
                    new AlleleTestData {AlleleName = "*01:02", PGroup = "01:01P", GGroup = "01:01:01G", NmdpCode = "*01:BC", Serology = "1"},
                    new AlleleTestData {AlleleName = "*01:06", PGroup = "01:01P", GGroup = "01:01:01G", NmdpCode = "*01:ATJJ", Serology = "1"},
                    new AlleleTestData {AlleleName = "*01:109", PGroup = "01:01P", GGroup = "01:01:01G", NmdpCode = "*01:ABTHU", Serology = "1"},
                    new AlleleTestData {AlleleName = "*01:23", PGroup = "01:01P", GGroup = "01:01:01G", NmdpCode = "*01:DEHR", Serology = "1"},
                    new AlleleTestData {AlleleName = "*01:37", PGroup = "01:01P", GGroup = "01:01:01G", NmdpCode = "*01:HWXW", Serology = "1"},
                    new AlleleTestData {AlleleName = "*01:45", PGroup = "01:01P", GGroup = "01:01:01G", NmdpCode = "*01:HWXW", Serology = "1"},
                    new AlleleTestData {AlleleName = "*01:51", PGroup = "01:01P", GGroup = "01:01:01G", NmdpCode = "*01:PMPX", Serology = "1"},
                    new AlleleTestData {AlleleName = "*02:09", PGroup = "02:01P", GGroup = "02:01:01G", NmdpCode = "*02:EJHG", Serology = "2"},
                    new AlleleTestData {AlleleName = "*02:13", PGroup = "02:01P", GGroup = "02:01:01G", NmdpCode = "*02:EXF", Serology = "2"},
                    new AlleleTestData {AlleleName = "*02:14", PGroup = "02:01P", GGroup = "02:01:01G", NmdpCode = "*02:ZMX", Serology = "2"},
                    new AlleleTestData {AlleleName = "*02:66", PGroup = "02:01P", GGroup = "02:01:01G", NmdpCode = "*02:EJHG", Serology = "2"},
                    new AlleleTestData {AlleleName = "*03:112", PGroup = "03:01P", GGroup = "03:01:01G", NmdpCode = "*03:KEGR", Serology = "3"},
                    new AlleleTestData {AlleleName = "*23:17", PGroup = "23:01P", GGroup = "23:01:01G", NmdpCode = "*23:EWFR", Serology = "23"},
                },
            A_2 = new List<AlleleTestData>
            {
                new AlleleTestData {AlleleName = "*01:23", PGroup = "01:01P", GGroup = "01:01:01G", NmdpCode = "*01:DEHR", Serology = "1"},
                new AlleleTestData {AlleleName = "*02:09", PGroup = "02:01P", GGroup = "02:01:01G", NmdpCode = "*02:EJHG", Serology = "2"},
                new AlleleTestData {AlleleName = "*02:10", PGroup = "02:10P", GGroup = "02:10:01G", NmdpCode = "*02:PYD", Serology = "210"},
                new AlleleTestData {AlleleName = "*02:12", PGroup = "02:06P", GGroup = "02:06:01G", NmdpCode = "*02:AKSA", Serology = "2"},
                new AlleleTestData {AlleleName = "*02:13", PGroup = "02:01P", GGroup = "02:01:01G", NmdpCode = "*02:EXF", Serology = "2"},
                new AlleleTestData {AlleleName = "*02:14", PGroup = "02:01P", GGroup = "02:01:01G", NmdpCode = "*02:ZMX", Serology = "2"},
                new AlleleTestData {AlleleName = "*02:66", PGroup = "02:01P", GGroup = "02:01:01G", NmdpCode = "*02:EJHG", Serology = "2"},
                new AlleleTestData {AlleleName = "*23:17", PGroup = "23:01P", GGroup = "23:01:01G", NmdpCode = "*23:EWFR", Serology = "23"},
                new AlleleTestData {AlleleName = "*23:18", PGroup = "23:01P", GGroup = "23:01:01G", NmdpCode = "*23:EWFR", Serology = "23"},
            },
            B_1 =
                new List<AlleleTestData>
                {
                    new AlleleTestData {AlleleName = "*07:16", PGroup = "07:02P", GGroup = "07:02:01G", NmdpCode = "*07:CMVB", Serology = "7"},
                    new AlleleTestData {AlleleName = "*08:18", PGroup = "08:01P", GGroup = "08:01:01G", NmdpCode = "*08:NWWN", Serology = "8"},
                    new AlleleTestData {AlleleName = "*15:12", PGroup = "15:12P", GGroup = "15:12:01G", NmdpCode = "*15:AND", Serology = "76"},
                    new AlleleTestData {AlleleName = "*15:14", PGroup = "15:01P", GGroup = "15:01:01G", NmdpCode = "*15:WMK", Serology = "76"},
                    new AlleleTestData {AlleleName = "*15:146", PGroup = "15:01P", GGroup = "15:01:01G", NmdpCode = "*15:GUAD", Serology = "62"},
                    new AlleleTestData {AlleleName = "*15:19", PGroup = "15:12P", GGroup = "15:12:01G", NmdpCode = "*15:WMK", Serology = "76"},
                    new AlleleTestData {AlleleName = "*15:228", PGroup = "15:01P", GGroup = "15:01:01G", NmdpCode = "*15:RZJS", Serology = "15"},
                    new AlleleTestData {AlleleName = "*15:35", PGroup = "15:02P", GGroup = "15:02:01G", NmdpCode = "*15:AWUD", Serology = "62"},
                },
            B_2 = new List<AlleleTestData>
            {
                new AlleleTestData {AlleleName = "*08:182", PGroup = "08:01P", GGroup = "08:01:01G", NmdpCode = "*08:AXHCG", Serology = "8"},
                new AlleleTestData {AlleleName = "*15:146", PGroup = "15:01P", GGroup = "15:01:01G", NmdpCode = "*15:GUAD", Serology = "62"},
                new AlleleTestData {AlleleName = "*15:228", PGroup = "15:01P", GGroup = "15:01:01G", NmdpCode = "*15:RZJS", Serology = "15"},
                new AlleleTestData {AlleleName = "*15:35", PGroup = "15:02P", GGroup = "15:02:01G", NmdpCode = "*15:AWUD", Serology = "62"},
            },
            C_1 =
                new List<AlleleTestData>
                {
                    new AlleleTestData {AlleleName = "*01:03", PGroup = "01:02P", GGroup = "01:02:01G", NmdpCode = "*01:AHC", Serology = "1"},
                    new AlleleTestData {AlleleName = "*01:05", PGroup = "01:02P", GGroup = "01:02:01G", NmdpCode = "*01:ADSN", Serology = "1"},
                    new AlleleTestData {AlleleName = "*01:44", PGroup = "01:02P", GGroup = "01:02:01G", NmdpCode = "*01:AWTXA", Serology = "1"},
                    new AlleleTestData {AlleleName = "*03:14", PGroup = "03:02P", GGroup = "03:02:01G", NmdpCode = "*03:EEJ", Serology = "3"},
                    new AlleleTestData {AlleleName = "*03:32", PGroup = "03:03P", GGroup = "03:03:01G", NmdpCode = "*03:UACS", Serology = "3"},
                    new AlleleTestData {AlleleName = "*04:82", PGroup = "04:01P", GGroup = "04:01:01G", NmdpCode = "*04:NYYT", Serology = "4"},
                    new AlleleTestData {AlleleName = "*05:37", PGroup = "05:01P", GGroup = "05:01:01G", NmdpCode = "*05:HUEP", Serology = "5"},
                    new AlleleTestData {AlleleName = "*05:53", PGroup = "05:01P", GGroup = "05:01:01G", NmdpCode = "*05:RJFW", Serology = "5"},
                    new AlleleTestData {AlleleName = "*06:14", PGroup = "06:02P", GGroup = "06:02:01G", NmdpCode = "*06:AMZCU", Serology = "6"},
                    new AlleleTestData {AlleleName = "*07:18", PGroup = "07:01P", GGroup = "07:01:01G", NmdpCode = "*07:BBTA", Serology = "7"},
                },
            C_2 = new List<AlleleTestData>
            {
                new AlleleTestData {AlleleName = "*03:14", PGroup = "03:02P", GGroup = "03:02:01G", NmdpCode = "*03:EEJ", Serology = "3"},
                new AlleleTestData {AlleleName = "*04:82", PGroup = "04:01P", GGroup = "04:01:01G", NmdpCode = "*04:NYYT", Serology = "4"},
                new AlleleTestData {AlleleName = "*05:37", PGroup = "05:01P", GGroup = "05:01:01G", NmdpCode = "*05:HUEP", Serology = "5"},
                new AlleleTestData {AlleleName = "*06:11", PGroup = "06:02P", GGroup = "06:02:01G", NmdpCode = "*06:BPNT", Serology = "6"},
                new AlleleTestData {AlleleName = "*07:18", PGroup = "07:01P", GGroup = "07:01:01G", NmdpCode = "*07:BBTA", Serology = "7"},
            },
            DPB1_1 = new List<AlleleTestData> { },
            DPB1_2 = new List<AlleleTestData> { },
            DQB1_1 =
                new List<AlleleTestData>
                {
                    new AlleleTestData {AlleleName = "*02:03", PGroup = "02:01P", GGroup = "02:01:01G", NmdpCode = "*02:WHP", Serology = "2"},
                    new AlleleTestData {AlleleName = "*03:09", PGroup = "03:01P", GGroup = "03:01:01G", NmdpCode = "*03:VDUX", Serology = "3"},
                    new AlleleTestData {AlleleName = "*03:18", PGroup = "03:01P", GGroup = "03:01:01G", NmdpCode = "*03:CZBZ", Serology = "3"},
                    new AlleleTestData {AlleleName = "*03:191", PGroup = "03:01P", GGroup = "03:01:01G", NmdpCode = "*03:AUXZT", Serology = "3"},
                    new AlleleTestData {AlleleName = "*05:10", PGroup = "05:01P", GGroup = "05:01:01G", NmdpCode = "*05:ACSRM", Serology = "5"},
                    new AlleleTestData {AlleleName = "*05:103", PGroup = "05:01P", GGroup = "05:01:01G", NmdpCode = "*05:ASXHK", Serology = "5"},
                    new AlleleTestData {AlleleName = "*05:107", PGroup = "05:01P", GGroup = "05:01:01G", NmdpCode = "*05:ASXHK", Serology = "5"},
                    new AlleleTestData {AlleleName = "*06:110", PGroup = "06:03P", GGroup = "06:03:01G", NmdpCode = "*06:ACSRM", Serology = "6"},
                    new AlleleTestData {AlleleName = "*06:12", PGroup = "06:02P", GGroup = "06:02:01G", NmdpCode = "*06:ANNV", Serology = "1"},
                    new AlleleTestData {AlleleName = "*06:20", PGroup = "06:01P", GGroup = "06:01:01G", NmdpCode = "*06:ANNV", Serology = "6"},
                    new AlleleTestData {AlleleName = "*06:39", PGroup = "06:04P", GGroup = "06:04:01G", NmdpCode = "*06:ACSRM", Serology = "6"},
                    new AlleleTestData {AlleleName = "*06:41", PGroup = "06:03P", GGroup = "06:03:01G", NmdpCode = "*06:ACSRM", Serology = "6"},
                    new AlleleTestData {AlleleName = "*06:88", PGroup = "06:09P", GGroup = "06:09:01G", NmdpCode = "*06:ACSRM", Serology = "6"},
                },
            DQB1_2 = new List<AlleleTestData>
            {
                new AlleleTestData {AlleleName = "*02:03", PGroup = "02:01P", GGroup = "02:01:01G", NmdpCode = "*02:WHP", Serology = "2"},
                new AlleleTestData {AlleleName = "*02:04", PGroup = "02:01P", GGroup = "02:01:01G", NmdpCode = "*02:YNVM", Serology = "2"},
                new AlleleTestData {AlleleName = "*03:09", PGroup = "03:01P", GGroup = "03:01:01G", NmdpCode = "*03:VDUX", Serology = "3"},
                new AlleleTestData {AlleleName = "*03:11", PGroup = "03:01P", GGroup = "03:01:01G", NmdpCode = "*03:VBH", Serology = "3"},
                new AlleleTestData {AlleleName = "*03:243", PGroup = "03:01P", GGroup = "03:01:01G", NmdpCode = "*03:AUXZT", Serology = "3"},
                new AlleleTestData {AlleleName = "*05:103", PGroup = "05:01P", GGroup = "05:01:01G", NmdpCode = "*05:ASXHK", Serology = "5"},
                new AlleleTestData {AlleleName = "*06:110", PGroup = "06:03P", GGroup = "06:03:01G", NmdpCode = "*06:ACSRM", Serology = "6"},
                new AlleleTestData {AlleleName = "*06:41", PGroup = "06:03P", GGroup = "06:03:01G", NmdpCode = "*06:ACSRM", Serology = "6"},
                new AlleleTestData {AlleleName = "*06:88", PGroup = "06:09P", GGroup = "06:09:01G", NmdpCode = "*06:ACSRM", Serology = "6"},
            },
            DRB1_1 =
                new List<AlleleTestData>
                {
                    new AlleleTestData {AlleleName = "*03:124", PGroup = "03:01P", GGroup = "03:01:01G", NmdpCode = "*03:AMMSZ", Serology = "3"},
                    new AlleleTestData {AlleleName = "*11:129", PGroup = "11:06P", GGroup = "11:06:01G", NmdpCode = "*11:ACSSH", Serology = "11"},
                },
            DRB1_2 = new List<AlleleTestData>
            {
                new AlleleTestData {AlleleName = "*03:124", PGroup = "03:01P", GGroup = "03:01:01G", NmdpCode = "*03:AMMSZ", Serology = "3"},
                new AlleleTestData {AlleleName = "*11:129", PGroup = "11:06P", GGroup = "11:06:01G", NmdpCode = "*11:ACSSH", Serology = "11"},
                new AlleleTestData {AlleleName = "*11:198", PGroup = "11:04P", GGroup = "11:04:01G", NmdpCode = "*11:ASEVD", Serology = "11"},
            },
        };
    }
}