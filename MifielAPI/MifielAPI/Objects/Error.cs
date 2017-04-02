using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace MifielAPI.Objects
{
    public class MifielError
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("errors")]
        public List<string> Errors { get; set; }

        public override string ToString()
        {
            return string.Format("Status: {0}, Error: {1}, Errors: {2}", Status, Error, GetErrors());
        }

        private string GetErrors()
        {
            StringBuilder errors = new StringBuilder("[ ");

            foreach (string error in Errors)
            {
                errors.AppendFormat(" {{ {0} }}, ", error);
            }

            errors.Append(" ]");

            return errors.ToString();
        }
    }
}
