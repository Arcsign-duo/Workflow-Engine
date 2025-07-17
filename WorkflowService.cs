using WorkflowEngine.Models;
using System.Collections.Concurrent;
using Action = WorkflowEngine.Models.Action;

namespace WorkflowEngine.Services;

public class WorkflowService
{
    private static readonly ConcurrentDictionary<string, WorkflowDefinition> _definitions = new();
    private static readonly ConcurrentDictionary<string, WorkflowInstance> _instances = new();

    public WorkflowDefinition CreateDefinition(string name, List<State> states, List<Action> actions)
    {
        var definition = new WorkflowDefinition { Name = name, States = states, Actions = actions };
        definition.Validate();
        _definitions[definition.Id] = definition;
        return definition;
    }

    public WorkflowDefinition? GetDefinition(string id) =>
        _definitions.GetValueOrDefault(id);

    public IEnumerable<WorkflowDefinition> GetAllDefinitions() => _definitions.Values;

    public WorkflowInstance StartInstance(string definitionId)
    {
        var definition = GetDefinitionOrThrow(definitionId);
        var initialState = definition.GetInitialState();
        var instance = new WorkflowInstance(definitionId, initialState.Id);
        _instances[instance.Id] = instance;
        return instance;
    }

    public WorkflowInstance? GetInstance(string instanceId) =>
        _instances.GetValueOrDefault(instanceId);

    public IEnumerable<WorkflowInstance> GetAllInstances() => _instances.Values;

    public WorkflowInstance ExecuteAction(string instanceId, string actionId)
    {
        var instance = GetInstanceOrThrow(instanceId);
        var definition = GetDefinitionOrThrow(instance.DefinitionId);
        
        ValidateCurrentStateIsActionable(instance, definition);

        var action = GetActionOrThrow(actionId, definition);
        
        ValidateActionCanBeExecuted(action, instance.CurrentStateId);

        // Perform the state transition
        instance.CurrentStateId = action.ToState;
        instance.History.Add(new WorkflowHistory(actionId, action.ToState, DateTime.UtcNow));

        return instance;
    }

    // Private Validation Helpers 

    private WorkflowInstance GetInstanceOrThrow(string instanceId)
    {
        return _instances.GetValueOrDefault(instanceId) 
            ?? throw new KeyNotFoundException($"Workflow instance '{instanceId}' not found.");
    }

    private WorkflowDefinition GetDefinitionOrThrow(string definitionId)
    {
        return _definitions.GetValueOrDefault(definitionId) 
            ?? throw new KeyNotFoundException($"Workflow definition '{definitionId}' not found.");
    }

    private Action GetActionOrThrow(string actionId, WorkflowDefinition definition)
    {
        var action = definition.Actions.Find(a => a.Id == actionId);
        if (action == null)
        {
            throw new KeyNotFoundException($"Action '{actionId}' not found in the workflow definition.");
        }
        // Also check if the target state exists
        if (!definition.States.Exists(s => s.Id == action.ToState))
        {
             throw new InvalidOperationException($"Action '{actionId}' transitions to an unknown state '{action.ToState}'.");
        }
        return action;
    }

    private void ValidateCurrentStateIsActionable(WorkflowInstance instance, WorkflowDefinition definition)
    {
        var currentState = definition.States.Find(s => s.Id == instance.CurrentStateId);
        if (currentState == null || currentState.IsFinal)
        {
            throw new InvalidOperationException("Cannot execute actions on an instance in a final or unknown state.");
        }
    }

    private void ValidateActionCanBeExecuted(Action action, string currentStateId)
    {
        if (!action.Enabled)
        {
            throw new InvalidOperationException($"Action '{action.Id}' is disabled.");
        }
        if (!action.FromStates.Contains(currentStateId))
        {
            throw new InvalidOperationException($"Action '{action.Id}' cannot be executed from the current state '{currentStateId}'.");
        }
    }
}