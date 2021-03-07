using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ActionWorkflow
{
    [DebuggerDisplay("Count = {" + nameof(_actions) + ".Count}")]
    public class ActionCollection<T> : IEnumerable<ActionInfo>
    {
        private readonly List<ActionCollectionEntry> _actions;

        public ActionCollection()
        {
            _actions = new List<ActionCollectionEntry>(4);
        }

        /// <summary>
        /// Adds the action with the given type to the collection.
        /// </summary>
        /// <typeparam name="TAction">The type of the action.</typeparam>
        /// <param name="priority">The priority of the action. Actions with a lower value will be executed first. Note that this only affects actions which are independent from each other.</param>
        public void AddAction<TAction>(int priority = 0) where TAction : IAction<T>
            => AddAction(typeof(TAction), priority);

        /// <summary>
        /// Adds the action with the given type to the collection.
        /// </summary>
        /// <param name="actionType">The type of the action.</param>
        /// <param name="priority">The priority of the action. Actions with a lower value will be executed first. Note that this only affects actions which are independent from each other.</param>
        public void AddAction(Type actionType, int priority = 0)
        {
            var actionInfo = ActionActivator.GetActionInfo(actionType);

            var indexToInsert = _actions.Count;
            for (int i = _actions.Count - 1; i >= 0; i--)
            {
                if (_actions[i].Priority <= priority)
                {
                    break;
                }

                indexToInsert = i;
            }

            _actions.Insert(indexToInsert, new ActionCollectionEntry(actionInfo, priority));
        }

        public IEnumerator<ActionInfo> GetEnumerator() => _actions.Select(x => x.ActionInfo).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        [DebuggerDisplay("Action = {" + nameof(ActionInfo) + ".Name}, Priority = {" + nameof(Priority) + "}")]
        private class ActionCollectionEntry
        {
            public ActionCollectionEntry(ActionInfo actionInfo, int priority)
            {
                ActionInfo = actionInfo ?? throw new ArgumentNullException(nameof(actionInfo));
                Priority = priority;
            }

            public ActionInfo ActionInfo { get; set; }

            public int Priority { get; set; }
        }
    }
}