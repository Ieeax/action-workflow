using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Xunit;

namespace ActionWorkflow.Tests
{
    public class ActionActivatorTests
    {
        private const string TestActionName = "SomeNamedAction";
        private const string TestActionDescription = "This is some description";
        private const string TestImportName = "SomeNamedImport";
        private const string TestImportName2 = "SomeNamedImport2";

        [Fact]
        public void GetActionInfoWithoutActionAttributeTest()
        {
            var actionInfo = ActionActivator.GetActionInfo(typeof(ActionWithNoName));
        
            Assert.NotNull(actionInfo);
            Assert.NotNull(actionInfo.Imports);
            Assert.NotNull(actionInfo.Constructor);
            Assert.Equal(typeof(ActionWithNoName), actionInfo.ActionType);
            Assert.Equal(nameof(ActionWithNoName), actionInfo.Name);
            Assert.Null(actionInfo.FriendlyName);
            Assert.Null(actionInfo.Description);
        }

        [Fact]
        public void GetActionInfoWithActionAttributeTest()
        {
            var actionInfo = ActionActivator.GetActionInfo(typeof(ActionWithName));
        
            Assert.NotNull(actionInfo);
            Assert.NotNull(actionInfo.Imports);
            Assert.NotNull(actionInfo.Constructor);
            Assert.Equal(typeof(ActionWithName), actionInfo.ActionType);
            Assert.Equal(TestActionName, actionInfo.Name);
            Assert.Equal(TestActionName, actionInfo.FriendlyName);
            Assert.Equal(TestActionDescription, actionInfo.Description);
        }

        [Fact]
        public void GetActionInfoWithImportsTest()
        {
            var actionInfo = ActionActivator.GetActionInfo(typeof(ActionWithImports));
        
            Assert.NotNull(actionInfo);
            Assert.NotNull(actionInfo.Imports);
            Assert.Equal(3, actionInfo.Imports.Count);

            AssertImport(typeof(int), null, actionInfo.Imports.ElementAt(0));
            AssertImport(typeof(double), TestImportName, actionInfo.Imports.ElementAt(1));
            AssertImport(typeof(decimal), null, actionInfo.Imports.ElementAt(2));
        }

        [Fact]
        public void CreateActionInstanceTest()
        {
            var actionInfo = ActionActivator.GetActionInfo(typeof(ActionWithImports));

            var serviceProvider = A.Fake<IServiceProvider>();
            A.CallTo(() => serviceProvider.GetService(typeof(string))).Returns("TEST");
            A.CallTo(() => serviceProvider.GetService(typeof(double))).Returns((double)1);

            var exportProvider = A.Fake<IExportProvider>();
            A.CallTo(() => exportProvider.GetExport(typeof(int), null)).Returns(2);
            A.CallTo(() => exportProvider.GetExport(typeof(double), TestImportName)).Returns((double)3);
            A.CallTo(() => exportProvider.GetExport(typeof(decimal), null)).Returns((decimal)4);

            var instance = ActionActivator.CreateInstance(actionInfo, serviceProvider, exportProvider);
        
            Assert.NotNull(instance);
            Assert.IsType<ActionWithImports>(instance);

            var instance2 = (ActionWithImports)instance;

            Assert.Equal("TEST", instance2.Service1);
            Assert.Equal(1, instance2.Service2);
            Assert.Equal(2, instance2.Import1);
            Assert.Equal(3, instance2.Import2);
            Assert.Equal(4, instance2.Import3);
        }

        [Fact]
        public void CreateActionInstanceWithImportsOfSameTypeTest()
        {
            var actionInfo = ActionActivator.GetActionInfo(typeof(ActionWithImportsOfSameType));
            var serviceProvider = A.Fake<IServiceProvider>();

            var exportProvider = A.Fake<IExportProvider>();
            A.CallTo(() => exportProvider.GetExport(typeof(int), null)).Returns(1);
            A.CallTo(() => exportProvider.GetExport(typeof(int), TestImportName)).Returns(2);
            A.CallTo(() => exportProvider.GetExport(typeof(int), TestImportName2)).Returns(3);

            var instance = ActionActivator.CreateInstance(actionInfo, serviceProvider, exportProvider);
        
            Assert.NotNull(instance);
            Assert.IsType<ActionWithImportsOfSameType>(instance);

            var instance2 = (ActionWithImportsOfSameType)instance;

            Assert.Equal(1, instance2.Import1);
            Assert.Equal(2, instance2.Import2);
            Assert.Equal(3, instance2.Import3);
        }
        
