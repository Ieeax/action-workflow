using System;
using System.Collections.Generic;

namespace ActionWorkflow
{
    public interface IReadOnlyExportProvider : IEnumerable<ActionExport>
    {
        /// <summary>
        /// Checks whether the export with the given type and name exists.
        /// </summary>
        /// <param name="exportType">The type of the export.</param>
        /// <param name="name">The name of the export. Pass <see langword="null"/> to check for a default export for the given type.</param>
        bool ContainsExport(Type exportType, string name);

        /// <summary>
        /// Gets the export for the given type and name. Returns <see langword="null"/> if not found.
        /// </summary>
        /// <param name="exportType">The type of the export.</param>
        /// <param name="name">The name of the export. Pass <see langword="null"/> to get the default export for the given type.</param>
        object GetExport(Type exportType, string name);
    }
}