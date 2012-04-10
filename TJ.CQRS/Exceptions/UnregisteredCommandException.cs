using System;

namespace TJ.CQRS.Exceptions
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