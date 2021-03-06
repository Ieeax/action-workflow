using System;

namespace ActionWorkflow
{
    public class ActionItem<T>
    {
        public ActionItem(IAction<T> action, ActionInfo actionInfo, IActionContext actionContext, IExportProvider exportProvider, IServiceProvider serviceProvider)
        {
            this.Action = action ?? throw new ArgumentNullException(nameof(action));
            this.ActionInfo = actionInfo ?? throw new ArgumentNullException(nameof(actionInfo));
            this.ActionContext = actionContext ?? throw new ArgumentNullException(nameof(actionContext));
            this.ExportProvider = exportProvider ?? throw new ArgumentNullException(nameof(exportProvider));
            this.ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IAction<T> Action { get; }

        public ActionInfo ActionInfo { get; set; }

        public IActionContext ActionContext { get; }

        public IExportProvider ExportProvider { get; }

        public IServiceProvider ServiceProvider { get; }
    }
}