using System;
using System.Collections.Generic;

namespace ActionWorkflow
{
    public class DefaultActionContext : IActionContext
    {
        private readonly Dictionary<Type, object> _exports = new Dictionary<Type, object>();
        private readonly IExportProvider _globalExportProvider;
        private readonly ActionInfo _actionInfo;

        public DefaultActionContext(IExportProvider globalExportProvider, ActionInfo actionInfo)
        {
            _globalExportProvider = globalExportProvider ?? throw new ArgumentNullException(nameof(globalExportProvider));
            _actionInfo = actionInfo ?? throw new ArgumentNullException(nameof(actionInfo));
        }

        public void Export(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var type = value.GetType();

            if (_globalExportProvider.ContainsExport(type)
                || _exports.ContainsKey(type))
            {
                throw new InvalidOperationException($"An object of type \"{type.FullName}\" was already exported. Each type can only be exported once.");
            }

            _exports.Add(type, value);
        }

        public void Export<T>(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var type = typeof(T);

            if (_globalExportProvider.ContainsExport(type)
                || _exports.ContainsKey(type))
            {
                throw new InvalidOperationException($"An object of type \"{type.FullName}\" was already exported. Each type can only be exported once.");
            }

            _exports.Add(type, value);
        }

        public string Name => _actionInfo.Name;

        public string Description => _actionInfo.Description;

        public IReadOnlyDictionary<Type, object> Exports => _exports;
    }
}