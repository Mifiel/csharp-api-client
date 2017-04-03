using Newtonsoft.Json;

namespace MifielAPI.Objects
{
    public class Data
    {
        [JsonProperty("document")]
        private Document Document { get; set; }
    }
}