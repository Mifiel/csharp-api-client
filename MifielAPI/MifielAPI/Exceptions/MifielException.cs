using MifielAPI.Objects;
using System;

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

