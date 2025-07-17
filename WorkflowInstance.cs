namespace WorkflowEngine.Models;

public class WorkflowInstance
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string DefinitionId { get; set; }
    public string CurrentStateId { get; set; }
    public List<WorkflowHistory> History { get; set; } = [];

    public WorkflowInstance(string definitionId, string initialStateId)
    {
        DefinitionId = definitionId;
        CurrentStateId = initialStateId;
        History.Add(new WorkflowHistory("start", initialStateId, DateTime.UtcNow));
    }
}

public record WorkflowHistory(string Action, string State, DateTime Timestamp);