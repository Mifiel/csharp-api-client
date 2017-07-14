using System;
using Newtonsoft.Json;

namespace MifielAPI.Objects
{
   public class CloseDocument
    {
       [JsonProperty("success")]
       public bool Success { get; set; }
    }
}
