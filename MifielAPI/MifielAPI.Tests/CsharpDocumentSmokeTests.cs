using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using MifielAPI;
using MifielAPI.Dao;
using MifielAPI.Objects;
using MifielAPI.Utils;

namespace MifielApiTests
{
    [TestFixture]
    [Category("Smoke")]
    public class CsharpDocumentSmokeTests
    {
        const string DefaultAppId = "7c938f8e2ff2f083d454127bffd7d4c8bc2c2dee";
        const string DefaultAppSecret = "1lXeVVMbNe5IH+INgMeQX463fsixeWEMMSLS4WXPwxpR9RwStD3iE4XaNMXbY8YigIxCQP9gb/8xZI3XILN2Rw==";
        const string DefaultBaseUrl = "https://app-sandbox.mifiel.com";

        private readonly string _currentDirectory = Path.GetFullPath(TestContext.CurrentContext.TestDirectory);

        [Test]
        public void CreateDocument_ShouldReturnADocumentWithId()
        {
            var appId = FirstNonEmpty(Environment.GetEnvironmentVariable("MIFIEL_APP_ID"), DefaultAppId);
            var appSecret = FirstNonEmpty(Environment.GetEnvironmentVariable("MIFIEL_APP_SECRET"), DefaultAppSecret);
            var baseUrl = FirstNonEmpty(Environment.GetEnvironmentVariable("MIFIEL_BASE_URL"), DefaultBaseUrl);
            var pdfFilePath = Path.Combine(_currentDirectory, "csharp-test-pdf.pdf");

            Assert.IsTrue(File.Exists(pdfFilePath), "Expected fixture PDF at " + pdfFilePath);

            var apiClient = new ApiClient(appId, appSecret) { Url = baseUrl };
            var docs = new Documents(apiClient);

            var document = new Document()
            {
                File = pdfFilePath,
                ManualClose = false,
                SendMail = false,
                SendInvites = false,
                Signatures = new List<Signature>()
                {
                    new Signature()
                    {
                        Email = "smoke-test-" + Guid.NewGuid().ToString("N").Substring(0, 8) + "@mifiel.com",
                        SignerName = "Smoke Test Signer"
                    }
                }
            };

            document = docs.Save(document);

            Assert.IsNotNull(document);
            Assert.IsFalse(string.IsNullOrEmpty(document.Id), "Created document should have an id");

            var newtonsoft = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(assembly => assembly.GetName().Name == "Newtonsoft.Json");
            Assert.IsNotNull(newtonsoft, "Newtonsoft.Json should be loaded when deserializing the document response");
            Assert.AreEqual(13, newtonsoft.GetName().Version.Major,
                "Expected Newtonsoft.Json 13.x after the dependabot bump, got " + newtonsoft.GetName().Version);

            var json = MifielUtils.ConvertObjectToJson(document);
            var parsedFromJson = MifielUtils.ConvertJsonToObject<Document>(json);
            Assert.AreEqual(document.Id, parsedFromJson.Id, "JSON round-trip with Newtonsoft.Json should preserve id");
            StringAssert.Contains("\"" + document.Id + "\"", json);

            Console.WriteLine("CREATED_DOCUMENT_ID=" + parsedFromJson.Id);
            Console.WriteLine("NEWTONSOFT_JSON_VERSION=" + newtonsoft.GetName().Version);
            TestContext.WriteLine("CREATED_DOCUMENT_ID=" + parsedFromJson.Id);
        }

        private static string FirstNonEmpty(string preferred, string fallback)
        {
            return string.IsNullOrEmpty(preferred) ? fallback : preferred;
        }
    }
}
