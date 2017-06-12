using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MifielAPI;
using MifielAPI.Dao;
using System.Collections.Generic;
using MifielAPI.Objects;

namespace MifielApiTests
{
    [TestClass]
    public class DocumentsTests
    {
        private static ApiClient _apiClient;
        private static Documents _docs;
        private static string _pdfFilePath;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            string appId = "585763293c61baf5ac9d3819e4610dc25e76cade";
            string appSecret = "SyD1xoS4JkaPoDPbfTnUG2QQ20SIV+WWxdQIZPSiH1WrK2E6LrWhWIjGeHeuSfxtteNsgQZo+Xq+fdriJexG7g==";

            _pdfFilePath = @"C:\Users\Jose Luis\Desktop\REBA sheet.pdf";
            _apiClient = new ApiClient(appId, appSecret);
            _docs = new Documents(_apiClient);
        }

        [TestMethod]
        [ExpectedException(typeof(MifielAPI.Exceptions.MifielException))]
        public void Documents__WrongUrl__ShouldThrowAnException()
        {
            _apiClient.Url = "www.google.com";
        }

        [TestMethod]
        public void Documents__CorrectUrl__ShouldNotThrowAnException()
        {
            _apiClient.Url = "https://sandbox.mifiel.com";
        }

        [TestMethod]
        public void Documents__FindAllDocuments__ShouldReturnAList()
        {
            SetSandboxUrl();
            List<Document> allDocuments = _docs.FindAll();
            Assert.IsNotNull(allDocuments);
        }

        [TestMethod]
        public void Documents__SaveWithFilePath__ShouldReturnADocument()
        {
            SetSandboxUrl();
            Document doc = new Document();
            doc.File = _pdfFilePath;

            doc = _docs.Save(doc);
            Assert.IsNotNull(doc);
        }

        [TestMethod]
        public void Documents__SaveWithOriginalHashAndFileName__ShouldReturnADocument()
        {
            SetSandboxUrl();
            Document doc = new Document();
            doc.OriginalHash = MifielAPI.Utils.MifielUtils.GetDocumentHash(_pdfFilePath);
            doc.FileName = "PdfFileName";

            doc = _docs.Save(doc);
            Assert.IsNotNull(doc);
        }

        [TestMethod]
        [ExpectedException(typeof(MifielAPI.Exceptions.MifielException))]
        public void Documents__SaveWithoutRequiredFields__ShouldThrowAnException()
        {
            SetSandboxUrl();
            Document doc = new Document();
            doc.CallbackUrl = "http://www.google.com";

            doc = _docs.Save(doc);
            Assert.IsNotNull(doc);
        }

        [TestMethod]
        public void Documents__Find__ShouldReturnADocument()
        {
            SetSandboxUrl();
            Documents__SaveWithOriginalHashAndFileName__ShouldReturnADocument();

            List<Document> allDocuments = _docs.FindAll();
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

        [TestMethod]
        public void Documents__Delete__ShouldRemoveADocument()
        {
            SetSandboxUrl();
            Document doc = new Document();
            doc.OriginalHash = MifielAPI.Utils.MifielUtils.GetDocumentHash(_pdfFilePath);
            doc.FileName = "PdfFileName";
            doc = _docs.Save(doc);

            _docs.Delete(doc.Id);
        }

        [TestMethod]
        public void Documents__RequestSignature__ShouldReturnASignatureResponse()
        {
            SetSandboxUrl();
            Document doc = new Document();
            doc.OriginalHash = MifielAPI.Utils.MifielUtils.GetDocumentHash(_pdfFilePath);
            doc.FileName = "PdfFileName";
            doc = _docs.Save(doc);

            SignatureResponse signatureResponse = _docs.RequestSignature(doc.Id,
                                "enrique@test.com", "enrique2@test.com");

            Assert.IsNotNull(signatureResponse);
        }

        [TestMethod]
        public void Documents__SaveFile__ShouldSaveFileOnSpecifiedPath()
        {
            SetSandboxUrl();
            Document doc = new Document();
            doc.File = _pdfFilePath;

            doc = _docs.Save(doc);

            _docs.SaveFile(doc.Id, @"C:\Users\Jose Luis\Desktop\test123.pdf");
        }

        private void SetSandboxUrl()
        {
            _apiClient.Url = "https://sandbox.mifiel.com";
        }
    }
}
