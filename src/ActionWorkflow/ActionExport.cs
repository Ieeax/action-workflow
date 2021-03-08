using System;

namespace ActionWorkflow
{
    public class ActionExport
    {
        public ActionExport(Type type, string name, object value)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets the type of the export.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the name of the export.
        /// </summary>
        public string Name { get; }
    
        /// <summary>
        /// Gets the exported value.
        /// </summary>
        public object Value { get; }
    
        /// <summary>
        /// Gets whether this is a default export.
        /// </summary>
        public bool IsDefault => Name == null;
    }
}