using ActionWorkflow.Tracing;
using System.Threading.Tasks;
using Xunit;

namespace ActionWorkflow.Tests
{
    public class ActionExportTests
    {
        [Fact]
        public async Task ActionSimpleExportTest()
        {
            var collection = new ActionCollection<IActionTracingContainer>();
            collection.AddAction<DummyActionTwo>();
            collection.AddAction<DummyActionOne>();

            var exportProvider = new ActionExportProvider();
            var sequence = new ActionSequence<IActionTracingContainer>(collection, exportProvider);

            var result = await sequence.ExecuteAsync(null);

            Assert.Equal(ActionSequenceExecutionResult.Full, result);
            Assert.NotNull(exportProvider.GetExport(typeof(DummyActionTwoExport), null));
        }
    }
}