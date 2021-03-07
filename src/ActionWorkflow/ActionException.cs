using System;

namespace ActionWorkflow
{
    public class ActionException : Exception
    {
        public ActionException(string message)
            : this(message, null, null)
        {
        }

        public ActionException(string message, ActionInfo actionInfo)
            : this(message, null, actionInfo)
        {
        }

        public ActionException(string message, Exception innerException, ActionInfo actionInfo)
            : base(message, innerException)
        {
            ActionInfo = actionInfo;
        }

        /// <summary>
        /// Gets infos about the action from which the exception originated.
        /// </summary>
        public ActionInfo ActionInfo { get; }
    }
}