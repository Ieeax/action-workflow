using ActionWorkflow.Tracing;
using System.Threading.Tasks;

namespace ActionWorkflow.Tests
{
    public class DummyActionTwo : IAction<IActionTracingContainer>
    {
        private readonly IActionContext _actionContext;

        public DummyActionTwo(IActionContext actionContext)
        {
            _actionContext = actionContext;
        }

        public Task ExecuteAsync(IActionTracingContainer context)
        {
            _actionContext.Export(new DummyActionTwoExport());

            return Task.CompletedTask;
        }
    }
}