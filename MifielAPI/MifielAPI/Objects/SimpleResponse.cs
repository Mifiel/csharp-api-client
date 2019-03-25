using System;
using Newtonsoft.Json;

namespace MifielAPI.Objects
{
    public class SimpleResponse
    {
        [JsonProperty("status")]
        public String Status { get; set; }
    }
}