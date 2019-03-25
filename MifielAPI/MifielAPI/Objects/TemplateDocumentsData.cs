using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MifielAPI.Objects
{
    public class TemplateDocumentsData
    {
        [JsonProperty("fields")]
        public Dictionary<string, string> Fields { get; set; }
        [JsonProperty("signatories")]
        public List<Signature> Signatures { get; set; }
        [JsonProperty("callback_url")]
        public string CallbackUrl { get; set; }
        [JsonProperty("sign_callback_url")]
        public string SignCallbackUrl { get; set; }
        [JsonProperty("external_id")]
        public string ExternalId { get; set; }
        [JsonProperty("manual_close")]
        public Boolean? ManualClose { get; set; }
    }
}
