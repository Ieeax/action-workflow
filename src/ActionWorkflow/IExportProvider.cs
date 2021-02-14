using System;

namespace ActionWorkflow
{
    public interface IExportProvider
    {
        bool TryExport(Type exportType, object value);

        bool ContainsExport(Type exportType);

        object GetExport(Type exportType);
    }
}