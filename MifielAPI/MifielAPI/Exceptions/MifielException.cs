using MifielAPI.Objects;
using System;
using System.Runtime.Serialization;

namespace MifielAPI.Exceptions
{
    public class MifielException : Exception
    {
        public MifielError MifielError { get; set; }

        public MifielException() : base()
        {
        }

        public MifielException(string message) : base(message)
        {
        }

        public MifielException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MifielException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }

        public MifielException(string message, string httpResponse) : base(message)
        {
            try
            {
                MifielError = Utils.MifielUtils.ConvertJsonToObject<MifielError>(httpResponse);
            }
            catch (Exception) { }
        }
    }
}
