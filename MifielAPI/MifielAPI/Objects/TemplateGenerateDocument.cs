using System;
using Newtonsoft.Json;

namespace MifielAPI.Objects
{
    public class TemplateGenerateDocument
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("track")]
        public Boolean? Track { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("data")]
        public TemplateDocumentsData Data { get; set; }
    }
}