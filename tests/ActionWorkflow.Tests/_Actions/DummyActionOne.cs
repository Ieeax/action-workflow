using System.Threading;
using ActionWorkflow.Tracing;
using System.Threading.Tasks;

namespace ActionWorkflow.Tests
{
    public class DummyActionOne : IAction<IActionTracingContainer>
    {
        private readonly DummyActionTwoExport _dummyActionTwoExport;

        public DummyActionOne([FromImport] DummyActionTwoExport dummyActionTwoExport)
        {
            _dummyActionTwoExport = dummyActionTwoExport;
        }

        public Task ExecuteAsync(IActionTracingContainer context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}