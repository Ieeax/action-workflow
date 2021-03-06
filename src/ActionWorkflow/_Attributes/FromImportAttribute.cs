﻿using System;

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
            this.Name = name;
        }

        public string Name { get; set; }
    }
}