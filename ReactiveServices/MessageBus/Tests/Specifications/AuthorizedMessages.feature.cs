﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.42000
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace ReactiveServices.MessageBus.Tests.Specifications
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Authorized Messages")]
    public partial class AuthorizedMessagesFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "AuthorizedMessages.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("pt-BR"), "Authorized Messages", "Como um trocador de mensagens\nDesejo que determinados tipos de mensagens tenham s" +
                    "eu envio ou recebimento autorizados por um serviço de autorização\nPara que eu po" +
                    "ssa manter a troca de mensagens confiável em casos críticos de segurança", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
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
        [NUnit.Framework.DescriptionAttribute("Impedir prosseguimento de um envio ou recebimento de mensagem caso o arquivo de c" +
            "onfigurações esteja ausente")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("fast")]
        public virtual void ImpedirProsseguimentoDeUmEnvioOuRecebimentoDeMensagemCasoOArquivoDeConfiguracoesEstejaAusente()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Impedir prosseguimento de um envio ou recebimento de mensagem caso o arquivo de c" +
                    "onfigurações esteja ausente", new string[] {
                        "stable",
                        "fast"});
#line 9
this.ScenarioSetup(scenarioInfo);
#line 10
 testRunner.Given("que o arquivo de configuração de autorização de mensagens não exista", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 11
 testRunner.When("a operação de envio ou recebimento de mensagens for executada", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 12
 testRunner.Then("a operação deve ser abortada com erro devido à ausência do arquivo de configuraçã" +
                    "o de autorização de mensagens", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Carregar configurações do arquivo")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("fast")]
        public virtual void CarregarConfiguracoesDoArquivo()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Carregar configurações do arquivo", new string[] {
                        "stable",
                        "fast"});
#line 15
this.ScenarioSetup(scenarioInfo);
#line 16
 testRunner.Given("que o arquivo de configuração de autorização de mensagens exista", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 17
 testRunner.When("a operação de envio ou recebimento de mensagens for executada", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 18
 testRunner.Then("o arquivo de configuração de autorização de mensagens deve ser carregado para mem" +
                    "ória e considerado antes de se executar o envio ou recebimento", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Autorizar envio e recebimento de mensagens devido ao paramentro AllowByDefault")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("fast")]
        public virtual void AutorizarEnvioERecebimentoDeMensagensDevidoAoParamentroAllowByDefault()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Autorizar envio e recebimento de mensagens devido ao paramentro AllowByDefault", new string[] {
                        "stable",
                        "fast"});
#line 21
this.ScenarioSetup(scenarioInfo);
#line 22
 testRunner.Given("que o arquivo de configuração de autorização de mensagens exista", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 23
 testRunner.And("que o arquivo de configuração de autorização de mensagens informe que mensagens n" +
                    "ão descriminadas devem ser autorizadas", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 24
 testRunner.And("uma mensagem a ser enviada ou recebida for uma mensagem não discriminada no arqui" +
                    "vo de configuração de autorização de mensagens", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 25
 testRunner.When("a operação de envio ou recebimento de mensagens for executada", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 26
 testRunner.Then("o envio ou recebimento deve ser realizado com sucesso", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Negar envio e recebimento de mensagens devido ao paramentro DenyByDefault")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("fast")]
        public virtual void NegarEnvioERecebimentoDeMensagensDevidoAoParamentroDenyByDefault()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Negar envio e recebimento de mensagens devido ao paramentro DenyByDefault", new string[] {
                        "stable",
                        "fast"});
#line 29
this.ScenarioSetup(scenarioInfo);
#line 30
 testRunner.Given("que o arquivo de configuração de autorização de mensagens exista", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 31
 testRunner.And("que o arquivo de configuração de autorização de mensagens informe que mensagens n" +
                    "ão descriminadas não devem ser autorizadas", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 32
 testRunner.And("uma mensagem a ser enviada ou recebida for uma mensagem não discriminada no arqui" +
                    "vo de configuração de autorização de mensagens", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 33
 testRunner.When("a operação de envio ou recebimento de mensagens for executada", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 34
 testRunner.Then("o envio ou recebimento não deve ser realizado", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Autorizar envio e recebimento de mensagens devido a resposta positiva de um servi" +
            "ço de autorização")]
        [NUnit.Framework.CategoryAttribute("unstable")]
        [NUnit.Framework.CategoryAttribute("fast")]
        public virtual void AutorizarEnvioERecebimentoDeMensagensDevidoARespostaPositivaDeUmServicoDeAutorizacao()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Autorizar envio e recebimento de mensagens devido a resposta positiva de um servi" +
                    "ço de autorização", new string[] {
                        "unstable",
                        "fast"});
#line 37
this.ScenarioSetup(scenarioInfo);
#line 38
 testRunner.Given("que o arquivo de configuração de autorização de mensagens exista", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 39
 testRunner.And("uma mensagem a ser enviada ou recebida for uma mensagem discriminada no arquivo d" +
                    "e configuração de autorização de mensagens", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 40
 testRunner.And("o serviço de autorização de mensagens estiver configurado para autorizar o envio " +
                    "ou recebimento da mensagem", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 41
 testRunner.When("a operação de envio ou recebimento de mensagens for executada", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 42
 testRunner.Then("o envio ou recebimento deve ser realizado com sucesso", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Negar envio e recebimento de mensagens devido a resposta negativa de um serviço d" +
            "e autorização")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("fast")]
        public virtual void NegarEnvioERecebimentoDeMensagensDevidoARespostaNegativaDeUmServicoDeAutorizacao()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Negar envio e recebimento de mensagens devido a resposta negativa de um serviço d" +
                    "e autorização", new string[] {
                        "stable",
                        "fast"});
#line 45
this.ScenarioSetup(scenarioInfo);
#line 46
 testRunner.Given("que o arquivo de configuração de autorização de mensagens exista", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 47
 testRunner.And("uma mensagem a ser enviada ou recebida for uma mensagem discriminada no arquivo d" +
                    "e configuração de autorização de mensagens", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 48
 testRunner.And("o serviço de autorização de mensagens estiver configurado para não autorizar o en" +
                    "vio ou recebimento da mensagem", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 49
 testRunner.When("a operação de envio ou recebimento de mensagens for executada", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 50
 testRunner.Then("o envio ou recebimento não deve ser realizado", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
