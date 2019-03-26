using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using MifielAPI;
using MifielAPI.Objects;
using MifielAPI.Dao;
using MifielAPI.Utils;

namespace MifielApiTests
{
    [TestFixture]
    public class DocumentsTests
    {
        const string APP_ID = "9fd550a3866164f48516f71bf85a6258cbd07ff3";
        const string APP_SECRET = "3vIc3r6yJ6PYnssO0S0UYfOqLE70/ulHU76nR/IwHm+zKOa8R+6LuwWeK7HX4vpC7AA8P/ViaixyRJuMb2aftg==";

        const string FISICAL_PRIVATE_KEY_FILE = "carlos_zavala_lopez.key";
        const string FISICAL_CERTIFICATE_FILE = "carlos_zavala_lopez.cer";
        const string FILE_PDF = "test-pdf.pdf";

        private static ApiClient _apiClient;
        private static Documents _docs;
        private static string _pdfFilePath;
        private static string _fisicalPrivateKey;
        private static string _fisicalCertificate;
        private static SignProperties _signProperties = new SignProperties();

        private readonly string _currentDirectory = Path.GetFullPath(TestContext.CurrentContext.TestDirectory);
        private readonly string _correctPassword = "12345678a";
        private readonly string _incorrectPassword = "12345678aa";

        [SetUp]
        public void SetUp()
        {

            _fisicalPrivateKey = Path.Combine(_currentDirectory, FISICAL_PRIVATE_KEY_FILE);
            _fisicalCertificate = Path.Combine(_currentDirectory, FISICAL_CERTIFICATE_FILE);
            _pdfFilePath = Path.Combine(_currentDirectory, FILE_PDF);

            _apiClient = new ApiClient(APP_ID, APP_SECRET);
            _docs = new Documents(_apiClient);

        }

        [Test]
        public void Documents__GetStatus__When__Unsigned__ShouldReturnUnsignedStatus()
        {
            SetSandboxUrl();
            var document = CreateDocumentForSign();
            document = _docs.GetStatus(document.Id);
            Assert.NotNull(document);
            Assert.IsFalse(document.Signed);
        }

        [Test]
        public void Documents__GetStatus__When__Signed__ShouldReturnSignedStatus()
        {
            SetSandboxUrl();
            var document = CreateDocumentForSign();
            document = _docs.GetStatus(document.Id);
            Assert.NotNull(document);
            Assert.IsFalse(document.Signed);

            _signProperties.PrivateKeyFullPath = _fisicalPrivateKey;
            _signProperties.PassPhrase = _correctPassword;
            _signProperties.DocumentId = document.Id;
            _signProperties.DocumentOriginalHash = document.OriginalHash;
            _signProperties.CertificateFullPath = _fisicalCertificate;

            var documentSigned = _docs.Sign(_signProperties);
            Assert.IsTrue(documentSigned.Signed);

            document = _docs.GetStatus(document.Id); 
            Assert.IsTrue(document.Signed);
        }

        [Test]
        public void Documents__Sign__With__Empty__Password__ShouldThrowAnException()
        {
            _signProperties = new SignProperties()
            {
                PassPhrase = string.Empty
            };

            var exception = Assert.Throws<MifielAPI.Exceptions.MifielException>(() => _docs.Sign(_signProperties));
            Assert.That(exception.Message.Contains(SignProperties.PASSPHRASE_REQUIRE_EXCEPTION));
        }

        [Test]
        public void Documents__Sign__Without__EncriptedPrivateKeyData__And__PrivateKeyFullPath__ShouldThrowAnException()
        {
            _signProperties = new SignProperties()
            {
                PassPhrase = _correctPassword
            };

            var exception = Assert.Throws<MifielAPI.Exceptions.MifielException>(() => _docs.Sign(_signProperties));
            Assert.That(exception.Message.Contains(SignProperties.PRIVATE_KEY_ARGUMENTS_EXCEPTION));
        }

        [Test]
        public void Documents__Sign__With__PrivateKeyFullPath__Incorrect__ShouldThrowAnException()
        {
            _signProperties = new SignProperties()
            {
                PassPhrase = _correctPassword,
                PrivateKeyFullPath = "fake_path"
            };

            var exception = Assert.Throws<MifielAPI.Exceptions.MifielException>(() => _docs.Sign(_signProperties));
            Assert.IsInstanceOf<FileNotFoundException>(exception.InnerException);
        }

        [Test]
        public void Documents__Sign__Without__DocumentId__ShouldThrowAnException()
        {
            _signProperties = new SignProperties()
            {
                PassPhrase = _correctPassword,
                PrivateKeyFullPath = Path.Combine(_currentDirectory, _fisicalPrivateKey)
            };

            var exception = Assert.Throws<MifielAPI.Exceptions.MifielException>(() => _docs.Sign(_signProperties));
            Assert.That(exception.Message.Contains(SignProperties.DOCUMENT_ID_REQUIRE_EXCEPTION));
        }

        [Test]
        public void Documents__Sign__Without__DocumentOriginalHash__ShouldThrowAnException()
        {
            _signProperties = new SignProperties()
            {
                PassPhrase = _correctPassword,
                PrivateKeyFullPath = Path.Combine(_currentDirectory, _fisicalPrivateKey),
                DocumentId = "SomeId"
            };

            var exception = Assert.Throws<MifielAPI.Exceptions.MifielException>(() => _docs.Sign(_signProperties));
            Assert.That(exception.Message.Contains(SignProperties.DOCUMENT_ORIGINAL_HASH_REQUIRE_EXCEPTION));
        }

