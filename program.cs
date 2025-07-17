using WorkflowEngine.Apis;
using WorkflowEngine.Services;
using WorkflowEngine.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Added services to the container.
builder.Services.AddSingleton<WorkflowService>(); // Used Singleton for in-memory data store
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseMiddleware<GlobalExceptionHandler>();

// Configured the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Mapped the API endpoints
app.MapWorkflowApi();

app.Run();  