using ActionWorkflow.Tracing;
using System.Threading.Tasks;
using Xunit;

namespace ActionWorkflow.Tests
{
    public class ActionExportTests
    {
        [Fact]
        public void ExportProviderWithNamedExportsTest()
        {
            var exportProvider = new ActionExportProvider(false);

            Assert.True(exportProvider.TryExport(typeof(string), null, "TEST"));
            Assert.True(exportProvider.TryExport(typeof(string), "STRING_EXPORT", "TEST2"));
            Assert.True(exportProvider.TryExport(typeof(int), "INT_EXPORT", 123));
            Assert.True(exportProvider.TryExport(typeof(int), "INT_EXPORT2", 321));

            Assert.Equal("TEST", exportProvider.GetExport(typeof(string), null));
            Assert.Equal("TEST2", exportProvider.GetExport(typeof(string), "STRING_EXPORT"));
            Assert.Equal(123, exportProvider.GetExport(typeof(int), "INT_EXPORT"));
            Assert.Equal(321, exportProvider.GetExport(typeof(int), "INT_EXPORT2"));
            Assert.Null(exportProvider.GetExport(typeof(int), null));
        }

        [Fact]
        public void ExportProviderWithoutReexportTest()
        {
            var exportProvider = new ActionExportProvider(false);

            Assert.True(exportProvider.TryExport(typeof(int), null, 123));
            Assert.False(exportProvider.TryExport(typeof(int), null, 321));
            
            Assert.Equal(123, exportProvider.GetExport(typeof(int), null));
        }

        [Fact]
        public void ExportProviderWithReexportTest()
        {
            var exportProvider = new ActionExportProvider(true);

            Assert.True(exportProvider.TryExport(typeof(int), null, 123));
            Assert.True(exportProvider.TryExport(typeof(int), null, 321));

            Assert.Equal(321, exportProvider.GetExport(typeof(int), null));
        }

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