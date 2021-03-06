using System;
using System.Collections.Generic;

namespace ActionWorkflow
{
    public class DefaultActionContext : IActionContext
    {
        private readonly IExportProvider _temporaryExportProvider;
        private readonly IExportProvider _globalExportProvider;
        private readonly ActionInfo _actionInfo;

        public DefaultActionContext(ActionInfo actionInfo, IExportProvider globalExportProvider)
        {
            _actionInfo = actionInfo ?? throw new ArgumentNullException(nameof(actionInfo));
            _globalExportProvider = globalExportProvider ?? throw new ArgumentNullException(nameof(globalExportProvider));
            _temporaryExportProvider = new ActionExportProvider();
        }
        
        public void Export(object value)
            => this.Export(value, null);

        public void Export(object value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var type = value.GetType();

            if (_globalExportProvider.ContainsExport(type, name)
                || !_temporaryExportProvider.TryExport(type, name, value))
            {
                throw new InvalidOperationException($"An object of type \"{type.FullName}\" with name \"{name}\" was already exported.");
            }
        }

        public void Export<T>(T value)
            => this.Export<T>(value, null);

        public void Export<T>(T value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var type = typeof(T);

            if (_globalExportProvider.ContainsExport(type, name)
                || !_temporaryExportProvider.TryExport(type, name, value))
            {
                throw new InvalidOperationException($"An object of type \"{type.FullName}\" with name \"{name}\" was already exported.");
            }
        }

        public string Name => _actionInfo.Name;

        public string Description => _actionInfo.Description;

        public IReadOnlyExportProvider ExportProvider => _temporaryExportProvider;
    }
}