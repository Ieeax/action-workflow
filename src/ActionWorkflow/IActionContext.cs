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
        /// Exports the given <paramref name="value"/>.
        /// Note that each type can only be exported once.
        /// </summary>
        /// <param name="value">The value to export.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        void Export(object value);

        /// <summary>
        /// Exports the given <paramref name="value"/> as type <typeparamref name="T"/>.
        /// Note that each type can only be exported once.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value to export.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        void Export<T>(T value);

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
        IReadOnlyDictionary<Type, object> Exports { get; }
    }
}