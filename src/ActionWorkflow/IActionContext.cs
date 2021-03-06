using System;
using System.Collections.Generic;

namespace ActionWorkflow
{
    /// <summary>
    /// Provides access to contextual functions and informations, scoped to the current action.
    /// </summary>
    public interface IActionContext
    {
        /// <summary>
        /// Exports the given <paramref name="value"/> as default export.
        /// Note that a default export can only be done once per type.
        /// </summary>
        /// <param name="value">The value to export.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        void Export(object value);

        /// <summary>
        /// Exports the given <paramref name="value"/> as named export.
        /// This allows multiple exports of the same type as long the name differs.
        /// </summary>
        /// <param name="value">The value to export.</param>
        /// <param name="name">The name of the export.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        void Export(object value, string name);

        /// <summary>
        /// Exports the given <paramref name="value"/> as default export of type <typeparamref name="T"/>.
        /// Note that a default export can only be done once per type.
        /// </summary>
        /// <typeparam name="T">The type of the export.</typeparam>
        /// <param name="value">The value to export.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        void Export<T>(T value);

        /// <summary>
        /// Exports the given <paramref name="value"/> as named export of type <typeparamref name="T"/>.
        /// This allows multiple exports of the same type as long the name differs.
        /// </summary>
        /// <typeparam name="T">The type of the export.</typeparam>
        /// <param name="value">The value to export.</param>
        /// <param name="name">The name of the export.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        void Export<T>(T value, string name);

        /// <summary>
        /// Gets the name of the action.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description of the action.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the already exported values from the action.
        /// </summary>
        IReadOnlyExportProvider ExportProvider { get; }
    }
}