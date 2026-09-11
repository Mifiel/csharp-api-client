using MifielAPI.Objects;
using MifielAPI.Utils;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace MifielAPI.Dao
{
    /// <summary>
    /// CRUD + trigger helpers for account-level webhooks.
    /// See https://docs.mifiel.com/en/#tag/Webhooks
    /// </summary>
    public class Webhooks : BaseObjectDAO<Webhook>
    {
        private string _webhooksPath = "webhooks";

        public Webhooks(ApiClient apiClient) : base(apiClient) { }

        public override void Delete(string id)
        {
            ApiClient.Delete(_webhooksPath + "/" + id);
        }

        public override Webhook Find(string id)
        {
            HttpContent httpResponse = ApiClient.Get(_webhooksPath + "/" + id);
            string response = httpResponse.ReadAsStringAsync().Result;
            return MifielUtils.ConvertJsonToObject<Webhook>(response);
        }

        public override List<Webhook> FindAll()
        {
            HttpContent httpResponse = ApiClient.Get(_webhooksPath);
            string response = httpResponse.ReadAsStringAsync().Result;
            return MifielUtils.ConvertJsonToObject<List<Webhook>>(response);
        }

        public override Webhook Save(Webhook webhook)
        {
            string json = MifielUtils.ConvertObjectToJson(webhook);
            HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            HttpContent httpResponse = ApiClient.Post(_webhooksPath, httpContent);
            string response = httpResponse.ReadAsStringAsync().Result;
            return MifielUtils.ConvertJsonToObject<Webhook>(response);
        }

        /// <summary>
        /// Trigger delivery for a webhook.
        /// </summary>
        /// <param name="id">Webhook id</param>
        /// <param name="resource">UUID of the related resource included in the callback payload</param>
        /// <param name="instant">When true, deliver immediately once instead of enqueueing retries</param>
        public string Trigger(string id, string resource, bool instant = false)
        {
            var body = new Dictionary<string, object>
            {
                { "resource", resource },
                { "instant", instant }
            };
            string json = MifielUtils.ConvertObjectToJson(body);
            HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            HttpContent httpResponse = ApiClient.Post(_webhooksPath + "/" + id + "/trigger", httpContent);
            return httpResponse.ReadAsStringAsync().Result;
        }
    }
}
