using System;
using System.Runtime.Serialization;

namespace FactorioModsManager.Infrastructure
{
    [Serializable]
    public class ImpossibleException : Exception
    {
        public ImpossibleException()
        {
        }

        public ImpossibleException(string? message) : base(message)
        {
        }

        public ImpossibleException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ImpossibleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
