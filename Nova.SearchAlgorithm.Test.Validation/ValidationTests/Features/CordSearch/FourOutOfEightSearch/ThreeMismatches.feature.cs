﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:2.3.2.0
//      SpecFlow Generator Version:2.3.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Nova.SearchAlgorithm.Test.Validation.ValidationTests.Features.CordSearch.FourOutOfEightSearch
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.3.2.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Four out of eight Search - three mismatches")]
    public partial class FourOutOfEightSearch_ThreeMismatchesFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "ThreeMismatches.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Four out of eight Search - three mismatches", "  As a member of the search team\r\n  I want to be able to run a 4/8 cord search\r\n " +
                    " And see results with three mismatches", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("4/8 Search with two mismatches at A, and one at B")]
        public virtual void _48SearchWithTwoMismatchesAtAAndOneAtB()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("4/8 Search with two mismatches at A, and one at B", ((string[])(null)));
#line 6
  this.ScenarioSetup(scenarioInfo);
#line 7
    testRunner.Given("a patient and a donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 8
    testRunner.And("the donor has a double mismatch at locus A", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 9
    testRunner.And("the donor has a single mismatch at locus B", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 10
    testRunner.And("the donor is of type cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 11
    testRunner.And("the search type is cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 12
    testRunner.When("I run a 4/8 search", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 13
    testRunner.Then("the results should contain the specified donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("4/8 Search with two mismatches at A, and one at DRB1")]
        public virtual void _48SearchWithTwoMismatchesAtAAndOneAtDRB1()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("4/8 Search with two mismatches at A, and one at DRB1", ((string[])(null)));
#line 15
  this.ScenarioSetup(scenarioInfo);
#line 16
    testRunner.Given("a patient and a donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 17
    testRunner.And("the donor has a double mismatch at locus A", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 18
    testRunner.And("the donor has a single mismatch at locus DRB1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 19
    testRunner.And("the donor is of type cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 20
    testRunner.And("the search type is cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 21
    testRunner.When("I run a 4/8 search", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 22
    testRunner.Then("the results should contain the specified donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("4/8 Search with two mismatches at A, and one at C")]
        public virtual void _48SearchWithTwoMismatchesAtAAndOneAtC()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("4/8 Search with two mismatches at A, and one at C", ((string[])(null)));
#line 24
  this.ScenarioSetup(scenarioInfo);
#line 25
    testRunner.Given("a patient and a donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 26
    testRunner.And("the donor has a double mismatch at locus A", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 27
    testRunner.And("the donor has a single mismatch at locus C", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 28
    testRunner.And("the donor is of type cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 29
    testRunner.And("the search type is cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 30
    testRunner.When("I run a 4/8 search", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 31
    testRunner.Then("the results should contain the specified donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("4/8 Search with two mismatches at B, and one at A")]
        public virtual void _48SearchWithTwoMismatchesAtBAndOneAtA()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("4/8 Search with two mismatches at B, and one at A", ((string[])(null)));
#line 33
  this.ScenarioSetup(scenarioInfo);
#line 34
    testRunner.Given("a patient and a donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 35
    testRunner.And("the donor has a double mismatch at locus B", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 36
    testRunner.And("the donor has a single mismatch at locus A", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 37
    testRunner.And("the donor is of type cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 38
    testRunner.And("the search type is cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 39
    testRunner.When("I run a 4/8 search", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 40
    testRunner.Then("the results should contain the specified donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("4/8 Search with two mismatches at B, and one at DRB1")]
        public virtual void _48SearchWithTwoMismatchesAtBAndOneAtDRB1()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("4/8 Search with two mismatches at B, and one at DRB1", ((string[])(null)));
#line 42
  this.ScenarioSetup(scenarioInfo);
#line 43
    testRunner.Given("a patient and a donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 44
    testRunner.And("the donor has a double mismatch at locus B", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 45
    testRunner.And("the donor has a single mismatch at locus DRB1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 46
    testRunner.And("the donor is of type cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 47
    testRunner.And("the search type is cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 48
    testRunner.When("I run a 4/8 search", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 49
    testRunner.Then("the results should contain the specified donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("4/8 Search with two mismatches at B, and one at C")]
        public virtual void _48SearchWithTwoMismatchesAtBAndOneAtC()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("4/8 Search with two mismatches at B, and one at C", ((string[])(null)));
#line 51
  this.ScenarioSetup(scenarioInfo);
#line 52
    testRunner.Given("a patient and a donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 53
    testRunner.And("the donor has a double mismatch at locus B", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 54
    testRunner.And("the donor has a single mismatch at locus C", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 55
    testRunner.And("the donor is of type cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 56
    testRunner.And("the search type is cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 57
    testRunner.When("I run a 4/8 search", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 58
    testRunner.Then("the results should contain the specified donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("4/8 Search with two mismatches at DRB1, and one at A")]
        public virtual void _48SearchWithTwoMismatchesAtDRB1AndOneAtA()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("4/8 Search with two mismatches at DRB1, and one at A", ((string[])(null)));
#line 60
  this.ScenarioSetup(scenarioInfo);
#line 61
    testRunner.Given("a patient and a donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 62
    testRunner.And("the donor has a double mismatch at locus DRB1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 63
    testRunner.And("the donor has a single mismatch at locus A", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 64
    testRunner.And("the donor is of type cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 65
    testRunner.And("the search type is cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 66
    testRunner.When("I run a 4/8 search", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 67
    testRunner.Then("the results should contain the specified donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("4/8 Search with two mismatches at DRB1, and one at B")]
        public virtual void _48SearchWithTwoMismatchesAtDRB1AndOneAtB()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("4/8 Search with two mismatches at DRB1, and one at B", ((string[])(null)));
#line 69
  this.ScenarioSetup(scenarioInfo);
#line 70
    testRunner.Given("a patient and a donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 71
    testRunner.And("the donor has a double mismatch at locus DRB1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 72
    testRunner.And("the donor has a single mismatch at locus B", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 73
    testRunner.And("the donor is of type cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 74
    testRunner.And("the search type is cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 75
    testRunner.When("I run a 4/8 search", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 76
    testRunner.Then("the results should contain the specified donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("4/8 Search with two mismatches at DRB1, and one at C")]
        public virtual void _48SearchWithTwoMismatchesAtDRB1AndOneAtC()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("4/8 Search with two mismatches at DRB1, and one at C", ((string[])(null)));
#line 78
  this.ScenarioSetup(scenarioInfo);
#line 79
    testRunner.Given("a patient and a donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 80
    testRunner.And("the donor has a double mismatch at locus DRB1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 81
    testRunner.And("the donor has a single mismatch at locus C", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 82
    testRunner.And("the donor is of type cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 83
    testRunner.And("the search type is cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 84
    testRunner.When("I run a 4/8 search", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 85
    testRunner.Then("the results should contain the specified donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("4/8 Search with two mismatches at C, and one at A")]
        public virtual void _48SearchWithTwoMismatchesAtCAndOneAtA()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("4/8 Search with two mismatches at C, and one at A", ((string[])(null)));
#line 87
  this.ScenarioSetup(scenarioInfo);
#line 88
    testRunner.Given("a patient and a donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 89
    testRunner.And("the donor has a double mismatch at locus C", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 90
    testRunner.And("the donor has a single mismatch at locus A", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 91
    testRunner.And("the donor is of type cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 92
    testRunner.And("the search type is cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 93
    testRunner.When("I run a 4/8 search", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 94
    testRunner.Then("the results should contain the specified donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("4/8 Search with two mismatches at C, and one at B")]
        public virtual void _48SearchWithTwoMismatchesAtCAndOneAtB()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("4/8 Search with two mismatches at C, and one at B", ((string[])(null)));
#line 96
  this.ScenarioSetup(scenarioInfo);
#line 97
    testRunner.Given("a patient and a donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 98
    testRunner.And("the donor has a double mismatch at locus C", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 99
    testRunner.And("the donor has a single mismatch at locus B", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 100
    testRunner.And("the donor is of type cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 101
    testRunner.And("the search type is cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 102
    testRunner.When("I run a 4/8 search", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 103
    testRunner.Then("the results should contain the specified donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("4/8 Search with two mismatches at C, and one at DRB1")]
        public virtual void _48SearchWithTwoMismatchesAtCAndOneAtDRB1()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("4/8 Search with two mismatches at C, and one at DRB1", ((string[])(null)));
#line 105
  this.ScenarioSetup(scenarioInfo);
#line 106
    testRunner.Given("a patient and a donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 107
    testRunner.And("the donor has a double mismatch at locus C", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 108
    testRunner.And("the donor has a single mismatch at locus DRB1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 109
    testRunner.And("the donor is of type cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 110
    testRunner.And("the search type is cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 111
    testRunner.When("I run a 4/8 search", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 112
    testRunner.Then("the results should contain the specified donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("4/8 Search with one mismatch each at locus A, B, and C")]
        public virtual void _48SearchWithOneMismatchEachAtLocusABAndC()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("4/8 Search with one mismatch each at locus A, B, and C", ((string[])(null)));
#line 114
  this.ScenarioSetup(scenarioInfo);
#line 115
    testRunner.Given("a patient and a donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 116
    testRunner.And("the donor has a single mismatch at locus A", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 117
    testRunner.And("the donor has a single mismatch at locus B", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 118
    testRunner.And("the donor has a single mismatch at locus C", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 119
    testRunner.And("the donor is of type cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 120
    testRunner.And("the search type is cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 121
    testRunner.When("I run a 4/8 search", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 122
    testRunner.Then("the results should contain the specified donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("4/8 Search with one mismatch each at locus A, B, and DRB1")]
        public virtual void _48SearchWithOneMismatchEachAtLocusABAndDRB1()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("4/8 Search with one mismatch each at locus A, B, and DRB1", ((string[])(null)));
#line 124
  this.ScenarioSetup(scenarioInfo);
#line 125
    testRunner.Given("a patient and a donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 126
    testRunner.And("the donor has a single mismatch at locus A", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 127
    testRunner.And("the donor has a single mismatch at locus B", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 128
    testRunner.And("the donor has a single mismatch at locus DRB1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 129
    testRunner.And("the donor is of type cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 130
    testRunner.And("the search type is cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 131
    testRunner.When("I run a 4/8 search", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 132
    testRunner.Then("the results should contain the specified donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("4/8 Search with one mismatch each at locus A, C, and DRB1")]
        public virtual void _48SearchWithOneMismatchEachAtLocusACAndDRB1()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("4/8 Search with one mismatch each at locus A, C, and DRB1", ((string[])(null)));
#line 134
  this.ScenarioSetup(scenarioInfo);
#line 135
    testRunner.Given("a patient and a donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 136
    testRunner.And("the donor has a single mismatch at locus A", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 137
    testRunner.And("the donor has a single mismatch at locus C", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 138
    testRunner.And("the donor has a single mismatch at locus DRB1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 139
    testRunner.And("the donor is of type cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 140
    testRunner.And("the search type is cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 141
    testRunner.When("I run a 4/8 search", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 142
    testRunner.Then("the results should contain the specified donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("4/8 Search with one mismatch each at locus B, C, and DRB1")]
        public virtual void _48SearchWithOneMismatchEachAtLocusBCAndDRB1()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("4/8 Search with one mismatch each at locus B, C, and DRB1", ((string[])(null)));
#line 144
  this.ScenarioSetup(scenarioInfo);
#line 145
    testRunner.Given("a patient and a donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 146
    testRunner.And("the donor has a single mismatch at locus B", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 147
    testRunner.And("the donor has a single mismatch at locus C", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 148
    testRunner.And("the donor has a single mismatch at locus DRB1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 149
    testRunner.And("the donor is of type cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 150
    testRunner.And("the search type is cord", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 151
    testRunner.When("I run a 4/8 search", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 152
    testRunner.Then("the results should contain the specified donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
