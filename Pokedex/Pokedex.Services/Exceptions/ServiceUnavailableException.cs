using System;
using System.Net;

namespace Pokedex.Services.Exceptions
{
    public class ServiceUnavailableException : Exception
    {
        public ServiceUnavailableException(string message) : base(message)
        {
        }

        public ServiceUnavailableException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
