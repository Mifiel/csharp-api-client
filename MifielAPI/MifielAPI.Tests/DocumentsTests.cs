using System.IO;
using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using MifielAPI;
using MifielAPI.Objects;
using MifielAPI.Dao;

namespace MifielApiTests
{
    [TestFixture]
    public class DocumentsTests
    {
        private const string DocumentJson = "{\"id\":\"doc-1\",\"name\":\"PdfFileName\"}";
        private const string DocumentsJson = "[{\"id\":\"doc-1\",\"name\":\"PdfFileName\"}]";

        private readonly string _currentDirectory = Path.GetFullPath(TestContext.CurrentContext.TestDirectory);
        private StubHttpMessageHandler _handler;
        private ApiClient _apiClient;
        private Documents _docs;
        private string _pdfFilePath;

        [SetUp]
        public void SetUp()
        {
            _pdfFilePath = Path.Combine(_currentDirectory, "test-pdf.pdf");
            _handler = new StubHttpMessageHandler();
            _apiClient = new ApiClient("test-app-id", "test-app-secret", _handler)
            {
                Url = "https://app-sandbox.mifiel.com"
            };
            _docs = new Documents(_apiClient);
        }

        [Test]
        public void Documents__WrongUrl__ShouldThrowAnException()
        {
            Assert.Throws<MifielAPI.Exceptions.MifielException>(() => _apiClient.Url = "www.google.com");
            Assert.AreEqual(0, _handler.SendCount);
        }

        [Test]
        public void Documents__CorrectUrl__ShouldNotThrowAnException()
        {
            _apiClient.Url = "https://app-sandbox.mifiel.com";
            Assert.AreEqual(0, _handler.SendCount);
        }

        [Test]
        public void Documents__AppendPDFBase64InOriginalXml__ShouldGenerateNewXML()
        {
            var pathOriginalXml = Path.Combine(_currentDirectory, "file_hash.xml");
            var pathNewXml = Path.Combine(_currentDirectory, "file_with_hash_and_document.xml");
            MifielAPI.Utils.MifielUtils.AppendPDFBase64InOriginalXml(_pdfFilePath, pathOriginalXml, pathNewXml);
            Assert.True(File.Exists(pathNewXml));
            Assert.AreEqual(0, _handler.SendCount);
        }

        [Test]
        public void Documents__FindAllDocuments__ShouldReturnAList()
        {
            _handler.EnqueueJson(DocumentsJson);
            var allDocuments = _docs.FindAll();
            Assert.IsNotNull(allDocuments);
            Assert.AreEqual(1, allDocuments.Count);
            Assert.AreEqual("doc-1", allDocuments[0].Id);
            Assert.AreEqual(1, _handler.SendCount);
        }

        [Test]
        public void Documents__Close__Should_Success()
        {
            _handler.EnqueueJson("{\"success\":true}");
            var closeDocument = _docs.Close("doc-1");
            Assert.IsTrue(closeDocument.Success);
            Assert.AreEqual(1, _handler.SendCount);
        }

        [Test]
        public void Documents__SaveWithFilePath__ShouldReturnADocument()
        {
            _handler.EnqueueJson(DocumentJson);
            var document = new Document()
            {
                File = _pdfFilePath,
                ManualClose = false,
                CallbackUrl = "https://example.com/callback",
                SendMail = true,
                SendInvites = true,
                Signatures = new List<Signature>()
                {
                    new Signature()
                    {
                        Email = "juan@mifiel.com",
                        TaxId = "ZAAJ8301061E0",
                        SignerName = "Juan Antonio Zavala Aguilar"
                    }
                },
                Viewers = new List<Viewer>()
                {
                    new Viewer()
                    {
                        Name = "Juan Zavala",
                        Email = "ja.zavala.aguilar@gmail.com"
                    }
                }
            };

            document = _docs.Save(document);
            Assert.IsNotNull(document);
            Assert.AreEqual("doc-1", document.Id);
            Assert.AreEqual(1, _handler.SendCount);
        }

        [Test]
        public void Documents__SaveWithOriginalHashAndFileName__ShouldReturnADocument()
        {
            _handler.EnqueueJson(DocumentJson);
            var document = new Document()
            {
                OriginalHash = MifielAPI.Utils.MifielUtils.GetDocumentHash(_pdfFilePath),
                FileName = "PdfFileName",
                ManualClose = false,
                Signatures = new List<Signature>()
                {
                    new Signature()
                    {
                        Email = "juan@mifiel.com",
                        SignerName = "Juan Antonio Zavala Aguilar"
                    }
                }
            };

            document = _docs.Save(document);
            Assert.IsNotNull(document);
            Assert.AreEqual("doc-1", document.Id);
            Assert.AreEqual(1, _handler.SendCount);
        }

        [Test]
        public void Documents__SaveWithoutRequiredFields__ShouldThrowAnException()
        {
            var document = new Document() { CallbackUrl = "http://www.google.com" };

            Assert.Throws<MifielAPI.Exceptions.MifielException>(() => _docs.Save(document));
            Assert.AreEqual(0, _handler.SendCount);
        }

        [Test]
        public void Documents__Find__ShouldReturnADocument()
        {
            _handler.EnqueueJson(DocumentJson);
            Document doc1 = _docs.Find("doc-1");
            Assert.IsNotNull(doc1);
            Assert.AreEqual("doc-1", doc1.Id);
            Assert.AreEqual(1, _handler.SendCount);
        }

        [Test]
        public void Documents__Delete__ShouldRemoveADocument()
        {
            _handler.EnqueueJson("{}");
            _docs.Delete("doc-1");
            Assert.AreEqual(1, _handler.SendCount);
        }

        [Test]
        public void Documents__RequestSignature__ShouldReturnASignatureResponse()
        {
            _handler.EnqueueJson("{\"status\":\"ok\",\"message\":\"sent\"}");
            SignatureResponse signatureResponse = _docs.RequestSignature("doc-1",
                                "enrique@test.com", "enrique2@test.com");
            Assert.IsNotNull(signatureResponse);
            Assert.AreEqual("ok", signatureResponse.Status);
            Assert.AreEqual(1, _handler.SendCount);
        }

        [Test]
        public void Documents__SaveFile__ShouldSaveFileOnSpecifiedPath()
        {
            _handler.EnqueueBytes(File.ReadAllBytes(_pdfFilePath));
            var savePath = Path.Combine(_currentDirectory, "pdf_save_test.pdf");
            _docs.SaveFile("doc-1", savePath);
            Assert.True(File.Exists(savePath));
            Assert.AreEqual(1, _handler.SendCount);
        }

        [Test]
        public void Documents__UnauthorizedResponse__ShouldThrowAnException()
        {
            _handler.EnqueueJson("{\"error\":\"unauthorized\"}", HttpStatusCode.Unauthorized);
            Assert.Throws<MifielAPI.Exceptions.MifielException>(() => _docs.FindAll());
            Assert.AreEqual(1, _handler.SendCount);
        }
    }
}
