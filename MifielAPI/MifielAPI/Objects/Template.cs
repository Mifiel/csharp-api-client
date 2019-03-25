using System;
using Newtonsoft.Json;

namespace MifielAPI.Objects
{
    public class Template
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("has_documents")]
        public Boolean? HasDocuments { get; set; }
        [JsonProperty("header")]
        public string Header { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("footer")]
        public string Footer { get; set; }
        [JsonProperty("csv")]
        public string Csv { get; set; }
        [JsonProperty("track")]
        public Boolean? Track { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}