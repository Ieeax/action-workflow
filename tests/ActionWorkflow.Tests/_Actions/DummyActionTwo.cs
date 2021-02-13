using System.Threading.Tasks;

namespace ActionWorkflow.Tests
{
    public class DummyActionTwo : IAction<string>
    {
        public Task ExecuteAsync(string context)
        {
            return Task.CompletedTask;
        }
    }
}