using System;
using System.Collections.Generic;

namespace ActionWorkflow
{
    public class ActionInfo
    {
        private readonly List<Type> _typesToImport;

        public ActionInfo(Type actionType, List<Type> typesToImport)
            : this(actionType, null, null, typesToImport)
        {
        }

        public ActionInfo(Type actionType, string friendlyName, string description, List<Type> typesToImport)
        {
            ActionType = actionType ?? throw new ArgumentNullException(nameof(actionType));
            Name = friendlyName ?? actionType.Name;
            FriendlyName = friendlyName;
            Description = description;
            _typesToImport = typesToImport ?? throw new ArgumentNullException(nameof(typesToImport));
        }

        public override string ToString()
        {
            return this.Name + (this.Description == null ? null : $"({this.Description})");
        }

        public Type ActionType { get; }

        public string Name { get; }

        public string FriendlyName { get; }

        public string Description { get; }

        public IReadOnlyCollection<Type> TypesToImport => _typesToImport;
    }
}