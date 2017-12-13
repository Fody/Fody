using System;

namespace Fody
{
    public class WeavingException : Exception
    {
        public WeavingException(string message)
            : base(message)
        {
        }
    }
}