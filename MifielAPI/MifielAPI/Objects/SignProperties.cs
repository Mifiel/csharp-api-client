using System;
using System.IO;

namespace MifielAPI.Objects
{
    public class SignProperties
    {
        const string RSA_ENCRYPTION = "SHA256WithRSAEncryption";
        public const string PASSPHRASE_REQUIRE_EXCEPTION = "Must provide PassPhrase";
        public const string DOCUMENT_ID_REQUIRE_EXCEPTION = "Must provide DocumentId";
        public const string DOCUMENT_ORIGINAL_HASH_REQUIRE_EXCEPTION = "Must provide OriginalHash";

        public const string CERTIFICATE_ARGUMENTS_EXCEPTION = "Must provide some of these arguments CertificateData or CertificateFullPath)";
        public const string PRIVATE_KEY_ARGUMENTS_EXCEPTION = "Must provide some of these arguments EncriptedPrivateKeyData or PrivateKeyFullPath)";

        public SignProperties()
        {
            this.Algorithm = RSA_ENCRYPTION;
        }

        public string PrivateKeyFullPath { get; set; }
        public byte[] EncriptedPrivateKeyData { get; set; }
        public string PassPhrase { get; set; }
        public string Algorithm { get; set; }

        public string DocumentId { get; set; }
        public string DocumentOriginalHash { get; set; }

        public string CertificateId { get; set; }
        public string CertificateFullPath { get; set; }
        public byte[] CertificateData { get; set; }

        public void Build()
        {
            BuildEncryptedPrivateKeyData();
            BuildDocument();
            BuildCertifacte();
        }

        private void BuildEncryptedPrivateKeyData()
        {
            if (string.IsNullOrEmpty(this.PassPhrase))
            {
                throw new ArgumentNullException(PASSPHRASE_REQUIRE_EXCEPTION);
            }
            if (this.EncriptedPrivateKeyData == null)
            {
                this.EncriptedPrivateKeyData = GetBytesFromFile(this.PrivateKeyFullPath, PRIVATE_KEY_ARGUMENTS_EXCEPTION);
            }
        }

        private void BuildDocument()
        {
            if (string.IsNullOrEmpty(this.DocumentId))
            {
                throw new ArgumentNullException(DOCUMENT_ID_REQUIRE_EXCEPTION);
            }
            if (string.IsNullOrEmpty(this.DocumentOriginalHash))
            {
                throw new ArgumentNullException(DOCUMENT_ORIGINAL_HASH_REQUIRE_EXCEPTION);
            }
        }

        private void BuildCertifacte()
        {
            if (string.IsNullOrEmpty(this.CertificateId))
            {
                if (this.CertificateData == null)
                {
                    this.CertificateData = GetBytesFromFile(this.CertificateFullPath, CERTIFICATE_ARGUMENTS_EXCEPTION);
                }
            }
        }

        private byte[] GetBytesFromFile(string fullPath, string exception)
        {
            ValidExistFile(fullPath, exception);
            return File.ReadAllBytes(fullPath);
        }

        private void ValidExistFile(string fullPath, string exception)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                throw new ArgumentNullException(exception);
            }

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException();
            }
        }
    }
}