using ActionWorkflow.Tracing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ActionWorkflow
{
    public class ActionSequence<T>
    {
        private readonly IList<ActionInfo> _actionInfos;
        private readonly int _actionInfosInitialCount;

        private readonly IServiceProvider _serviceProvider;
        private readonly IExportProvider _exportProvider;

        public ActionSequence(IList<ActionInfo> actionInfos, IExportProvider exportProvider = null)
            : this(actionInfos, null, exportProvider)
        {
        }

        public ActionSequence(IList<ActionInfo> actionInfos, IServiceProvider serviceProvider, IExportProvider exportProvider = null)
        {
            _actionInfos = actionInfos ?? throw new ArgumentNullException(nameof(actionInfos));
            _actionInfosInitialCount = actionInfos.Count;

            _serviceProvider = serviceProvider;
            _exportProvider = exportProvider ?? new ActionExportProvider();
        }

        private ActionBundle<T> GetNextActionBundle()
        {
            IServiceProvider serviceProvider = null;
            IActionContext context = null;

            List<IAction<T>> instances = null;

            for (int i = 0; i < _actionInfos.Count; i++)
            {
                var curActionInfo = _actionInfos[i];

                if (curActionInfo.TypesToImport.Count > 0)
                {
                    bool continueLoop = false;
                    foreach (var curImportType in curActionInfo.TypesToImport)
                    {
                        if (!_exportProvider.Contains(curImportType))
                        {
                            continueLoop = true;
                            break;
                        }
                    }

                    if (continueLoop)
                    {
                        continue;
                    }
                }

                if (instances == null)
                {
                    context = new DefaultActionContext();
                    serviceProvider = new DefaultActionServiceProvider(context, _exportProvider, _serviceProvider);
                    instances = new List<IAction<T>>();
                }

                instances.Add(
                    (IAction<T>)ActivatorUtilities.CreateInstance(
                        serviceProvider,
                        curActionInfo.ActionType));

                _actionInfos.RemoveAt(i--);
            }

            return instances == null 
                ? null 
                : new ActionBundle<T>(context, instances);
        }

        public async Task<ActionSequenceExecutionResult> ExecuteAsync(T context)
        {
            // Check whether the target wants to be able to trace which actions were executed on it
            var trace = context is IActionTracingContainer atc ? atc.ActionTrace : null;

            try
            {
                trace?.AddEvent(ActionTraceEvent.Begin, this.ToString());

                while (_actionInfos.Count > 0)
                {
                    var actionBundle = this.GetNextActionBundle();
                    if (actionBundle == null)
                    {
                        // Stop the execution because no further actions are possible
                        break;
                    }

                    // Execute all actions in the bundle
                    await actionBundle.ExecuteAsync(context);

                    // Add all exports to the export-provider
                    _exportProvider.AddRange(
                        actionBundle.ActionContext.Exports);
                }

                trace?.AddEvent(ActionTraceEvent.End, this.ToString());
            }
            catch (Exception ex)
            {
                trace?.AddEvent(ActionTraceEvent.UnexpectedEnd, this.ToString(), ex);
            }

            return _actionInfos.Count == _actionInfosInitialCount
                ? ActionSequenceExecutionResult.None
                : _actionInfos.Count > 0
                    ? ActionSequenceExecutionResult.Partial
                    : ActionSequenceExecutionResult.Full;
        }

        public override string ToString()
        {
            return $"{nameof(ActionSequence<T>)} ({_actionInfosInitialCount})";
        }
    }
}