using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAF
{
    public class AuthorizationFailedException : Exception
    {
        public AuthorizationFailedException()
            : base()
        {
        }

        public AuthorizationFailedException(string message)
            : base(message)
        {
        }

        public AuthorizationFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
