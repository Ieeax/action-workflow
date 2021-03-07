using ActionWorkflow.Tracing;
using System;
using System.Linq;
using Xunit;

namespace ActionWorkflow.Tests
{
    public class ActionSequenceFactoryBuilderTests
    {
        [Fact]
        public void AddActionsTest()
        {
            var collection = new ActionCollection<IActionTracingContainer>();
            collection.AddAction<DummyActionOne>();
            collection.AddAction(typeof(DummyActionTwo));

            Assert.True(collection.Count() == 2);
            Assert.Equal(typeof(DummyActionOne), collection.ElementAt(0).ActionType);
            Assert.Equal(typeof(DummyActionTwo), collection.ElementAt(1).ActionType);
        }

        [Fact]
        public void AddActionsWithPriorityTest()
        {
            var collection = new ActionCollection<IActionTracingContainer>();
            collection.AddAction<DummyActionOne>(4);
            collection.AddAction(typeof(DummyActionTwo), 1);
            collection.AddAction(typeof(DummyActionThree), 3);
            collection.AddAction(typeof(DummyActionFour), 3);

            Assert.True(collection.Count() == 4);
            Assert.Equal(typeof(DummyActionTwo), collection.ElementAt(0).ActionType);
            Assert.Equal(typeof(DummyActionThree), collection.ElementAt(1).ActionType);
            Assert.Equal(typeof(DummyActionFour), collection.ElementAt(2).ActionType);
            Assert.Equal(typeof(DummyActionOne), collection.ElementAt(3).ActionType);
        }

        [Fact]
        public void AddActionWithInvalidTypeTest()
        {
            var collection = new ActionCollection<string>();

            Assert.Throws<InvalidOperationException>(() => collection.AddAction(typeof(string)));
            Assert.Throws<InvalidOperationException>(() => collection.AddAction(typeof(IAction<string>)));
        }
    }
}