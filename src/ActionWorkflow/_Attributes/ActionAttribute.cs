using System;

namespace ActionWorkflow
{
    public class ActionAttribute : Attribute
    {
        public ActionAttribute(string friendlyName)
            : this(friendlyName, null)
        {
        }

        public ActionAttribute(string friendlyName, string description)
        {
            FriendlyName = friendlyName;
            Description = description;
        }

        public string FriendlyName { get; set; }

        public string Description { get; set; }
    }
}