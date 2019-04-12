using Mono.Cecil.Cil;

namespace Fody
{
    /// <summary>
    /// Only for test usage. Only for development purposes when building Fody addins. The API may change in minor releases.
    /// </summary>
    public class SequencePointMessage
    {
        public string Text { get; internal set; }
        public SequencePoint SequencePoint { get; internal set; }
    }
}