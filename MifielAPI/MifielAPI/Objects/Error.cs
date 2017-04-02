using System.Collections.Generic;

namespace MifielAPI.Objects
{
    public class MifielError
    {
        public string Status { get; set; }
        public string Error { get; set; }
        public List<string> Errors { get; set; }
    }
}
