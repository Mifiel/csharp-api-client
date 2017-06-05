using Newtonsoft.Json;
using System.Collections.Generic;

namespace MifielAPI.Objects
{
    public class Document
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("original_hash")]
        public string OriginalHash { get; set; }
        [JsonProperty("name")]
        public string FileName { get; set; }
        [JsonProperty("signed_by_all")]
        public bool SignedByAll { get; set; }
        [JsonProperty("signed")]
        public bool Signed { get; set; }
        [JsonProperty("signed_at")]
        public string SignedAt { get; set; }
        [JsonProperty("status")]
        public List<object> Status { get; set; }
        [JsonProperty("owner")]
        public Owner Owner { get; set; }
        [JsonProperty("callback_url")]
        public string CallbackUrl { get; set; }
        [JsonProperty("file")]
        public string File { get; set; }
        [JsonProperty("file_download")]
        public string FileDownload { get; set; }
        [JsonProperty("file_signed")]
        public string FileSigned { get; set; }
        [JsonProperty("file_signed_download")]
        public string FileSignedDownload { get; set; }
        [JsonProperty("file_zipped")]
        public string FileZipped { get; set; }
        [JsonProperty("signatures")]
        public List<Signature> Signatures { get; set; }
    }
}
