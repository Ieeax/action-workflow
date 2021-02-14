using System;
using System.Collections.Concurrent;

namespace ActionWorkflow
{
    public class ActionExportProvider : IExportProvider
    {
        private readonly ConcurrentDictionary<Type, object> _exports = new ConcurrentDictionary<Type, object>();

        public bool TryExport(Type exportType, object value)
            => _exports.TryAdd(exportType, value);

        public bool ContainsExport(Type exportType)
            => _exports.ContainsKey(exportType);

        public object GetExport(Type exportType)
            => _exports.TryGetValue(exportType, out var export) ? export : null;
    }
}