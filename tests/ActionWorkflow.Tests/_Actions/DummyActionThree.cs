using ActionWorkflow.Tracing;
using System.Threading.Tasks;

namespace ActionWorkflow.Tests
{
    public class DummyActionThree : IAction<IActionTracingContainer>
    {
        private readonly DummyActionTwoExport _dummyActionTwoExport;

        public DummyActionThree(DummyActionTwoExport dummyActionTwoExport)
        {
            _dummyActionTwoExport = dummyActionTwoExport;
        }

        public Task ExecuteAsync(IActionTracingContainer context)
        {
            return Task.CompletedTask;
        }
    }
}