using Mono.Cecil.Cil;

namespace Fody
{
    /// <summary>
    /// Only for test usage. Only for development purposes when building Fody addins. The API may change in minor releases.
    /// </summary>
    public class SequencePointMessage
    {
        public SequencePointMessage(string text, SequencePoint? sequencePoint)
        {
            Text = text;
            SequencePoint = sequencePoint;
        }

        public string Text { get; }
        public SequencePoint? SequencePoint { get; }
    }
}