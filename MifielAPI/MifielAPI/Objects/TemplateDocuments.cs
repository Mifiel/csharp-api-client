using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MifielAPI.Objects
{
    public class TemplateDocuments
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("file_file_name")]
        public string FileName { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("status")]
        public List<object> Status { get; set; }
        [JsonProperty("owner")]
        public User Owner { get; set; }
        [JsonProperty("file")]
        public String File { get; set; } 
    }
}