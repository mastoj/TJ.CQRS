using System;

namespace TJ.CQRS.Infrastructure.Exceptions
{
    public class UnregisteredCommandException : Exception
    {
        public UnregisteredCommandException()
        {

        }

        public UnregisteredCommandException(string message)
            : base(message)
        {

        }
    }
}