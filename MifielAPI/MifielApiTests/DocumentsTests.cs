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
        const string APP_ID = "7c938f8e2ff2f083d454127bffd7d4c8bc2c2dee";
        const string APP_SECRET = "1lXeVVMbNe5IH+INgMeQX463fsixeWEMMSLS4WXPwxpR9RwStD3iE4XaNMXbY8YigIxCQP9gb/8xZI3XILN2Rw==";
        private static ApiClient _apiClient;
        private static Documents _docs;
        private static string _pdfFilePath;

        private readonly string _currentDirectory = Path.GetFullPath(TestContext.CurrentContext.TestDirectory);

        [SetUp]
        public void SetUp()
        { 
            _pdfFilePath = Path.Combine(_currentDirectory, "test-pdf.pdf");
            _apiClient = new ApiClient(APP_ID, APP_SECRET);
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
                    Email = "juan@mifiel.com",
                    TaxId = "ZAAJ8301061E0",
                    SignerName = "Juan Antonio Zavala Aguilar"
                }
            };

            var viewers = new List<Viewer>() {
                new Viewer(){
                    Name = "Juan Zavala",
                    Email = "ja.zavala.aguilar@gmail.com"
                }
            };

            document.SendMail = true;
            document.SendInvites = true;
            document.Signatures = signatures;
            document.Viewers = viewers;
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
                ManualClose = false,
            };

            var signatures = new List<Signature>(){
                new Signature(){
                    Email = "juan@mifiel.com",
                    SignerName = "Juan Antonio Zavala Aguilar"
                }
            };

            var viewers = new List<Viewer>() {
                new Viewer(){
                    Name = "Juan Zavala",
                    Email = "ja.zavala.aguilar@gmail.com"
                }
            };

            document.Signatures = signatures;
            document.Viewers = viewers;

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
