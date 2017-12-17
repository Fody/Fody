using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil.Cil;

namespace Fody
{
    [Obsolete(OnlyForTesting.Message)]
    public class TestResult
    {
        List<LogMessage> messages = new List<LogMessage>();

        internal void AddMessage(string text, MessageImportance messageImportance)
        {
            var message = new LogMessage
            {
                Text = text,
                MessageImportance = messageImportance
            };
            messages.Add(message);
        }

        public IReadOnlyList<LogMessage> Messages => messages;


        List<SequencePointMessage> warnings = new List<SequencePointMessage>();

        internal void AddWarning(string text, SequencePoint sequencePoint)
        {
            var message = new SequencePointMessage
            {
                Text = text,
                SequencePoint = sequencePoint
            };
            warnings.Add(message);
        }

        public IReadOnlyList<SequencePointMessage> Warnings => warnings;

        List<SequencePointMessage> errors = new List<SequencePointMessage>();

        internal void AddError(string text, SequencePoint sequencePoint)
        {
            var message = new SequencePointMessage
            {
                Text = text,
                SequencePoint = sequencePoint
            };
            errors.Add(message);
        }

        public IReadOnlyList<SequencePointMessage> Errors => errors;
        public Assembly Assembly { get; internal set; }
    }
}