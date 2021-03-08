using System;

namespace ActionWorkflow
{
    internal class ActionItem<T>
    {
        public ActionItem(ActionInfo actionInfo, IActionContext actionContext, IExportProvider exportProvider, IServiceProvider serviceProvider)
        {
            ActionInfo = actionInfo ?? throw new ArgumentNullException(nameof(actionInfo));
            ActionContext = actionContext ?? throw new ArgumentNullException(nameof(actionContext));
            ExportProvider = exportProvider ?? throw new ArgumentNullException(nameof(exportProvider));
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public ActionInfo ActionInfo { get; }

        public IActionContext ActionContext { get; }

        public IExportProvider ExportProvider { get; }

        public IServiceProvider ServiceProvider { get; }
    }
}