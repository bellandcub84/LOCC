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
builder.Services.AddScoped<JurisdictionExportService>();

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

app.MapGet("/api/zones", (LoccDbContext db) =>
{
    var zones = db.FacilityRooms
        .ToList()
        .Select(room => new
        {
            roomId = room.FacilityRoomId,
            roomName = room.RoomName,
            riskZoneStatus = room.RiskZoneStatus.ToString(),
            enhancedPrecautionsRequired = room.EnhancedPrecautionsRequired,
            terminalCleanRequired = room.TerminalCleanRequired,
            lastExposureDate = room.LastExposureDate,
            zoningNotes = room.ZoningNotes
        });

    return Results.Ok(zones);
});

app.MapPatch("/api/zones/{roomId:int}", (int roomId, UpdateZoneRoomRequest request, LoccDbContext db) =>
{
    var room = db.FacilityRooms.FirstOrDefault(r => r.FacilityRoomId == roomId);

    if (room == null)
    {
        return Results.NotFound(new { message = "Room not found" });
    }

    if (!string.IsNullOrWhiteSpace(request.RiskLevel))
    {
        if (!Enum.TryParse<LOCC.Domain.RoomRiskLevel>(request.RiskLevel, true, out var riskLevel))
        {
            return Results.BadRequest(new { message = $"Invalid risk level: {request.RiskLevel}" });
        }

        room.RiskLevel = riskLevel;
    }

    if (request.IsClosed.HasValue)
    {
        room.IsClosed = request.IsClosed.Value;
    }

    if (request.IsIsolationRoom.HasValue)
    {
        room.IsIsolationRoom = request.IsIsolationRoom.Value;
    }

    if (request.TerminalCleanCompleted == true)
    {
        room.HasConfirmedCase = false;
        room.HasSuspectedCase = false;
        room.IsClosed = false;
        room.RiskLevel = LOCC.Domain.RoomRiskLevel.Low;
        room.Notes = "Terminal clean completed. Room returned to low risk.";
    }

    if (!string.IsNullOrWhiteSpace(request.Notes))
    {
        room.Notes = request.Notes;
    }

    db.SaveChanges();

    return Results.Ok(new
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

app.MapGet("/api/surveillance", (LoccDbContext db) =>
{
    var cases = db.SurveillanceCases
        .ToList()
        .Select(c => new
        {
            c.SurveillanceCaseId,
            c.DisplayName,
            c.PersonType,
            c.RoomName,
            c.Zone,
            c.CaseStatus,
            c.Pathogen,
            c.SymptomOnsetDate,
            c.TestType,
            c.TestDate,
            c.TestResult,
            c.IsolationStartDate,
            c.HospitalTransferred,
            c.Deceased,
            c.PublicHealthNotificationStatus,
            c.Jurisdiction
        });

    return Results.Ok(cases);
});

app.MapGet("/api/exports/{jurisdiction}/surveillance", (
    string jurisdiction,
    LoccDbContext db,
    JurisdictionExportService exportService) =>
{
    var cases = db.SurveillanceCases
        .Where(c => c.Jurisdiction == jurisdiction.ToUpper())
        .ToList();

    if (!cases.Any())
    {
        cases = db.SurveillanceCases.ToList();
    }

    var csv = exportService.ExportSurveillanceCasesToCsv(cases, jurisdiction);

    var fileName =
        $"LOCC_{jurisdiction.ToUpper()}_Surveillance_Line_List_{DateTime.UtcNow:yyyyMMdd}.csv";

    return Results.File(
        System.Text.Encoding.UTF8.GetBytes(csv),
        "text/csv",
        fileName
    );
});

app.MapGet("/api/epidemiology/summary", (LoccDbContext db) =>
{
    var cases = db.SurveillanceCases.ToList();

    var totalCases = cases.Count;
    var confirmedCases = cases.Count(c => c.CaseStatus == "Confirmed");
    var suspectedCases = cases.Count(c => c.CaseStatus == "Suspected");

    var residentCases = cases.Count(c => c.PersonType == "Resident");
    var staffCases = cases.Count(c => c.PersonType == "Staff");

    var hospitalisations = cases.Count(c => c.HospitalTransferred);
    var deaths = cases.Count(c => c.Deceased);

    var earliestOnset = cases
        .Where(c => c.SymptomOnsetDate.HasValue)
        .OrderBy(c => c.SymptomOnsetDate)
        .Select(c => c.SymptomOnsetDate)
        .FirstOrDefault();

    var latestOnset = cases
        .Where(c => c.SymptomOnsetDate.HasValue)
        .OrderByDescending(c => c.SymptomOnsetDate)
        .Select(c => c.SymptomOnsetDate)
        .FirstOrDefault();

    var casesByDate = cases
        .Where(c => c.SymptomOnsetDate.HasValue)
        .GroupBy(c => c.SymptomOnsetDate!.Value.Date)
        .OrderBy(g => g.Key)
        .Select(g => new
        {
            date = g.Key,
            count = g.Count(),
            confirmed = g.Count(c => c.CaseStatus == "Confirmed"),
            suspected = g.Count(c => c.CaseStatus == "Suspected")
        });

    var casesByZone = cases
        .Where(c => !string.IsNullOrWhiteSpace(c.Zone))
        .GroupBy(c => c.Zone)
        .Select(g => new
        {
            zone = g.Key,
            count = g.Count(),
            confirmed = g.Count(c => c.CaseStatus == "Confirmed"),
            suspected = g.Count(c => c.CaseStatus == "Suspected")
        })
        .OrderByDescending(z => z.count);

    return Results.Ok(new
    {
        totalCases,
        confirmedCases,
        suspectedCases,
        residentCases,
        staffCases,
        hospitalisations,
        deaths,
        earliestOnset,
        latestOnset,
        casesByDate,
        casesByZone
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
    (PPECalculatorRequestDto request, PPEConsumptionCalculator calculator) =>
{
    var result = calculator.Calculate(request);
    return Results.Ok(result);
});

app.Run();

public class UpdateTaskStatusRequest
{
    public string Status { get; set; } = "";
}