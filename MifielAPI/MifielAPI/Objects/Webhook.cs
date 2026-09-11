using Newtonsoft.Json;

namespace MifielAPI.Objects
{
    /// <summary>
    /// Account-level webhook subscription.
    /// See https://docs.mifiel.com/en/#tag/Webhooks
    /// </summary>
    public class Webhook
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("callback_type")]
        public string CallbackType { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }
    }
}
