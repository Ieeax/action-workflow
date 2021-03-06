using System;
using System.Collections.Generic;

namespace ActionWorkflow
{
    public interface IReadOnlyExportProvider : IEnumerable<ActionExport>
    {
        bool ContainsExport(Type exportType, string name);

        object GetExport(Type exportType, string name);
    }
}