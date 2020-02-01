using System;
using Mono.Cecil.Cil;

namespace Fody
{
    public class WeavingException :
        Exception
    {
        public WeavingException(string message)
            : base(message)
        {
        }

        public SequencePoint? SequencePoint { get; set; }
    }
}