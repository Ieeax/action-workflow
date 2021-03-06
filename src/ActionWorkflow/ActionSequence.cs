using ActionWorkflow.Tracing;
using System;
using System.Collections.Generic;
using System.Threading;
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

        private ActionBundle<T> GetNextActionBundle(CancellationToken cancellationToken)
        {
            List<ActionItem<T>> entries = null;

            for (int i = 0; i < _actionInfos.Count; i++)
            {
                var curActionInfo = _actionInfos[i];

                if (curActionInfo.Imports.Count > 0)
                {
                    bool continueLoop = false;
                    foreach (var curImport in curActionInfo.Imports)
                    {
                        if (!_exportProvider.ContainsExport(curImport.Type, curImport.Name))
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

                if (entries == null)
                {
                    entries = new List<ActionItem<T>>();
                }

                entries.Add(
                    this.CreateActionItem(curActionInfo));

                _actionInfos.RemoveAt(i--);
            }

            return entries == null 
                ? null 
                : new ActionBundle<T>(entries, cancellationToken);
        }

        private ActionItem<T> CreateActionItem(ActionInfo actionInfo)
        {
            var actionContext = new DefaultActionContext(actionInfo, _exportProvider);
            var serviceProvider = new DefaultActionServiceProvider(actionContext, _serviceProvider);

            return new ActionItem<T>(
                (IAction<T>)ActionActivator.CreateInstance(actionInfo, serviceProvider, _exportProvider),
                actionInfo,
                actionContext,
                _exportProvider,
                serviceProvider);
        }

        /// <summary>
        /// Executes the sequence for the given <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The item for which the sequence is executed.</param>
        /// <exception cref="ActionException"></exception>
        public Task<ActionSequenceExecutionResult> ExecuteAsync(T context)
            => this.ExecuteAsync(context, CancellationToken.None);

        /// <summary>
        /// Executes the sequence for the given <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The item for which the sequence is executed.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> which indicates whether the sequence should be canceled.</param>
        /// <exception cref="ActionException"></exception>
        public async Task<ActionSequenceExecutionResult> ExecuteAsync(T context, CancellationToken cancellationToken)
        {
            // Check whether the target wants to be able to trace which actions were executed on it
            var trace = context is IActionTracingContainer atc ? atc.ActionTrace : null;

            try
            {
                trace?.AddEvent(ActionTraceEvent.Begin, this.ToString());

                while (_actionInfos.Count > 0)
                {
                    var actionBundle = this.GetNextActionBundle(cancellationToken);
                    if (actionBundle == null)
                    {
                        // Stop the execution because no further actions are possible
                        break;
                    }

                    // Execute all actions in the bundle
                    await actionBundle.ExecuteAsync(context);
                }

                trace?.AddEvent(ActionTraceEvent.End, this.ToString());
            }
            catch (Exception ex)
            {
                trace?.AddEvent(ActionTraceEvent.UnexpectedEnd, this.ToString(), ex);
                
                if (ex is ActionException) throw;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return ActionSequenceExecutionResult.Canceled;
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