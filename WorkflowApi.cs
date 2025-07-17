using Microsoft.AspNetCore.Mvc;
using WorkflowEngine.Services;
using WorkflowEngine.DTOs; 

namespace WorkflowEngine.Apis;

public static class WorkflowApi
{
    public static void MapWorkflowApi(this WebApplication app)
    {
        var workflowGroup = app.MapGroup("/workflows");

        // Workflow Definition Endpoints 
        // No try-catch block needed
        workflowGroup.MapPost("/definitions", (CreateDefinitionRequest req, WorkflowService service) =>
        {
            var definition = service.CreateDefinition(req.Name, req.States, req.Actions);
            return Results.Created($"/workflows/definitions/{definition.Id}", definition);
        });

        workflowGroup.MapGet("/definitions", (WorkflowService service) =>
        {
            return Results.Ok(service.GetAllDefinitions());
        });

        workflowGroup.MapGet("/definitions/{id}", (string id, WorkflowService service) =>
        {
            var definition = service.GetDefinition(id);
            return definition is not null ? Results.Ok(definition) : Results.NotFound();
        });

        // Workflow Instance Endpoints
        // No try-catch block needed
        workflowGroup.MapPost("/instances", ([FromBody] StartInstanceRequest req, WorkflowService service) =>
        {
            var instance = service.StartInstance(req.DefinitionId);
            return Results.Created($"/workflows/instances/{instance.Id}", instance);
        });

        workflowGroup.MapGet("/instances", (WorkflowService service) =>
        {
            return Results.Ok(service.GetAllInstances());
        });

        workflowGroup.MapGet("/instances/{id}", (string id, WorkflowService service) =>
        {
            var instance = service.GetInstance(id);
            return instance is not null ? Results.Ok(instance) : Results.NotFound();
        });

        // Action Execution Endpoint
        // No try-catch block needed
        workflowGroup.MapPost("/instances/{instanceId}/actions", ([FromRoute] string instanceId, [FromBody] ExecuteActionRequest req, WorkflowService service) =>
        {
            var instance = service.ExecuteAction(instanceId, req.ActionId);
            return Results.Ok(instance);
        });
    }
}