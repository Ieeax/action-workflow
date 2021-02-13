namespace ActionWorkflow.Tracing
{
    public interface IActionTracingContainer
    {
        ActionTrace ActionTrace { get; }
    }
}