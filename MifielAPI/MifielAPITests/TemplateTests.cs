using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MifielAPI;
using MifielAPI.Dao;
using MifielAPI.Objects;
using NUnit.Framework;

namespace MifielAPITests
{
    [TestFixture]
    public class TemplateTests
    {
        const string APP_ID = "9fd550a3866164f48516f71bf85a6258cbd07ff3";
        const string APP_SECRET = "3vIc3r6yJ6PYnssO0S0UYfOqLE70/ulHU76nR/IwHm+zKOa8R+6LuwWeK7HX4vpC7AA8P/ViaixyRJuMb2aftg==";
        const string ROOT_ENDPOINTS = "https://sandbox.mifiel.com";
        private static ApiClient _apiClient;
        private static Templates _templates;
        private static string _content;
        private static string _footer;
        private static Dictionary<string, string> _fields;

        [SetUp]
        public void SetUp()
        {
            _apiClient = new ApiClient(APP_ID, APP_SECRET);
            _templates = new Templates(_apiClient);
            _apiClient.Url = ROOT_ENDPOINTS;
            _fields = new Dictionary<string, string>
                {
                    { "name", "My Client Name" },
                    { "date", System.DateTime.Today.ToShortDateString() }
                };
        }

        [Test]
        public void Templates__Create__ShouldReturnNewTemplate()
        {
            var template_create = GenerateTemplate();
            Assert.NotNull(template_create);
        }

        [Test]
        public void Templates__GetTemplate__ShouldReturnTemplate()
        {
            Templates__Create__ShouldReturnNewTemplate();
            var templates = _templates.FindAll();

            var specific_template = _templates.Find(templates[0].Id);
            Assert.NotNull(specific_template);
        }

        [Test]
        public void Templates__Delete__ShouldRemoveATemplate()
        {
            Templates__Create__ShouldReturnNewTemplate();
            var templates = _templates.FindAll();

            if (templates.Count > 0)
            {
                templates.ForEach((template) => _templates.Delete(template.Id));
                templates = _templates.FindAll();
                Assert.IsTrue(templates.Count == 0);
            }
        }

        [Test]
        public void Templates__GenerateADocumentFromTemplate__ShouldCreateDocument()
        {
            var template = GenerateTemplate();
            var document = CreateDocumentFromTemplate(template);
            var documents = new Documents(_apiClient);

            var dir = Path.GetFullPath(TestContext.CurrentContext.TestDirectory);
            var file = Path.Combine(dir, "prueba.pdf");
            documents.SaveFile(document.Id, file);

            Assert.IsTrue(File.Exists(file));
            Assert.NotNull(document);
        }

        [Test]
        public void Templates__GetFields__ShouldReturnFields()
        {
            var template = GenerateTemplate();
            var fields = _templates.GetFields(template.Id);
            Assert.AreEqual(fields.Count, 2);
        }

        [Test]
        public void Templates__GetDocuments__ShouldReturnFields()
        {
            var template = GenerateTemplate();
            Assert.NotNull(template);
            var document = CreateDocumentFromTemplate(template);
            Assert.NotNull(document);
            var documents = _templates.GetDocuments(template.Id);
            Assert.AreEqual(documents.Count, 1);
            Assert.NotNull(documents[0].Owner);
        }

        [Test]
        public void Templates__CreateSeveralDocuments__ShouldReturnSuccess()
        {
            var template = GenerateTemplate();
            Assert.NotNull(template); 

            var documents = new List<TemplateDocumentsData> {
                new TemplateDocumentsData(){
                        Signatures = new List<Signature>(){
                      new Signature(){
                          Email = "juan+carlos+zavala+lopez@mifiel.com",
                          TaxId = "ZACA850805JX8",
                          SignerName = "Carlos Zavala Lopez"
                      }
                    },
                    Fields = _fields,
                    SignCallbackUrl = "https://www.example.com/webhook/sign",
                    CallbackUrl = "https://www.example.com/webhook/url",
                    ExternalId = Guid.NewGuid().ToString(),
                    ManualClose=false
                },
                new TemplateDocumentsData()
                {
                    Signatures = new List<Signature>(){
                      new Signature(){
                          Email = "juan+carlos+zavala+lopez@mifiel.com",
                          TaxId = "ZACA850805JX8",
                          SignerName = "Carlos Zavala Lopez"
                      }
                    },
                    Fields = _fields,
                    SignCallbackUrl = "https://www.example.com/webhook/sign",
                    CallbackUrl = "https://www.example.com/webhook/url",
                    ExternalId = Guid.NewGuid().ToString(),
                    ManualClose=false
                }
            };

            var templateGenarateDocs = new TemplateGenerateDocuments()
            {
                TemplateId = template.Id,
                Identifier = Guid.NewGuid().ToString(),
                Documents = documents,
                CallbackUrl = "https://www.example.com/webhook/url",
            };

            var results = _templates.GenerateSeveralDocuments(templateGenarateDocs);
            Assert.AreEqual(results.Status, "success");
        }

        private Template GenerateTemplate()
        {
            var content = new StringBuilder("<div>");
            content.AppendLine();
            content.Append("Name <field name='name' type='text'>NAME</field>");
            content.AppendLine();
            content.Append("Date <field name='date' type='text'>DATE</field>");
            content.AppendLine();
            content.Append("</div>");

            _content = content.ToString();
            _footer = "<div>some footer html</div>";

            var template = new Template()
            {
                Name = "New Template-" + Guid.NewGuid().ToString(),
                Description = "Confidential disclosure agreement",
                Header = "<div>some header html</div>",
                Content = _content,
                Footer = _footer
            };

            return _templates.Save(template);
        }

        private Document CreateDocumentFromTemplate(Template template)
        {
            var data = new TemplateDocumentsData()
            {
                Fields = _fields,
                CallbackUrl = "https://www.example.com/webhook/url",
                ExternalId = Guid.NewGuid().ToString(),
                SignCallbackUrl = "https://www.example.com/webhook/sign",
                ManualClose = false,
                Signatures = new List<Signature>(){
                  new Signature(){
                      Email = "juan+carlos+zavala+lopez@mifiel.com",
                      TaxId = "ZACA850805JX8",
                      SignerName = "Carlos Zavala Lopez"
                  }
                }
            };

            var templateGenarateDoc = new TemplateGenerateDocument()
            {
                Id = template.Id,
                Name = "document",
                Data = data
            };

            templateGenarateDoc.Data = data;
            return _templates.GenerateDocument(templateGenarateDoc);
        }
    }
}
