using System;

namespace ActionWorkflow
{
    public class ActionImportInfo
    {
        public ActionImportInfo(Type type, string name, int constructorIndex)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Name = name;

            if (constructorIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(constructorIndex));
            }

            ConstructorIndex = constructorIndex;
        }

        /// <summary>
        /// Gets the type of the import.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the name of the import.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the index of the import in the constructor.
        /// </summary>
        public int ConstructorIndex { get; }
    }
}