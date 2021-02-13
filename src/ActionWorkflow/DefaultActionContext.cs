using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ActionWorkflow
{
    public class DefaultActionContext : IActionContext
    {
        private readonly ConcurrentBag<object> _exports = new ConcurrentBag<object>();

        public void Export(object value)
            => _exports.Add(value);

        public IReadOnlyCollection<object> Exports => _exports;
    }
}