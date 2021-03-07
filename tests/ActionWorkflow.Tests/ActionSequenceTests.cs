using ActionWorkflow.Tracing;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ActionWorkflow.Tests
{
    public class ActionSequenceTests
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
            var collection = new ActionCollection<IActionTracingContainer>();
            collection.AddAction<DummyActionTwo>();
            collection.AddAction<DummyActionOne>();

            var sequence = new ActionSequence<IActionTracingContainer>(collection);

            var container = this.GetMockedTracingContainer();
            var result = await sequence.ExecuteAsync(container);

            Assert.Equal(ActionSequenceExecutionResult.Full, result);
            Assert.True(this.MatchTraceEntry(container, 0, ActionTraceEvent.Begin, "ActionSequence (2)"));
            Assert.True(this.MatchTraceEntry(container, 1, ActionTraceEvent.Begin, "ActionBundle (1)"));
            Assert.True(this.MatchTraceEntry(container, 2, ActionTraceEvent.Begin, nameof(DummyActionTwo)));
            Assert.True(this.MatchTraceEntry(container, 3, ActionTraceEvent.End, nameof(DummyActionTwo)));
            Assert.True(this.MatchTraceEntry(container, 4, ActionTraceEvent.End, "ActionBundle (1)"));
            Assert.True(this.MatchTraceEntry(container, 5, ActionTraceEvent.Begin, "ActionBundle (1)"));
            Assert.True(this.MatchTraceEntry(container, 6, ActionTraceEvent.Begin, nameof(DummyActionOne)));
            Assert.True(this.MatchTraceEntry(container, 7, ActionTraceEvent.End, nameof(DummyActionOne)));
            Assert.True(this.MatchTraceEntry(container, 8, ActionTraceEvent.End, "ActionBundle (1)"));
            Assert.True(this.MatchTraceEntry(container, 9, ActionTraceEvent.End, "ActionSequence (2)"));
        }

        [Fact]
        public async Task ActionSequencePartialExecutionTest()
        {
            var collection = new ActionCollection<IActionTracingContainer>();
            collection.AddAction<DummyActionTwo>();
            collection.AddAction<DummyActionFour>();

            var sequence = new ActionSequence<IActionTracingContainer>(collection);

            var container = this.GetMockedTracingContainer();
            var result = await sequence.ExecuteAsync(container);

            Assert.Equal(ActionSequenceExecutionResult.Partial, result);
            Assert.True(this.MatchTraceEntry(container, 0, ActionTraceEvent.Begin, "ActionSequence (2)"));
            Assert.True(this.MatchTraceEntry(container, 1, ActionTraceEvent.Begin, "ActionBundle (1)"));
            Assert.True(this.MatchTraceEntry(container, 2, ActionTraceEvent.Begin, nameof(DummyActionTwo)));
            Assert.True(this.MatchTraceEntry(container, 3, ActionTraceEvent.End, nameof(DummyActionTwo)));
            Assert.True(this.MatchTraceEntry(container, 4, ActionTraceEvent.End, "ActionBundle (1)"));
            Assert.True(this.MatchTraceEntry(container, 5, ActionTraceEvent.End, "ActionSequence (2)"));
        }

        [Fact]
        public async Task ActionSequenceNoneExecutionTest()
        {
            var collection = new ActionCollection<IActionTracingContainer>();
            collection.AddAction<DummyActionThree>();
            collection.AddAction<DummyActionOne>();

            var sequence = new ActionSequence<IActionTracingContainer>(collection);

            var container = this.GetMockedTracingContainer();
            var result = await sequence.ExecuteAsync(container);

            Assert.Equal(ActionSequenceExecutionResult.None, result);
            Assert.True(this.MatchTraceEntry(container, 0, ActionTraceEvent.Begin, "ActionSequence (2)"));
            Assert.True(this.MatchTraceEntry(container, 1, ActionTraceEvent.End, "ActionSequence (2)"));
        }

        [Fact]
        public async Task ThrowsForActionExceptionTest()
        {
            var collection = new ActionCollection<IActionTracingContainer>();
            collection.AddAction<ActionWhichThrowsActionException>();

            var sequence = new ActionSequence<IActionTracingContainer>(collection);

            var container = this.GetMockedTracingContainer();
            var exception = await Assert.ThrowsAsync<ActionException>(
                async () => await sequence.ExecuteAsync(container));

            Assert.Equal("TEST", exception.Message);
            
            Assert.True(this.MatchTraceEntry(container, 0, ActionTraceEvent.Begin, "ActionSequence (1)"));
            Assert.True(this.MatchTraceEntry(container, 1, ActionTraceEvent.Begin, "ActionBundle (1)"));
            Assert.True(this.MatchTraceEntry(container, 2, ActionTraceEvent.Begin, nameof(ActionWhichThrowsActionException)));
            Assert.True(this.MatchTraceEntry(container, 3, ActionTraceEvent.UnexpectedEnd, nameof(ActionWhichThrowsActionException)));
            Assert.True(this.MatchTraceEntry(container, 4, ActionTraceEvent.UnexpectedEnd, "ActionBundle (1)"));
            Assert.True(this.MatchTraceEntry(container, 5, ActionTraceEvent.UnexpectedEnd, "ActionSequence (1)"));
        }

        [Fact]
        public async Task ThrowsForExceptionWhileDiposeTest()
        {
            var collection = new ActionCollection<IActionTracingContainer>();
            collection.AddAction<ActionWhichThrowsWhileDispose>();

            var sequence = new ActionSequence<IActionTracingContainer>(collection);

            var container = this.GetMockedTracingContainer();
            var exception = await Assert.ThrowsAsync<ActionException>(
                async () => await sequence.ExecuteAsync(container));

            Assert.Equal(typeof(ActionWhichThrowsWhileDispose), exception.ActionInfo.ActionType);
            Assert.IsType<ApplicationException>(exception.InnerException);
            Assert.Equal("TEST", exception.InnerException.Message);
            
            Assert.True(this.MatchTraceEntry(container, 0, ActionTraceEvent.Begin, "ActionSequence (1)"));
            Assert.True(this.MatchTraceEntry(container, 1, ActionTraceEvent.Begin, "ActionBundle (1)"));
            Assert.True(this.MatchTraceEntry(container, 2, ActionTraceEvent.Begin, nameof(ActionWhichThrowsWhileDispose)));
            Assert.True(this.MatchTraceEntry(container, 3, ActionTraceEvent.UnexpectedEnd, nameof(ActionWhichThrowsWhileDispose)));
            Assert.True(this.MatchTraceEntry(container, 4, ActionTraceEvent.UnexpectedEnd, "ActionBundle (1)"));
            Assert.True(this.MatchTraceEntry(container, 5, ActionTraceEvent.UnexpectedEnd, "ActionSequence (1)"));
        }

        #region Test classes
        private class ActionWhichThrowsActionException : IAction<IActionTracingContainer>
        {
            public Task ExecuteAsync(IActionTracingContainer context, CancellationToken cancellationToken) => throw new ActionException("TEST");
        }
        
        private class ActionWhichThrowsWhileDispose : IAction<IActionTracingContainer>, IDisposable
        {
            public Task ExecuteAsync(IActionTracingContainer context, CancellationToken cancellationToken) => Task.CompletedTask;

            public void Dispose() => throw new ApplicationException("TEST");
        }
        #endregion
    }
}