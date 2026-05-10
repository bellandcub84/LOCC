using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using LOCC.Infrastructure;
using LOCC.Infrastructure.Seed;
using LOCC.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<LoccDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Application services
builder.Services.AddScoped<RuleEngine>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<LoccDbContext>();
    db.Database.EnsureCreated();
    SeedData.EnsureSeedData(db);

    // Run rule engine once to demonstrate outputs
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
        Console.WriteLine($"Recovery record created. BAU Score: {result.Recovery.BAUReadinessScore:0.##}");
    }
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
