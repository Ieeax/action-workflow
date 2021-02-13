using ActionWorkflow.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActionWorkflow
{
    public class ActionBundle<T>
    {
        private readonly IActionContext _context;
        private readonly IEnumerable<IAction<T>> _actions;

        public ActionBundle(IActionContext context, IEnumerable<IAction<T>> actions)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
        }

        public async Task ExecuteAsync(T context)
        {
            // Check whether the target wants to be able to trace which actions were executed on it
            var trace = context is IActionTracingContainer atc ? atc.ActionTrace : null;

            try
            {
                trace?.AddEvent(ActionTraceEvent.Begin, this.ToString());

                foreach (var curAction in _actions)
                {
                    if (curAction == null)
                    {
                        continue;
                    }

                    // TODO: Find a faster/optimized way to determine the action-info
                    var actionInfo = ActionSequenceFactory<T>.GetActionInfo(curAction.GetType());
                    var actionIdentifier = actionInfo.ToString();

                    try
                    {
                        trace?.AddEvent(ActionTraceEvent.Begin, actionIdentifier);

                        await curAction.ExecuteAsync(context);

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
            return $"{nameof(ActionBundle<T>)} ({_actions.Count()})";
        }

        public IActionContext ActionContext => _context;
    }
}