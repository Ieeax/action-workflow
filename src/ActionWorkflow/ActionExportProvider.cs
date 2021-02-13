using System;
using System.Collections.Generic;

namespace ActionWorkflow
{
    public class ActionExportProvider : IExportProvider
    {
        private readonly Dictionary<Type, object> _exports = new Dictionary<Type, object>();

        public void AddRange(IEnumerable<object> values)
        {
            foreach (var curItem in values)
            {
                if (curItem == null)
                {
                    continue;
                }

                _exports.Add(curItem.GetType(), curItem);
            }
        }

        public bool Contains(Type exportType)
            => _exports.ContainsKey(exportType);

        public object GetExport(Type exportType)
            => _exports.TryGetValue(exportType, out var export) ? export : null;
    }
}