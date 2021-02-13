using System;

namespace ActionWorkflow.Tracing
{
    public class ActionTraceEntry
    {
        public ActionTraceEntry(ActionTraceEvent eventType, string identifier)
            : this(eventType, identifier, null)
        {
        }

        public ActionTraceEntry(ActionTraceEvent eventType, string identifier, Exception exception)
        {
            EventType = eventType;
            Identifier = identifier;
            Exception = exception;
        }

        public ActionTraceEvent EventType { get; }

        public string Identifier { get; }

        public Exception Exception { get; }
    }
}