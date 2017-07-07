using System;
using MifielAPI.Exceptions;
using MifielAPI.Objects;
using MifielAPI.Utils;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MifielAPI.Dao
{
    public class Documents : BaseObjectDAO<Document>
    {
        private string _documentsPath = "documents";

        public Documents(ApiClient apiClient) : base(apiClient) { }

        public override void Delete(string id)
        {
            ApiClient.Delete(_documentsPath + "/" + id);
        }

        public override Document Find(string id)
        {
            HttpContent httpResponse = ApiClient.Get(_documentsPath + "/" + id);
            string response = httpResponse.ReadAsStringAsync().Result;
            return MifielUtils.ConvertJsonToObject<Document>(response);
        }

        public override List<Document> FindAll()
        {
            HttpContent httpResponse = ApiClient.Get(_documentsPath);
            string response = httpResponse.ReadAsStringAsync().Result;
            return MifielUtils.ConvertJsonToObject<List<Document>>(response);
        }

        public void SaveFile(string id, string localPath)
        {
            HttpContent httpResponse = ApiClient.Get(_documentsPath + "/" + id + "/file");
            MifielUtils.SaveHttpResponseToFile(httpResponse, localPath);
        }


        public void SaveXml(string id, string localPath)
        {
            HttpContent httpResponse = ApiClient.Get(_documentsPath + "/" + id + "/xml");
            MifielUtils.SaveHttpResponseToFile(httpResponse, localPath);
        }

        public SignatureResponse RequestSignature(string id, string email, string cc)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("email", email);
            parameters.Add("cc", cc);

            FormUrlEncodedContent httpContent = new FormUrlEncodedContent(parameters);
            HttpContent httpResponse = ApiClient.Post(_documentsPath + "/" + id + "/request_signature", httpContent);
            string response = httpResponse.ReadAsStringAsync().Result;
            return MifielUtils.ConvertJsonToObject<SignatureResponse>(response);
        }

        public override Document Save(Document document)
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

                var parameters =new  List <KeyValuePair<string, string>>();

                if (!String.IsNullOrEmpty(document.CallbackUrl.Trim()))
                {
                    parameters.Add(new KeyValuePair<string, string>("callback_url", document.CallbackUrl));
                }

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

                MifielUtils.AppendTextParamToContent(parameters, "callback_url", document.CallbackUrl);

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
