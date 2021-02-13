using System.Collections.Generic;

namespace ActionWorkflow
{
    public interface IActionContext
    {
        void Export(object value);

        IReadOnlyCollection<object> Exports { get; }
    }
}