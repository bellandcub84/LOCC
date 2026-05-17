using Microsoft.EntityFrameworkCore;
using LOCC.Infrastructure;
using LOCC.Infrastructure.Seed;
using LOCC.Application.Services;
using LOCC.Application.DTOs;
using LOCC.Domain;
using LOCC.Application.Calculators.PPE;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<LoccDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Application services
builder.Services.AddScoped<RuleEngine>();
builder.Services.AddScoped<PPEConsumptionCalculator>();

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
        alertsGenerated = result.Alerts.Count,
        tasksGenerated = result.Tasks.Count,
        recoveryAvailable = result.Recovery != null,
        bauScore = result.Recovery?.BAUReadinessScore
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
                DueDateTime = task.DueDateTime,
                GeneratedFrom = task.GeneratedFrom,
                DecisionRationale = task.DecisionRationale
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

    var cleanedStatus = request.Status
        .Replace(" ", "")
        .Replace("🟢", "")
        .Replace("🔵", "")
        .Replace("🔴", "")
        .Replace("🟠", "")
        .Replace("🟡", "")
        .Replace("⚪", "")
        .Trim();

    if (!Enum.TryParse<LOCC.Domain.TaskStatus>(
        cleanedStatus,
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
        DueDateTime = task.DueDateTime,
        GeneratedFrom = task.GeneratedFrom,
        DecisionRationale = task.DecisionRationale
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
    var resources = db.Resources
        .ToList()
        .Select(r => new ResourceDto
        {
            ResourceId = r.ResourceId,
            ItemName = r.ItemName,
            ResourceType = r.ResourceType.ToString(),
            DaysRemaining = r.DaysRemaining,
            ReorderThreshold = r.ReorderThreshold,
            CurrentStockLevel = r.CurrentStockLevel,
            DailyUsageRate = r.DailyUsageRate,
            MinimumSafeStockLevel = r.MinimumSafeStockLevel,
            ProjectedDaysRemaining = r.ProjectedDaysRemaining
        });

    return Results.Ok(resources);
});

app.MapGet("/api/recovery", (LoccDbContext db) =>
{
    return Results.Ok(db.RecoveryBAUs.ToList());
});

app.MapGet("/api/rooms", (LoccDbContext db) =>
{
    var rooms = db.FacilityRooms
        .ToList()
        .Select(room => new
        {
            room.FacilityRoomId,
            room.RoomName,
            room.Zone,
            riskLevel = room.RiskLevel.ToString(),
            room.IsIsolationRoom,
            room.HasConfirmedCase,
            room.HasSuspectedCase,
            room.IsClosed,
            room.Notes
        });

    return Results.Ok(rooms);
});

app.MapPost("/api/ppe/calculate",
    (PPECalculatorRequestDto request,
     PPEConsumptionCalculator calculator) =>
{
    var result = calculator.Calculate(request);
    return Results.Ok(result);
});

app.Run();

public class UpdateTaskStatusRequest
{
    public string Status { get; set; } = "";
}