using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MifielAPI.Objects
{
    public class TemplateGenerateDocuments
    {
        [JsonProperty("template_id")]
        public string TemplateId { get; set; }
        [JsonProperty("identifier")]
        public string Identifier { get; set; }
        [JsonProperty("documents")]
        public List<TemplateDocumentsData> Documents { get; set; }
        [JsonProperty("callback_url")]
        public string CallbackUrl { get; set; }
    }
}