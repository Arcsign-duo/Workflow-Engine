using WorkflowEngine.Models;
using Action = WorkflowEngine.Models.Action;

namespace WorkflowEngine.DTOs;

public record CreateDefinitionRequest(string Name, List<State> States, List<Action> Actions);
public record StartInstanceRequest(string DefinitionId);
public record ExecuteActionRequest(string ActionId);