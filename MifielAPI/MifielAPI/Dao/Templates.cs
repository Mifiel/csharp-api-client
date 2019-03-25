using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using MifielAPI.Exceptions;
using MifielAPI.Objects;
using MifielAPI.Utils;
using Newtonsoft.Json;

namespace MifielAPI.Dao
{
    public class Templates : BaseObjectDAO<Template>
    {
        const string TEMPLATES_PATH = "templates";
        const string GENERATE_DOCUMENT = "generate_document";
        const string GENERATE_DOCUMENTS = "generate_documents";
        const string FIELDS = "fields";
        const string DOCUMENTS = "documents";

        public Templates(ApiClient apiClient) : base(apiClient) { }

        public override void Delete(string id)
        {
            HttpContent httpResponse = ApiClient.Delete(TEMPLATES_PATH + "/" + id);
        }

        public override Template Find(string id)
        {
            try
            {
                HttpContent httpResponse = ApiClient.Get(TEMPLATES_PATH + "/" + id);
                string response = httpResponse.ReadAsStringAsync().Result;
                return MifielUtils.ConvertJsonToObject<Template>(response);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override List<Template> FindAll()
        {
            HttpContent httpResponse = ApiClient.Get(TEMPLATES_PATH);
            string response = httpResponse.ReadAsStringAsync().Result;
            return MifielUtils.ConvertJsonToObject<List<Template>>(response);
        }

        public List<TemplateFields> GetFields(string templateId)
        {
            var url = TEMPLATES_PATH + "/" + templateId + "/" + FIELDS;

            HttpContent httpResponse = ApiClient.Get(url);
            string response = httpResponse.ReadAsStringAsync().Result;
            return MifielUtils.ConvertJsonToObject<List<TemplateFields>>(response);
        }

        public List<TemplateDocuments> GetDocuments(string templateId)
        {
            var url = TEMPLATES_PATH + "/" + templateId + "/" + DOCUMENTS;

            HttpContent httpResponse = ApiClient.Get(url);
            string response = httpResponse.ReadAsStringAsync().Result;
            return MifielUtils.ConvertJsonToObject<List<TemplateDocuments>>(response);
        }

        public SimpleResponse GenerateSeveralDocuments(TemplateGenerateDocuments generateDocuments)
        {
            try
            {
                var url = TEMPLATES_PATH + "/" + generateDocuments.TemplateId + "/" + GENERATE_DOCUMENTS;
                var json = JsonConvert.SerializeObject(generateDocuments);

                string response = CreateDocument(url, json);
                return MifielUtils.ConvertJsonToObject<SimpleResponse>(response);
            }
            catch (Exception ex)
            {
                throw new MifielException(ex.Message, ex);
            }
        }

        public override Template Save(Template template)
        {
            HttpContent httpContent = BuildHttpBody(template);
            HttpContent httpResponse = ApiClient.Post(TEMPLATES_PATH, httpContent);
            string response = httpResponse.ReadAsStringAsync().Result;
            return MifielUtils.ConvertJsonToObject<Template>(response);
        }

        public Document GenerateDocument(TemplateGenerateDocument generateDocument)
        {
            try
            {
                var url = TEMPLATES_PATH + "/" + generateDocument.Id + "/" + GENERATE_DOCUMENT;
                var json = JsonConvert.SerializeObject(generateDocument);
                json = json.Replace("\"data\":{", "");
                json = json.Replace("}}", "}");

                string response = CreateDocument(url, json);
                return MifielUtils.ConvertJsonToObject<Document>(response);
            }
            catch (Exception ex)
            {
                throw new MifielException(ex.Message, ex);
            }
        }

        private string CreateDocument(string url, string json)
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpContent httpResponse = ApiClient.Post(url, content);
            return httpResponse.ReadAsStringAsync().Result;
        }

        private HttpContent BuildHttpBody(Template template)
        {
            var parameters = new Dictionary<string, string>();
            MifielUtils.AppendTextParamToContent(parameters, "name", template.Name);
            MifielUtils.AppendTextParamToContent(parameters, "description", template.Description);
            MifielUtils.AppendTextParamToContent(parameters, "header", template.Header);
            MifielUtils.AppendTextParamToContent(parameters, "content", template.Content);
            MifielUtils.AppendTextParamToContent(parameters, "footer", template.Footer);
            MifielUtils.AppendTextParamToContent(parameters, "track", template.Track.ToString().ToLower());
            MifielUtils.AppendTextParamToContent(parameters, "type", template.Type);

            return new FormUrlEncodedContent(parameters);
        }
    }
}