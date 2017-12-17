using System;
using Mono.Cecil.Cil;

namespace Fody
{
    [Obsolete(OnlyForTesting.Message)]
    public class SequencePointMessage
    {
        public string Text { get; internal set; }
        public SequencePoint SequencePoint { get; internal set; }
    }
}