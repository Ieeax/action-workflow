using ActionWorkflow.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ActionWorkflow
{
    public class ActionBundle<T>
    {
        private readonly IEnumerable<ActionItem<T>> _items;
        private readonly CancellationToken _cancellationToken;

        public ActionBundle(IEnumerable<ActionItem<T>> items, CancellationToken cancellationToken)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
            _cancellationToken = cancellationToken;
        }

        public async Task ExecuteAsync(T context)
        {
            // Check whether the target wants to be able to trace which actions were executed on it
            var trace = context is IActionTracingContainer atc ? atc.ActionTrace : null;

            try
            {
                trace?.AddEvent(ActionTraceEvent.Begin, this.ToString());

                foreach (var curItem in _items)
                {
                    if (curItem == null)
                    {
                        continue;
                    }

                    // Build the action-identifier for tracing
                    var actionIdentifier = trace == null
                        ? null
                        : curItem.ActionContext.Name + (curItem.ActionContext.Description == null ? null : $"({curItem.ActionContext.Description})");

                    try
                    {
                        trace?.AddEvent(ActionTraceEvent.Begin, actionIdentifier);

                        await curItem.Action.ExecuteAsync(context, _cancellationToken);

                        // Add all exports to the global export-provider when the action finished successfully
                        // -> If the action would throw an exception we don't want to add already exported objects to it
                        foreach (var curExport in curItem.ActionContext.ExportProvider)
                        {
                            if (!curItem.ExportProvider.TryExport(curExport.Type, curExport.Name, curExport.Value))
                            {
                                throw new InvalidOperationException($"An object of type \"{curExport.Type.FullName}\" with name \"{curExport.Name}\" was already exported.");
                            }
                        }

                        trace?.AddEvent(ActionTraceEvent.End, actionIdentifier);
                    }
                    catch (Exception ex)
                    {
                        trace?.AddEvent(ActionTraceEvent.UnexpectedEnd, actionIdentifier, ex);
                    }
                }

                trace?.AddEvent(ActionTraceEvent.End, this.ToString());
            }
            catch (Exception ex)
            {
                trace?.AddEvent(ActionTraceEvent.UnexpectedEnd, this.ToString(), ex);
            }
        }

        public override string ToString()
        {
            return $"{nameof(ActionBundle<T>)} ({_items.Count()})";
        }
    }
}