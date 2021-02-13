using System;

namespace ActionWorkflow
{
    public class DefaultActionServiceProvider : IServiceProvider
    {
        private readonly IActionContext _actionContext;
        private readonly IExportProvider _exportProvider;
        private readonly IServiceProvider _serviceProvider;

        public DefaultActionServiceProvider(IActionContext actionContext, IExportProvider exportProvider, IServiceProvider serviceProvider)
        {
            _actionContext = actionContext ?? throw new ArgumentNullException(nameof(actionContext));
            _exportProvider = exportProvider ?? throw new ArgumentNullException(nameof(exportProvider));
            _serviceProvider = serviceProvider;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IActionContext))
            {
                return _actionContext;
            }

            var export = _exportProvider.GetExport(serviceType);
            if (export != null)
            {
                return export;
            }

            return _serviceProvider?.GetService(serviceType);
        }
    }
}