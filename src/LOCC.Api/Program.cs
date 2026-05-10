using Microsoft.EntityFrameworkCore;
using LOCC.Infrastructure;
using LOCC.Infrastructure.Seed;
using LOCC.Application.Services;

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

// Seed database and run rule engine once on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<LoccDbContext>();

    db.Database.EnsureCreated();
    SeedData.EnsureSeedData(db);

    var engine = services.GetRequiredService<RuleEngine>();
    var result = engine.EvaluateAll();

    Console.WriteLine("Rule engine executed. Generated alerts:");
    foreach (var a in result.Alerts)
    {
        Console.WriteLine($"- [{a.Type}] {a.Message}");
    }

    Console.WriteLine("Generated tasks:");
    foreach (var t in result.Tasks)
    {
        Console.WriteLine($"- [{t.AIIMSFunction}] {t.TaskDescription} (Priority: {t.Priority})");
    }

    if (result.Recovery != null)
    {
        Console.WriteLine($"Recovery record available. BAU Score: {result.Recovery.BAUReadinessScore:0.##}");
    }
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

app.MapGet("/api/cases", (LoccDbContext db) =>
{
    return Results.Ok(db.Cases.ToList());
});

app.MapGet("/api/tasks", (LoccDbContext db) =>
{
    return Results.Ok(db.TaskActions.ToList());
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