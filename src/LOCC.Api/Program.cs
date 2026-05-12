using Microsoft.EntityFrameworkCore;
using LOCC.Infrastructure;
using LOCC.Infrastructure.Seed;
using LOCC.Application.Services;
using LOCC.Application.DTOs;
using LOCC.Domain;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<LoccDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Application services
builder.Services.AddScoped<RuleEngine>();

// CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowFrontend");

// Seed database only on startup.
// Rule evaluation is triggered manually through POST /api/rules/evaluate.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<LoccDbContext>();

    db.Database.EnsureCreated();
    SeedData.EnsureSeedData(db);

    Console.WriteLine("Database initialised and seed data checked.");
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/api/rules/evaluate", (RuleEngine ruleEngine) =>
{
    var result = ruleEngine.EvaluateAll();

    return Results.Ok(new
    {
        alerts = result.Alerts,
        tasks = result.Tasks,
        recovery = result.Recovery
    });
});

app.MapGet("/api/outbreaks", (LoccDbContext db) =>
{
    return Results.Ok(db.OutbreakEvents.ToList());
});

app.MapGet("/api/tasks", (LoccDbContext db) =>
{
    var tasks = db.TaskActions
        .ToList()
        .Select(task => new TaskDto
        {
            TaskId = task.TaskId,
            TaskDescription = task.TaskDescription,
            Priority = task.Priority.ToString(),
            Status = TaskStatusLabelService.GetDisplayLabel(task.Status),
            OperationalArea = AIIMSLabelService.GetOperationalLabel(task.AIIMSFunction),
            DueDateTime = task.DueDateTime
        });

    return Results.Ok(tasks);
});

app.MapPatch("/api/tasks/{taskId:guid}/status", (Guid taskId, UpdateTaskStatusRequest request, LoccDbContext db) =>
{
    var task = db.TaskActions.FirstOrDefault(t => t.TaskId == taskId);

    if (task == null)
    {
        return Results.NotFound(new { message = "Task not found" });
    }

    if (Enum.TryParse<LOCC.Domain.TaskStatus>(
        request.Status.Replace(" ", ""),
        true,
        out var newStatus))
    {
        return Results.BadRequest(new { message = $"Invalid task status: {request.Status}" });
    }

    task.Status = newStatus;

    db.SaveChanges();

    var updatedTask = new TaskDto
    {
        TaskId = task.TaskId,
        TaskDescription = task.TaskDescription,
        Priority = task.Priority.ToString(),
        Status = TaskStatusLabelService.GetDisplayLabel(task.Status),
        OperationalArea = AIIMSLabelService.GetOperationalLabel(task.AIIMSFunction),
        DueDateTime = task.DueDateTime
    };

    return Results.Ok(updatedTask);
});

app.MapGet("/api/outbreak-summary", (LoccDbContext db) =>
{
    var outbreak = db.OutbreakEvents.FirstOrDefault();

    if (outbreak == null)
    {
        return Results.NotFound();
    }

    var activeCases = db.Cases.Count(c =>
        c.CaseStatus == CaseStatus.Suspected ||
        c.CaseStatus == CaseStatus.Confirmed);

    var recovery = db.RecoveryBAUs
        .FirstOrDefault(r => r.OutbreakId == outbreak.OutbreakId);

    var lowPPE = db.Resources
        .Where(r => r.ResourceType == ResourceType.PPE && r.DaysRemaining <= 3)
        .Count();

    return Results.Ok(new
    {
        pathogen = outbreak.Pathogen,
        outbreakPhase = outbreak.OutbreakPhase.ToString(),
        riskLevel = outbreak.CurrentRiskLevel,
        activeCases,
        bauScore = recovery?.BAUReadinessScore ?? 0,
        ppeWarnings = lowPPE
    });
});

app.MapGet("/api/resources", (LoccDbContext db) =>
{
    return Results.Ok(db.Resources.ToList());
});

app.MapGet("/api/recovery", (LoccDbContext db) =>
{
    return Results.Ok(db.RecoveryBAUs.ToList());
});

app.Run();

public class UpdateTaskStatusRequest
{
    public string Status { get; set; } = "";
}