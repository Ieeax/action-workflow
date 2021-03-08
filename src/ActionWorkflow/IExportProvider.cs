using System;

namespace ActionWorkflow
{
    public interface IExportProvider : IReadOnlyExportProvider
    {
        /// <summary>
        /// Tries to export the value for the given type and name.
        /// Ensure that the value inherits from the given type.
        /// </summary>
        /// <param name="exportType">The type of the export.</param>
        /// <param name="name">The name of the export. Pass <see langword="null"/> for the default export for the given type.</param>
        /// <param name="value">The value to export. Nees to inherit from the given type.</param>
        bool TryExport(Type exportType, string name, object value);
    }
}