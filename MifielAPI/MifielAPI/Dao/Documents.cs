using System;
using MifielAPI.Exceptions;
using MifielAPI.Objects;
using MifielAPI.Utils;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace MifielAPI.Dao
{
    public class Documents : BaseObjectDAO<Document>
    {
        private string _documentsPath = "documents";

        public Documents(ApiClient apiClient) : base(apiClient) { }

        public override void Delete(string id)
        {
            try
            {
                ApiClient.Delete(_documentsPath + "/" + id);
            }
            catch (Exception ex)
            {
                throw new MifielException(ex.Message, ex);
            }
        }

        public override Document Find(string id)
        {
            try
            {
                HttpContent httpResponse = ApiClient.Get(_documentsPath + "/" + id);
                string response = httpResponse.ReadAsStringAsync().Result;
                return MifielUtils.ConvertJsonToObject<Document>(response);
            }
            catch (Exception ex)
            {
                throw new MifielException(ex.Message, ex);
            }
        }

        public CloseDocument Close(string id)
        {
            try
            {
                var stringBuilder = new StringBuilder(_documentsPath);
                stringBuilder.Append("/");
                stringBuilder.Append(id);
                stringBuilder.Append("/close");

                HttpContent httpResponse = ApiClient.Post(stringBuilder.ToString());
                string response = httpResponse.ReadAsStringAsync().Result;
                return MifielUtils.ConvertJsonToObject<CloseDocument>(response);
            }
            catch (Exception ex)
            {
                throw new MifielException(ex.Message, ex);
            }
        }

        public override List<Document> FindAll()
        {
            try
            {
                HttpContent httpResponse = ApiClient.Get(_documentsPath);
                string response = httpResponse.ReadAsStringAsync().Result;
                return MifielUtils.ConvertJsonToObject<List<Document>>(response);
            }
            catch (Exception ex)
            {
                throw new MifielException(ex.Message, ex);
            }
        }

        public void SaveFile(string id, string localPath)
        {
            try
            {
                HttpContent httpResponse = ApiClient.Get(_documentsPath + "/" + id + "/file");
                MifielUtils.SaveHttpResponseToFile(httpResponse, localPath);
            }
            catch (Exception ex)
            {
                throw new MifielException(ex.Message, ex);
            }
        }


        public void SaveXml(string id, string localPath)
        {
            try
            {
                HttpContent httpResponse = ApiClient.Get(_documentsPath + "/" + id + "/xml");
                MifielUtils.SaveHttpResponseToFile(httpResponse, localPath);
            }
            catch (Exception ex)
            {
                throw new MifielException(ex.Message, ex);
            }
        }

        public SignatureResponse RequestSignature(string id, string email, string cc)
        {
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("email", email);
                parameters.Add("cc", cc);

                FormUrlEncodedContent httpContent = new FormUrlEncodedContent(parameters);
                HttpContent httpResponse = ApiClient.Post(_documentsPath + "/" + id + "/request_signature", httpContent);
                string response = httpResponse.ReadAsStringAsync().Result;
                return MifielUtils.ConvertJsonToObject<SignatureResponse>(response);
            }
            catch (Exception ex)
            {
                throw new MifielException(ex.Message, ex);
            }
        }

        public override Document Save(Document document)
        {
            try
            {
                if (string.IsNullOrEmpty(document.Id))
                {
                    HttpContent httpContent = BuildHttpBody(document);
                    HttpContent httpResponse = ApiClient.Post(_documentsPath, httpContent);
                    string response = httpResponse.ReadAsStringAsync().Result;
                    return MifielUtils.ConvertJsonToObject<Document>(response);
                }
                else
                {
                    string json = MifielUtils.ConvertObjectToJson(document);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpContent httpResponse = ApiClient.Put(_documentsPath, httpContent);
                    string response = httpResponse.ReadAsStringAsync().Result;
                    return MifielUtils.ConvertJsonToObject<Document>(response);
                }
            }
            catch (Exception ex)
            {
                throw new MifielException(ex.Message, ex);
            }
        }

        public Document Sign(SignProperties signProperties)
        {
            try
            {
                signProperties.Build();
                var signer = MifielUtils.BuildSignature(signProperties);

                var utfEncoding = new UTF8Encoding();
                var inputData = utfEncoding.GetBytes(signProperties.DocumentOriginalHash);
                signer.BlockUpdate(inputData, 0, inputData.Length);

                var signatureHex = MifielUtils.ConvertBytesToHex(signer.GenerateSignature());

                var parameters = new Dictionary<string, string>
                {
                    { "signature", signatureHex }
                };

                if (!string.IsNullOrEmpty(signProperties.CertificateId))
                {
                    parameters.Add("key", signProperties.CertificateId);
                }
                else
                {
                    var certificateHex = MifielUtils.ConvertBytesToHex(signProperties.CertificateData);
                    parameters.Add("certificate", certificateHex);
                }
                FormUrlEncodedContent httpContent = new FormUrlEncodedContent(parameters);
                HttpContent httpResponse = ApiClient.Post(_documentsPath + "/" + signProperties.DocumentId + "/sign", httpContent);
                string response = httpResponse.ReadAsStringAsync().Result;
                return MifielUtils.ConvertJsonToObject<Document>(response);
            }
            catch (Exception ex)
            {
                throw new MifielException(ex.Message, ex);
            }
        }

        public Document GetStatus(string id)
        {
            try
            {
                var uri = _documentsPath + "/" + id + "/status";
                HttpContent httpResponse = ApiClient.Get(uri);
                string response = httpResponse.ReadAsStringAsync().Result;
                return MifielUtils.ConvertJsonToObject<Document>(response);
            }
            catch (Exception ex)
            {
                throw new MifielException(ex.Message, ex);
            }
        }

        private HttpContent BuildHttpBody(Document document)
        {
            List<Signature> signatures = document.Signatures;
            string filePath = document.File;
            string fileName = document.FileName;
            string originalHash = document.OriginalHash;

            if (!string.IsNullOrEmpty(filePath))
            {
                MultipartFormDataContent multipartContent = new MultipartFormDataContent();
                ByteArrayContent pdfContent = new ByteArrayContent(File.ReadAllBytes(filePath));
                pdfContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");

                multipartContent.Add(pdfContent, "file", Path.GetFileName(filePath));

                var parameters = new List<KeyValuePair<string, string>>();

                if (!String.IsNullOrEmpty(document.CallbackUrl))
                {
                    parameters.Add(new KeyValuePair<string, string>("callback_url", document.CallbackUrl));
                }

                if (!String.IsNullOrEmpty(document.SignCallbackUrl))
                {
                    parameters.Add(new KeyValuePair<string, string>("sign_callback_url", document.SignCallbackUrl));
                }

                parameters.Add(new KeyValuePair<string, string>("manual_close", document.ManualClose.ToString().ToLower()));

                if (signatures != null)
                {
                    for (int i = 0; i < signatures.Count; i++)
                    {
                        parameters.Add(new KeyValuePair<string, string>("signatories[" + i + "][name]", signatures[i].SignerName));
                        parameters.Add(new KeyValuePair<string, string>("signatories[" + i + "][email]", signatures[i].Email));
                        parameters.Add(new KeyValuePair<string, string>("signatories[" + i + "][tax_id]", signatures[i].TaxId));
                    }
                }

                foreach (var keyValuePair in parameters)
                {
                    multipartContent.Add(new StringContent(keyValuePair.Value),
                        String.Format("\"{0}\"", keyValuePair.Key));
                }

                return multipartContent;
            }
            if (!string.IsNullOrEmpty(originalHash)
                && !string.IsNullOrEmpty(fileName))
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("original_hash", originalHash);
                parameters.Add("name", fileName);
                parameters.Add("manual_close", document.ManualClose.ToString().ToLower());

                MifielUtils.AppendTextParamToContent(parameters, "callback_url", document.CallbackUrl);
                MifielUtils.AppendTextParamToContent(parameters, "sign_callback_url", document.SignCallbackUrl);

                if (signatures != null)
                {
                    for (int i = 0; i < signatures.Count; i++)
                    {
                        MifielUtils.AppendTextParamToContent(parameters,
                            "signatories[" + i + "][name]", signatures[i].SignerName);
                        MifielUtils.AppendTextParamToContent(parameters,
                            "signatories[" + i + "][email]", signatures[i].Email);
                        MifielUtils.AppendTextParamToContent(parameters,
                            "signatories[" + i + "][tax_id]", signatures[i].TaxId);
                    }
                }

                return new FormUrlEncodedContent(parameters);
            }
            throw new MifielException("You must provide file or original hash and file name");
        }
    }
}
