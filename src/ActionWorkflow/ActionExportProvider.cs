using System;
using System.Collections;
using System.Collections.Generic;

namespace ActionWorkflow
{
    public class ActionExportProvider : IExportProvider
    {
        public const string DefaultExportName = "__DEFAULT_EXPORT";

        private readonly Dictionary<Type, Dictionary<string, object>> _exports = new Dictionary<Type, Dictionary<string, object>>();
        private readonly object _exportsLock = new object();
        private readonly bool _allowReexport;

        public ActionExportProvider(bool allowReexport = false)
        {
            _allowReexport = allowReexport;
        }

        public bool TryExport(Type exportType, string name, object value)
        {
            if (value == null)
            {
                return false;
            }

            name = name ?? DefaultExportName;

            lock (_exportsLock)
            {
                if (!_exports.TryGetValue(exportType, out var exports))
                {
                    exports = new Dictionary<string, object>();
                    _exports.Add(exportType, exports);
                }
                else if (!_allowReexport && exports.ContainsKey(name))
                {
                    return false;
                }

                exports[name] = value;
                return true;
            }
        }

        public bool ContainsExport(Type exportType, string name)
        {
            name = name ?? DefaultExportName;

            lock (_exportsLock)
            {
                return _exports.TryGetValue(exportType, out var exports) 
                    && exports.ContainsKey(name);
            }
        }

        public object GetExport(Type exportType, string name)
        {
            name = name ?? DefaultExportName;
            
            lock (_exportsLock)
            {
                if (_exports.TryGetValue(exportType, out var exports)
                    && exports.TryGetValue(name, out var value))
                {
                    return value;
                }
            }

            return null;
        }

        public IEnumerator<ActionExport> GetEnumerator()
        {
            lock (_exportsLock)
            {
                foreach (var curItem in _exports)
                {
                    foreach (var curExport in curItem.Value)
                    {
                        yield return new ActionExport(
                            curItem.Key, 
                            curExport.Key == DefaultExportName ? null : curExport.Key, 
                            curExport.Value);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}