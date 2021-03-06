using System;
using System.Collections.Generic;

namespace ActionWorkflow
{
    public class ActionSequenceFactory<T>
    {
        private readonly IList<ActionInfo> _actionInfos;

        private ActionSequenceFactory(IList<ActionInfo> actionInfos)
        {
            _actionInfos = actionInfos;
        }

        public static ActionSequenceFactory<T> CreateFactory(ICollection<Type> actions)
        {
            if (actions == null)
            {
                throw new ArgumentNullException(nameof(actions));
            }

            var actionInfos = new List<ActionInfo>(actions.Count);

            foreach (var curActionType in actions)
            {
                actionInfos.Add(ActionActivator.GetActionInfo(curActionType));
            }

            return new ActionSequenceFactory<T>(actionInfos);
        }

        public ActionSequence<T> Create(IExportProvider exportProvider = null)
            => new ActionSequence<T>(_actionInfos, exportProvider);

        public ActionSequence<T> Create(IServiceProvider serviceProvider, IExportProvider exportProvider = null)
            => new ActionSequence<T>(_actionInfos, serviceProvider, exportProvider);
    }
}