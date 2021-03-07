using System;

namespace ActionWorkflow.Services
{
    public class DefaultActionServiceProvider : IServiceProvider
    {
        private readonly IActionContext _actionContext;
        private readonly IServiceProvider _serviceProvider;

        public DefaultActionServiceProvider(IActionContext actionContext, IServiceProvider serviceProvider)
        {
            _actionContext = actionContext ?? throw new ArgumentNullException(nameof(actionContext));
            _serviceProvider = serviceProvider;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IActionContext))
            {
                return _actionContext;
            }

            return _serviceProvider?.GetService(serviceType);
        }
    }
}