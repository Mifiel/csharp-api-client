using Newtonsoft.Json;

namespace MifielAPI.Objects
{
    public class Signer
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("tax_id")]
        public string TaxId { get; set; }
        [JsonProperty("field")]
        public string Field { get; set; }
        [JsonProperty("signed")]
        public bool Signed { get; set; }
        [JsonProperty("widget_id")]
        public string WidgetId { get; set; }
        [JsonProperty("current")]
        public bool Current { get; set; }
        [JsonProperty("fos")]
        public string Fos { get; set; }
        [JsonProperty("pos")]
        public string Pos { get; set; }
    }
}
