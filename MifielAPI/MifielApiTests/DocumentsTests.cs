using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using MifielAPI;
using MifielAPI.Objects;
using MifielAPI.Dao;

namespace MifielApiTests
{
    [TestFixture]
    public class DocumentsTests
    {
        private static ApiClient _apiClient;
        private static Documents _docs;
        private static string _pdfFilePath;

        private readonly string _currentDirectory = Path.GetFullPath(TestContext.CurrentContext.TestDirectory);

        [SetUp]
        public void SetUp()
        {
            string appId = "bf8a81b18bffbacf70aa4ac9d5e496a2b7e0e6dd";
            string appSecret = "hBVLKRTyat56vHcNPVsrSm4ItzmSuMBCrJybLUsX7iH4FWxXg6P2c0g3mdl1SYpeGysHJe2vZDN/RM1nw9tXiw==";

            _pdfFilePath = Path.Combine(_currentDirectory, "test-pdf.pdf");
            _apiClient = new ApiClient(appId, appSecret);
            _docs = new Documents(_apiClient);
        }

        [Test]
        public void Documents__WrongUrl__ShouldThrowAnException()
        {
            Assert.Throws<MifielAPI.Exceptions.MifielException>(() => _apiClient.Url = "www.google.com");
        }

        [Test]
        public void Documents__CorrectUrl__ShouldNotThrowAnException()
        {
            _apiClient.Url = "https://sandbox.mifiel.com";
        }

        [Test]
        public void Documents__FindAllDocuments__ShouldReturnAList()
        {
            SetSandboxUrl();
            var allDocuments = _docs.FindAll();
            Assert.IsNotNull(allDocuments);
        }

        [Test]
        public void Documents__Close__Should_Success()
        {
            SetSandboxUrl();
            var docId = _docs.FindAll()[0].Id;
            var closeDocument = _docs.Close(docId);
            Assert.IsTrue(closeDocument.Success);
        }

        [Test]
        public void Documents__SaveWithFilePath__ShouldReturnADocument()
        {
            SetSandboxUrl();
            var document = new Document()
            {
                File = Path.Combine(_currentDirectory, _pdfFilePath),
                ManualClose = false,
                CallbackUrl = "https://requestb.in/1cuddmz1"
            };

            var signatures = new List<Signature>(){
                new Signature(){
                    Email = "ja.zavala.aguilar@gmail.com",
                    TaxId = "ZAAJ8301061E0",
                    SignerName = "Juan Antonio Zavala Aguilar"
                }
            };

            document.Signatures = signatures;
            document = _docs.Save(document);
            Assert.IsNotNull(document);
        }

        [Test]
        public void Documents__AppendPDFBase64InOriginalXml__ShouldGenerateNewXML()
        {
            var pathOriginalXml = Path.Combine(_currentDirectory, "file_hash.xml");
            var pathNewXml = Path.Combine(_currentDirectory, "file_with_hash_and_document.xml");
            MifielAPI.Utils.MifielUtils.AppendPDFBase64InOriginalXml(_pdfFilePath, pathOriginalXml, pathNewXml);
            Assert.True(File.Exists(pathNewXml));
        }

        [Test]
        public void Documents__SaveWithOriginalHashAndFileName__ShouldReturnADocument()
        {
            SetSandboxUrl();
            Document document = new Document()
            {
                OriginalHash = MifielAPI.Utils.MifielUtils.GetDocumentHash(_pdfFilePath),
                FileName = "PdfFileName",
                Signatures = new List<Signature>(),
                ManualClose = false
            };
            document = _docs.Save(document);
            Assert.IsNotNull(document);
        }

        [Test]
        public void Documents__SaveWithoutRequiredFields__ShouldThrowAnException()
        {
            SetSandboxUrl();
            var document = new Document() { CallbackUrl = "http://www.google.com" };

            Assert.Throws<MifielAPI.Exceptions.MifielException>(() => _docs.Save(document));
            Assert.IsNotNull(document);
        }

        [Test]
        public void Documents__Find__ShouldReturnADocument()
        {
            SetSandboxUrl();
            Documents__SaveWithOriginalHashAndFileName__ShouldReturnADocument();
            var allDocuments = _docs.FindAll();
            if (allDocuments.Count > 0)
            {
                Document doc1 = _docs.Find(allDocuments[0].Id);
                Assert.IsNotNull(doc1);
            }
            else
            {
                throw new MifielAPI.Exceptions.MifielException("No documents found");
            }
        }

        [Test]
        public void Documents__Delete__ShouldRemoveADocument()
        {
            SetSandboxUrl();
            Document document = new Document()
            {
                OriginalHash = MifielAPI.Utils.MifielUtils.GetDocumentHash(_pdfFilePath),
                FileName = "PdfFileName"
            };

            document = _docs.Save(document);
            _docs.Delete(document.Id);
        }

        [Test]
        public void Documents__RequestSignature__ShouldReturnASignatureResponse()
        {
            SetSandboxUrl();
            var document = new Document()
            {
                OriginalHash = MifielAPI.Utils.MifielUtils.GetDocumentHash(_pdfFilePath),
                FileName = "PdfFileName"
            };

            document = _docs.Save(document);

            SignatureResponse signatureResponse = _docs.RequestSignature(document.Id,
                                "enrique@test.com", "enrique2@test.com");
            Assert.IsNotNull(signatureResponse);
        }

        [Test]
        public void Documents__SaveFile__ShouldSaveFileOnSpecifiedPath()
        {
            SetSandboxUrl();
            Document document = new Document() { File = _pdfFilePath };

            document = _docs.Save(document);

            _docs.SaveFile(document.Id, Path.Combine(_currentDirectory, "pdf_save_test.pdf"));
        }

        private void SetSandboxUrl()
        {
            _apiClient.Url = "https://sandbox.mifiel.com";
        }
    }
}
