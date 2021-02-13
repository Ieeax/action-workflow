using System;
using System.Collections.Generic;

namespace ActionWorkflow
{
    public class ActionSequenceFactoryBuilder<T>
    {
        private readonly ICollection<Type> _actions;

        public ActionSequenceFactoryBuilder()
        {
            _actions = new List<Type>();
        }

        public void AddAction<TAction>()
            where TAction : IAction<T>
        {
            _actions.Add(typeof(TAction));
        }

        public void AddAction(Type actionType)
        {
            if (!typeof(IAction<T>).IsAssignableFrom(actionType))
            {
                throw new InvalidOperationException($"Type \"{actionType.FullName}\" does not inherit from \"{typeof(IAction<T>).FullName}\".");
            }

            _actions.Add(actionType);
        }

        public ICollection<Type> ToCollection() => _actions;

        public ActionSequenceFactory<T> ToFactory()
            => ActionSequenceFactory<T>.CreateFactory(_actions);
    }
}