        [Test]
        public void Documents__Sign__Without__CertificateId__And__CertificateData__ShouldThrowAnException()
        {
            _signProperties = new SignProperties()
            {
                PassPhrase = _correctPassword,
                PrivateKeyFullPath = Path.Combine(_currentDirectory, _fisicalPrivateKey),
                DocumentId = "SomeId",
                DocumentOriginalHash = "SomeHash"
            };

            var exception = Assert.Throws<MifielAPI.Exceptions.MifielException>(() => _docs.Sign(_signProperties));
            Assert.That(exception.Message.Contains(SignProperties.CERTIFICATE_ARGUMENTS_EXCEPTION));
        }

        [Test]
        public void Documents__Sign__Using__Password__Incorrect__ShouldThrowAnException()
        {
            _signProperties.PrivateKeyFullPath = _fisicalPrivateKey;
            _signProperties.PassPhrase = _incorrectPassword;
            _signProperties.DocumentId = "SomeId";
            _signProperties.DocumentOriginalHash = "OriginalHash";
            _signProperties.CertificateFullPath = Path.Combine(_currentDirectory, "carlos_zavala_lopez.cer");

            var exception = Assert.Throws<MifielAPI.Exceptions.MifielException>(() => _docs.Sign(_signProperties));
            Assert.That(exception.Message.Contains(MifielUtils.INVALID_PASSWORD));

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

        private Document CreateDocumentForSign()
        {
            var document = new Document()
            {
                File = Path.Combine(_currentDirectory, _pdfFilePath)
            };

            var signatures = new List<Signature>(){
                  new Signature(){
                      Email = "juan+carlos+zavala+lopez@mifiel.com",
                      TaxId = "ZACA850805JX8",
                      SignerName = "Carlos Zavala Lopez"
                  }
              };

            document.Signatures = signatures;
            return _docs.Save(document);
        }

        [Test]
        public void Documents__Sign__Using__FisicalKey__And__FisicalCertificate__ShouldReturnADocumentSigned()
        {
            SetSandboxUrl();
            var document = CreateDocumentForSign();

            _signProperties.PrivateKeyFullPath = _fisicalPrivateKey;
            _signProperties.PassPhrase = _correctPassword;
            _signProperties.DocumentId = document.Id;
            _signProperties.DocumentOriginalHash = document.OriginalHash;
            _signProperties.CertificateFullPath = _fisicalCertificate;

            var documentSigned = _docs.Sign(_signProperties);
            Assert.True(documentSigned.Signers[0].Signed);
            Assert.True(documentSigned.Signed);
            Assert.IsNotNull(documentSigned.SignedAt);
            Assert.True(documentSigned.SignedByAll);
        }

        [Test]
        public void Documents__Sign__Using__EncriptedPrivateKeyData__And__CertificateData__ShouldReturnADocumentSigned()
        {
            SetSandboxUrl();
            var document = CreateDocumentForSign();

            _signProperties.EncriptedPrivateKeyData = File.ReadAllBytes(_fisicalPrivateKey);
            _signProperties.PassPhrase = _correctPassword;
            _signProperties.DocumentId = document.Id;
            _signProperties.DocumentOriginalHash = document.OriginalHash;
            _signProperties.CertificateData = File.ReadAllBytes(_fisicalCertificate);

            var documentSigned = _docs.Sign(_signProperties);
            Assert.True(documentSigned.Signers[0].Signed);
            Assert.True(documentSigned.Signed);
            Assert.IsNotNull(documentSigned.SignedAt);
            Assert.True(documentSigned.SignedByAll);
        }

        [Test]
        public void Documents__Sign__Using__EncriptedPrivateKeyData__And__CertificateFullPath__ShouldReturnADocumentSigned()
        {
            SetSandboxUrl();
            var document = CreateDocumentForSign();

            _signProperties.EncriptedPrivateKeyData = File.ReadAllBytes(_fisicalPrivateKey);
            _signProperties.PassPhrase = _correctPassword;
            _signProperties.DocumentId = document.Id;
            _signProperties.DocumentOriginalHash = document.OriginalHash;
            _signProperties.CertificateFullPath = _fisicalCertificate;

            var documentSigned = _docs.Sign(_signProperties);
            Assert.True(documentSigned.Signers[0].Signed);
            Assert.True(documentSigned.Signed);
            Assert.IsNotNull(documentSigned.SignedAt);
            Assert.True(documentSigned.SignedByAll);
        }


        [Test]
        public void Documents__Sign__Using__PrivateKeyFullPath__And__CertificateData__ShouldReturnADocumentSigned()
        {
            SetSandboxUrl();
            var document = CreateDocumentForSign();

            _signProperties.PrivateKeyFullPath = _fisicalPrivateKey;
            _signProperties.PassPhrase = _correctPassword;
            _signProperties.DocumentId = document.Id;
            _signProperties.DocumentOriginalHash = document.OriginalHash;
            _signProperties.CertificateData = File.ReadAllBytes(_fisicalCertificate);

            var documentSigned = _docs.Sign(_signProperties);
            Assert.True(documentSigned.Signers[0].Signed);
            Assert.True(documentSigned.Signed);
            Assert.IsNotNull(documentSigned.SignedAt);
            Assert.True(documentSigned.SignedByAll);
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
