namespace WorkflowEngine.Models;

public class WorkflowDefinition
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty; 
    public List<State> States { get; set; } = [];
    public List<Action> Actions { get; set; } = [];

    // Method to validate the definition upon creation.
    public void Validate()
    {
        if (States.Count(s => s.IsInitial) != 1)
        {
            throw new InvalidOperationException("Workflow definition must have exactly one initial state.");
        }
        if (States.Select(s => s.Id).Distinct().Count() != States.Count)
        {
            throw new InvalidOperationException("State IDs must be unique within a definition.");
        }
        if (Actions.Select(a => a.Id).Distinct().Count() != Actions.Count)
        {
            throw new InvalidOperationException("Action IDs must be unique within a definition.");
        }
    }

    public State GetInitialState() => States.Single(s => s.IsInitial);
}