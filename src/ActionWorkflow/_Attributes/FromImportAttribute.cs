using System;

namespace ActionWorkflow
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class FromImportAttribute : Attribute
    {
        public FromImportAttribute()
        {
        }

        public FromImportAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}