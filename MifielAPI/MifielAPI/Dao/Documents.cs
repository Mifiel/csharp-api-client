using MifielAPI.Objects;
using MifielAPI.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;

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
            string response = ApiClient.Get("canonical-string");//_documentsPath + "/" + id);
            return MifielUtils.ConvertJsonToObject<Document>(response);
        }

        public override List<Document> FindAll()
        {
            string response = ApiClient.Get(_documentsPath);
            return MifielUtils.ConvertJsonToObject<List<Document>>(response);
        }

        public override Document Save(Document document)
        {
            HttpContent httpContent = BuildHttpBody(document);
            string response = ApiClient.Post(_documentsPath, httpContent);
            return MifielUtils.ConvertJsonToObject<Document>(response);
        }

        private HttpContent BuildHttpBody(Document document)
        {
            throw new NotImplementedException();
        }
    }
}
