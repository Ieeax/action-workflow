using System;

namespace ActionWorkflow
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class FromImportAttribute : Attribute
    {
    }
}