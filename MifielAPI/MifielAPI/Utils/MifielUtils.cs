using MifielAPI.Exceptions;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using MifielAPI.Objects;

namespace MifielAPI.Utils
{

    public static class MifielUtils
    {
        private static Regex _rgx = new Regex("/+$");
        private static SHA256 _sha256 = SHA256.Create();
        private static UTF8Encoding _utfEncoding = new UTF8Encoding();
        public const string INVALID_PASSWORD = "The password is incorrect. Please try again";

        public static ISigner BuildSignature(SignProperties properties)
        {
            try
            {

                var privateKey = PrivateKeyFactory.DecryptKey(properties.PassPhrase.ToCharArray(), properties.EncriptedPrivateKeyData);
                var signer = SignerUtilities.GetSigner(properties.Algorithm);
                signer.Init(true, privateKey);

                return signer;
            }

            catch (InvalidCipherTextException)
            {
                throw new ArgumentException(INVALID_PASSWORD);
            }
            catch (Exception ex)
            {
                throw new MifielException(ex.Message, ex);
            }

        }

        internal static bool IsValidUrl(string url)
        {
            Uri uriResult;
            return Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp ||
                        uriResult.Scheme == Uri.UriSchemeHttps);
        }

        internal static string RemoveLastSlashFromUrl(string url)
        {
            return _rgx.Replace(url, "");
        }

        public static string GetDocumentHash(string path)
        {
            try
            {
                using (FileStream stream = File.OpenRead(path))
                {
                    byte[] hashValue = _sha256.ComputeHash(stream);
                    return BitConverter.ToString(hashValue).Replace("-", string.Empty);
                }
            }
            catch (Exception ex)
            {
                throw new MifielException("Error generating document Hash", ex);
            }
        }

        public static string CalculateMD5(string content)
        {
            try
            {
                byte[] contentBytes = _utfEncoding.GetBytes(content);
                byte[] contentHash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(contentBytes);
                return ConvertBytesToHex(contentHash);
            }
            catch (Exception ex)
            {
                throw new MifielException("Error calculating MD5", ex);
            }
        }

        internal static string ConvertBytesToHex(Byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLowerInvariant();
        }

        internal static void SaveHttpResponseToFile(HttpContent httpResponse, string localPath)
        {
            try
            {
                byte[] byteContent = httpResponse.ReadAsByteArrayAsync().Result;
                File.WriteAllBytes(localPath, byteContent);
            }
            catch (Exception ex)
            {
                throw new MifielException("Error saving file", ex);
            }
        }

        public static string CalculateHMAC(string appSecret, string canonicalString)
        {
            try
            {
                HMACSHA1 hmacSha1 = new HMACSHA1(Encoding.UTF8.GetBytes(appSecret));
                byte[] byteArray = Encoding.ASCII.GetBytes(canonicalString);
                MemoryStream stream = new MemoryStream(byteArray);
                return Convert.ToBase64String(hmacSha1.ComputeHash(stream));
            }
            catch (Exception ex)
            {
                throw new MifielException("Error calculating HMAC SHA1", ex);
            }
        }

        internal static void AppendTextParamToContent(Dictionary<string, string> parameters, string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                parameters.Add(name, value);
            }
        }

        public static T ConvertJsonToObject<T>(string json)
        {
            try
            {
                using (var stringReader = new StringReader(json))
                {
                    using (var jsonReader = new JsonTextReader(stringReader))
                    {
                        var jsonSerializer = new JsonSerializer();
                        return jsonSerializer.Deserialize<T>(jsonReader);
                    }
                }
            }
            catch (Exception e)
            {
                throw new MifielException("Error converting JSON to Object", e);
            }
        }

        public static string ConvertObjectToJson<T>(T objectToConvert)
        {
            try
            {
                return JsonConvert.SerializeObject(objectToConvert);
            }
            catch (Exception e)
            {
                throw new MifielException("Error converting Object to JSON", e);
            }
        }
    }
}
