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
namespace Nova.SearchAlgorithm.Test.Validation.ValidationTests.Features.NullAlleles
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.3.2.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Ten Out Of Ten Search - Null Alleles - patient has single null allele")]
    public partial class TenOutOfTenSearch_NullAlleles_PatientHasSingleNullAlleleFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "PatientHasSingleNull.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Ten Out Of Ten Search - Null Alleles - patient has single null allele", "  As a member of the search team\r\n  When patient has a single null allele\r\n  I wa" +
                    "nt to be able to run a search when the patient or donor has a potential null all" +
                    "ele", ProgrammingLanguage.CSharp, ((string[])(null)));
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
        [NUnit.Framework.DescriptionAttribute("Null Alleles - Same typing")]
        public virtual void NullAlleles_SameTyping()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Null Alleles - Same typing", ((string[])(null)));
#line 6
  this.ScenarioSetup(scenarioInfo);
#line 7
    testRunner.Given("a patient has a match", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 8
    testRunner.And("the donor has a null allele at locus A at position 1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 9
    testRunner.When("I run a 10/10 search", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 10
    testRunner.Then("the results should contain the specified donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Null Alleles - Same expressing allele, different null allele")]
        public virtual void NullAlleles_SameExpressingAlleleDifferentNullAllele()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Null Alleles - Same expressing allele, different null allele", ((string[])(null)));
#line 12
  this.ScenarioSetup(scenarioInfo);
#line 13
    testRunner.Given("a patient has a match", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 14
    testRunner.And("the donor has a null allele at locus A at position 1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 15
    testRunner.And("the patient has a different null allele at locus A at position 1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 16
    testRunner.When("I run a 10/10 search", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 17
    testRunner.Then("the results should contain the specified donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Null Alleles - Homozygous donor")]
        public virtual void NullAlleles_HomozygousDonor()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Null Alleles - Homozygous donor", ((string[])(null)));
#line 19
  this.ScenarioSetup(scenarioInfo);
#line 20
    testRunner.Given("a patient has a match", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 21
    testRunner.And("the donor is homozygous at locus A", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 22
    testRunner.And("the patient has a null allele at locus A at position 1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 23
    testRunner.When("I run a 10/10 search", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 24
    testRunner.Then("the results should contain the specified donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Null Alleles - Same null allele, different expressing allele")]
        public virtual void NullAlleles_SameNullAlleleDifferentExpressingAllele()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Null Alleles - Same null allele, different expressing allele", ((string[])(null)));
#line 26
  this.ScenarioSetup(scenarioInfo);
#line 27
    testRunner.Given("a patient has a match", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 28
    testRunner.And("the donor has a null allele at locus A at position 1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 29
    testRunner.And("the donor has a mismatch at locus A at position 2", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 30
    testRunner.When("I run a 9/10 search at locus A", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 31
    testRunner.Then("the results should not contain the specified donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Null Alleles - Different null allele, different expressing allele")]
        public virtual void NullAlleles_DifferentNullAlleleDifferentExpressingAllele()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Null Alleles - Different null allele, different expressing allele", ((string[])(null)));
#line 33
  this.ScenarioSetup(scenarioInfo);
#line 34
    testRunner.Given("a patient has a match", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 35
    testRunner.And("the donor has a null allele at locus A at position 1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 36
    testRunner.And("the donor has a mismatch at locus A at position 2", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 37
    testRunner.And("the patient has a different null allele at locus A at position 1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 38
    testRunner.When("I run a 9/10 search at locus A", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 39
    testRunner.Then("the results should not contain the specified donor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
