using Newtonsoft.Json;

namespace MifielAPI.Objects
{
    public class Certificate
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("file")]
        public string File { get; set; }
        [JsonProperty("type_of")]
        public string TypeOf { get; set; }
        [JsonProperty("cer_hex")]
        public string CerHex { get; set; }
        [JsonProperty("owner")]
        public string Owner { get; set; }
        [JsonProperty("tax_id")]
        public string TaxId { get; set; }
        [JsonProperty("expires_at")]
        public string ExpiresAt { get; set; }
        [JsonProperty("expired")]
        public bool Expired { get; set; }
    }
}
