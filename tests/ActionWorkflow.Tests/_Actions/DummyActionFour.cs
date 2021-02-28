using System.Threading;
using ActionWorkflow.Tracing;
using System.Threading.Tasks;

namespace ActionWorkflow.Tests
{
    public class DummyActionFour : IAction<IActionTracingContainer>
    {
        private readonly DummyActionOneExport _dummyActionOneExport;

        public DummyActionFour([FromImport] DummyActionOneExport dummyActionOneExport)
        {
            _dummyActionOneExport = dummyActionOneExport;
        }

        public Task ExecuteAsync(IActionTracingContainer context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}