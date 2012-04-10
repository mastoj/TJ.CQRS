using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TJ.CQRS.Exceptions
{
    public class UnregisteredEventException : Exception
    {
        public UnregisteredEventException()
        {
            
        }

        public UnregisteredEventException(string message) : base(message)
        {
            
        }
    }
}
