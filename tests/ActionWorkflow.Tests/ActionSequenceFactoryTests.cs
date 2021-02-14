using ActionWorkflow.Tracing;
using FakeItEasy;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ActionWorkflow.Tests
{
    public class ActionSequenceFactoryTests
    {
        private IActionTracingContainer GetMockedTracingContainer()
        {
            var trace = new ActionTrace();

            var traceContainer = A.Fake<IActionTracingContainer>();
            A.CallTo(() => traceContainer.ActionTrace).Returns(trace);

            return traceContainer;
        }

        private bool MatchTraceEntry(IActionTracingContainer container, int index, ActionTraceEvent eventType, string identifier)
        {
            return container.ActionTrace.Entries.ElementAt(index).EventType == eventType
                && container.ActionTrace.Entries.ElementAt(index).Identifier == identifier;
        }

        [Fact]
        public async Task ActionSequenceFullExecutionTest()
        {
            var builder = new ActionSequenceFactoryBuilder<IActionTracingContainer>();
            builder.AddAction<DummyActionTwo>();
            builder.AddAction<DummyActionOne>();

            var sequenceFactory = builder.ToFactory();
            var sequence = sequenceFactory.Create();

            var container = this.GetMockedTracingContainer();
            var result = await sequence.ExecuteAsync(container);

            Assert.Equal(ActionSequenceExecutionResult.Full, result);
            Assert.True(this.MatchTraceEntry(container, 0, ActionTraceEvent.Begin, "ActionSequence (2)"));
            Assert.True(this.MatchTraceEntry(container, 1, ActionTraceEvent.Begin, "ActionBundle (1)"));
            Assert.True(this.MatchTraceEntry(container, 2, ActionTraceEvent.Begin, "DummyActionTwo"));
            Assert.True(this.MatchTraceEntry(container, 3, ActionTraceEvent.End, "DummyActionTwo"));
            Assert.True(this.MatchTraceEntry(container, 4, ActionTraceEvent.End, "ActionBundle (1)"));
            Assert.True(this.MatchTraceEntry(container, 5, ActionTraceEvent.Begin, "ActionBundle (1)"));
            Assert.True(this.MatchTraceEntry(container, 6, ActionTraceEvent.Begin, "DummyActionOne"));
            Assert.True(this.MatchTraceEntry(container, 7, ActionTraceEvent.End, "DummyActionOne"));
            Assert.True(this.MatchTraceEntry(container, 8, ActionTraceEvent.End, "ActionBundle (1)"));
            Assert.True(this.MatchTraceEntry(container, 9, ActionTraceEvent.End, "ActionSequence (2)"));
        }

        [Fact]
        public async Task ActionSequencePartialExecutionTest()
        {
            var builder = new ActionSequenceFactoryBuilder<IActionTracingContainer>();
            builder.AddAction<DummyActionTwo>();
            builder.AddAction<DummyActionFour>();

            var sequenceFactory = builder.ToFactory();
            var sequence = sequenceFactory.Create();

            var container = this.GetMockedTracingContainer();
            var result = await sequence.ExecuteAsync(container);

            Assert.Equal(ActionSequenceExecutionResult.Partial, result);
            Assert.True(this.MatchTraceEntry(container, 0, ActionTraceEvent.Begin, "ActionSequence (2)"));
            Assert.True(this.MatchTraceEntry(container, 1, ActionTraceEvent.Begin, "ActionBundle (1)"));
            Assert.True(this.MatchTraceEntry(container, 2, ActionTraceEvent.Begin, "DummyActionTwo"));
            Assert.True(this.MatchTraceEntry(container, 3, ActionTraceEvent.End, "DummyActionTwo"));
            Assert.True(this.MatchTraceEntry(container, 4, ActionTraceEvent.End, "ActionBundle (1)"));
            Assert.True(this.MatchTraceEntry(container, 5, ActionTraceEvent.End, "ActionSequence (2)"));
        }

        [Fact]
        public async Task ActionSequenceNoneExecutionTest()
        {
            var builder = new ActionSequenceFactoryBuilder<IActionTracingContainer>();
            builder.AddAction<DummyActionThree>();
            builder.AddAction<DummyActionOne>();

            var sequenceFactory = builder.ToFactory();
            var sequence = sequenceFactory.Create();

            var container = this.GetMockedTracingContainer();
            var result = await sequence.ExecuteAsync(container);

            Assert.Equal(ActionSequenceExecutionResult.None, result);
            Assert.True(this.MatchTraceEntry(container, 0, ActionTraceEvent.Begin, "ActionSequence (2)"));
            Assert.True(this.MatchTraceEntry(container, 1, ActionTraceEvent.End, "ActionSequence (2)"));
        }
    }
}