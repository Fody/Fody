using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Fody;

public static class CecilExtensions
{
    public static SequencePoint? GetSequencePoint(this MethodDefinition method)
    {
        Guard.AgainstNull(nameof(method), method);
        return method.Body.Instructions
            .Select(_ => method.DebugInformation.GetSequencePoint(_))
            .FirstOrDefault(_ => _ != null);
    }
}