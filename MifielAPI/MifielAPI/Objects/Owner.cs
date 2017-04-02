using Newtonsoft.Json;

namespace MifielAPI.Objects
{
    public class Owner
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}