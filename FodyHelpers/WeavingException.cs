using Mono.Cecil.Cil;

namespace Fody;

public class WeavingException(string message) :
    Exception(message)
{
    public SequencePoint? SequencePoint { get; set; }
}