        [Fact]
        public void CreateActionInstanceWithMissingImportTest()
        {
            var actionInfo = ActionActivator.GetActionInfo(typeof(ActionWithImportsOfSameType));
            var serviceProvider = A.Fake<IServiceProvider>();

            var exportProvider = A.Fake<IExportProvider>();
            A.CallTo(() => exportProvider.GetExport(typeof(int), null)).Returns(null);

            Assert.Throws<InvalidOperationException>(
                () => ActionActivator.CreateInstance(actionInfo, serviceProvider, exportProvider));
        }
        
        [Fact]
        public void CreateActionInstanceWithOptionalServiceTest()
        {
            var actionInfo = ActionActivator.GetActionInfo(typeof(ActionWithOptionalService));
            var exportProvider = A.Fake<IExportProvider>();

            var serviceProvider = A.Fake<IServiceProvider>();
            A.CallTo(() => serviceProvider.GetService(typeof(string))).Returns("TEST");
            A.CallTo(() => serviceProvider.GetService(typeof(int))).Returns(null);

            var instance = ActionActivator.CreateInstance(actionInfo, serviceProvider, exportProvider);
        
            Assert.NotNull(instance);
            Assert.IsType<ActionWithOptionalService>(instance);

            var instance2 = (ActionWithOptionalService)instance;

            Assert.Equal("TEST", instance2.Service1);
            Assert.Equal(-1, instance2.Service2);
        }

        [Fact]
        public void CreateActionInstanceWithMissingServiceTest()
        {
            var actionInfo = ActionActivator.GetActionInfo(typeof(ActionWithOptionalService));
            var exportProvider = A.Fake<IExportProvider>();

            var serviceProvider = A.Fake<IServiceProvider>();
            A.CallTo(() => serviceProvider.GetService(typeof(string))).Returns(null);

            Assert.Throws<InvalidOperationException>(
                () => ActionActivator.CreateInstance(actionInfo, serviceProvider, exportProvider));
        }

        private void AssertImport(Type expectedType, string expectedName, ActionImport actualImport)
        {
            Assert.NotNull(actualImport);
            Assert.Equal(expectedType, actualImport.Type);
            Assert.Equal(expectedName, actualImport.Name);
        }

        #region Test classes
        private class ActionWithNoName : IAction<string>
        {
            public Task ExecuteAsync(string context, CancellationToken cancellationToken) => throw new NotImplementedException();
        }

        [Action(TestActionName, TestActionDescription)]
        private class ActionWithName : IAction<string>
        {
            public Task ExecuteAsync(string context, CancellationToken cancellationToken) => throw new NotImplementedException();
        }

        private class ActionWithImports : IAction<string>
        {
            public ActionWithImports(
                [FromImport] int import1,
                string service1,
                [FromImport(TestImportName)] double import2, 
                [FromImport] decimal import3,
                double service2)
            {
                Import1 = import1;
                Import2 = import2;
                Import3 = import3;
                Service1 = service1;
                Service2 = service2;
            }

            public Task ExecuteAsync(string context, CancellationToken cancellationToken) => throw new NotImplementedException();

            public int Import1 { get; }
            
            public double Import2 { get; }

            public decimal Import3 { get; }

            public string Service1 { get; }

            public double Service2 { get; }
        }

        private class ActionWithImportsOfSameType : IAction<string>
        {
            public ActionWithImportsOfSameType(
                [FromImport] int import1,
                [FromImport(TestImportName)] int import2, 
                [FromImport(TestImportName2)] int import3)
            {
                Import1 = import1;
                Import2 = import2;
                Import3 = import3;
            }

            public Task ExecuteAsync(string context, CancellationToken cancellationToken) => throw new NotImplementedException();

            public int Import1 { get; }

            public int Import2 { get; }

            public int Import3 { get; }
        }

        private class ActionWithOptionalService : IAction<string>
        {
            public ActionWithOptionalService(
                string service1,
                int service2 = -1)
            {
                Service1 = service1;
                Service2 = service2;
            }

            public Task ExecuteAsync(string context, CancellationToken cancellationToken) => throw new NotImplementedException();

            public string Service1 { get; }

            public int Service2 { get; }
        }
        #endregion
    }
}
