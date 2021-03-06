using System;

namespace ActionWorkflow
{
    public interface IExportProvider : IReadOnlyExportProvider
    {
        bool TryExport(Type exportType, string name, object value);
    }
}