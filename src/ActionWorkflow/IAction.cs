using System.Threading;
using System.Threading.Tasks;

namespace ActionWorkflow
{
    public interface IAction<T>
    {
        /// <summary>
        /// Executes the action for the given <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The item for which the action is executed.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> which indicates whether the action should be canceled.</param>
        Task ExecuteAsync(T context, CancellationToken cancellationToken);
    }
}