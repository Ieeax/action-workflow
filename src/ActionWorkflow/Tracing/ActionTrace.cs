using System;
using System.Collections.Generic;

namespace ActionWorkflow.Tracing
{
    public class ActionTrace
    {
        private readonly List<ActionTraceEntry> _entries = new List<ActionTraceEntry>();

        public void AddEvent(ActionTraceEvent eventType, string identifier)
        {
            _entries.Add(new ActionTraceEntry(eventType, identifier));
        }

        public void AddEvent(ActionTraceEvent eventType, string identifier, Exception exception)
        {
            _entries.Add(new ActionTraceEntry(eventType, identifier, exception));
        }

        public IReadOnlyCollection<ActionTraceEntry> Entries => _entries;
    }
}