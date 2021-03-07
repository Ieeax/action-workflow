using System;
using System.Collections.Generic;
using System.Reflection;

namespace ActionWorkflow
{
    public class ActionInfo
    {
        private readonly List<ActionImport> _imports;

        internal ActionInfo(Type actionType, ConstructorInfo constructor, List<ActionImport> imports)
            : this(actionType, null, null, constructor, imports)
        {
        }

        internal ActionInfo(Type actionType, string friendlyName, string description, ConstructorInfo constructor, List<ActionImport> imports)
        {
            ActionType = actionType ?? throw new ArgumentNullException(nameof(actionType));
            Name = friendlyName ?? actionType.Name;
            FriendlyName = friendlyName;
            Description = description;
            Constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            _imports = imports ?? throw new ArgumentNullException(nameof(imports));
        }

        public override string ToString()
        {
            return this.Name + (this.Description == null ? null : $"({this.Description})");
        }

        /// <summary>
        /// Gets the type of the action.
        /// </summary>
        public Type ActionType { get; }

        /// <summary>
        /// Gets the name of the action.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the friendly name of the action.
        /// Can be set through the <see cref="ActionAttribute" />.
        /// </summary>
        public string FriendlyName { get; }

        /// <summary>
        /// Gets the description of the action.
        /// Can be set through the <see cref="ActionAttribute" />.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the constructor which is used to create the action.
        /// </summary>
        public ConstructorInfo Constructor { get; }

        /// <summary>
        /// Gets the collection of required imports for the action.
        /// </summary>
        public IReadOnlyCollection<ActionImport> Imports => _imports;
    }
}