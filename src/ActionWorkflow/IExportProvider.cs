using System;
using System.Collections.Generic;

namespace ActionWorkflow
{
    public interface IExportProvider
    {
        void AddRange(IEnumerable<object> values);

        bool Contains(Type exportType);

        object GetExport(Type exportType);
    }
}