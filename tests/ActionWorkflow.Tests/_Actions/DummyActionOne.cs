using System.Threading.Tasks;

namespace ActionWorkflow.Tests
{
    public class DummyActionOne : IAction<string>
    {
        public Task ExecuteAsync(string context)
        {
            return Task.CompletedTask;
        }
    }
}