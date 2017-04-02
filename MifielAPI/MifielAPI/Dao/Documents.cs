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
            ApiClient.Delete(_documentsPath + "/" + id);
        }

        public override Document Find(string id)
        {
            string response = ApiClient.Get(_documentsPath + "/" + id);
            return MifielUtils.ConvertJsonToObject<Document>(response);
        }

        public override List<Document> FindAll()
        {
            string response = ApiClient.Get(_documentsPath);
            return MifielUtils.ConvertJsonToObject<List<Document>>(response);
        }

        public override Document Save(Document document)
        {
            if (string.IsNullOrEmpty(document.Id))
            {
                HttpContent httpContent = BuildHttpBody(document);
                string response = ApiClient.Post(_documentsPath, httpContent);
                return MifielUtils.ConvertJsonToObject<Document>(response);
            }
            else
            {
                string json = MifielUtils.ConvertObjectToJson(document);
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                string response = ApiClient.Put(_documentsPath, httpContent);
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
                return multipartContent;
            }
            else if (!string.IsNullOrEmpty(originalHash) 
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
                            "signatories[" + i + "][name]", signatures[i].SignatureStr);
                        MifielUtils.AppendTextParamToContent(parameters,
                            "signatories[" + i + "][email]", signatures[i].Email);
                        MifielUtils.AppendTextParamToContent(parameters,
                            "signatories[" + i + "][tax_id]", signatures[i].TaxId);
                    }
                }
                
                return new FormUrlEncodedContent(parameters);
            }
            else
            {
                throw new MifielException("You must provide file or original hash and file name");
            }
        }
    }
}
