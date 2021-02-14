using ActionWorkflow.Tracing;
using System.Threading.Tasks;

namespace ActionWorkflow.Tests
{
    public class DummyActionFour : IAction<IActionTracingContainer>
    {
        private readonly DummyActionOneExport _dummyActionOneExport;

        public DummyActionFour(DummyActionOneExport dummyActionOneExport)
        {
            _dummyActionOneExport = dummyActionOneExport;
        }

        public Task ExecuteAsync(IActionTracingContainer context)
        {
            return Task.CompletedTask;
        }
    }
}