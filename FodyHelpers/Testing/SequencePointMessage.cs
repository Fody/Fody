using Mono.Cecil.Cil;

namespace Fody;

/// <summary>
/// Only for test usage. Only for development purposes when building Fody addins. The API may change in minor releases.
/// </summary>
public class SequencePointMessage(string text, SequencePoint? sequencePoint)
{
    public string Text { get; } = text;
    public SequencePoint? SequencePoint { get; } = sequencePoint;
}