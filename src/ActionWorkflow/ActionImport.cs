using System;

namespace ActionWorkflow
{
    public class ActionImport
    {
        public ActionImport(Type type, string name, int constructorIndex)
        {
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.Name = name;

            if (constructorIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(constructorIndex));
            }

            this.ConstructorIndex = constructorIndex;
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