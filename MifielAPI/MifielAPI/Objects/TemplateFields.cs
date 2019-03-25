using Newtonsoft.Json;

namespace MifielAPI.Objects
{
    public class TemplateFields
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}