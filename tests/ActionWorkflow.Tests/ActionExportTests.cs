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
            var builder = new ActionSequenceFactoryBuilder<IActionTracingContainer>();
            builder.AddAction<DummyActionTwo>();
            builder.AddAction<DummyActionOne>();

            var exportProvider = new ActionExportProvider();
            var sequenceFactory = builder.ToFactory();
            var sequence = sequenceFactory.Create(exportProvider);

            var result = await sequence.ExecuteAsync(null);

            Assert.Equal(ActionSequenceExecutionResult.Full, result);
            Assert.NotNull(exportProvider.GetExport(typeof(DummyActionTwoExport)));
        }
    }
}