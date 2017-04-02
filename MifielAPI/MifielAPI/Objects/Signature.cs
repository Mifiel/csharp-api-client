using Newtonsoft.Json;

namespace MifielAPI.Objects
{
    public class Signature
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("signed")]
        public bool Signed { get; set; }
        [JsonProperty("signed_at")]
        public string SignedAt { get; set; }
        [JsonProperty("certificate_id")]
        public string CertificateId { get; set; }
        [JsonProperty("tax_id")]
        public string TaxId { get; set; }
        [JsonProperty("signature")]
        public string SignatureStr { get; set; }
        [JsonProperty("user")]
        public User User { get; set; }
    }
}
