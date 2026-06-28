using LOCC.Application.DTOs;
using LOCC.Infrastructure;

namespace LOCC.Application.Services;

public class OperationalHealthCalculator
{
    private readonly LoccDbContext _db;

    public OperationalHealthCalculator(LoccDbContext db)
    {
        _db = db;
    }

    public OperationalHealthDto Calculate()
    {
        var openTasks = _db.TaskActions
            .Count(t =>
                t.Status != LOCC.Domain.TaskStatus.Completed &&
                t.Status != LOCC.Domain.TaskStatus.Cancelled);

        var blockedTasks = _db.TaskActions
            .Count(t => t.Status == LOCC.Domain.TaskStatus.Blocked);

        var criticalTasks = _db.TaskActions
            .Count(t =>
                t.Priority == LOCC.Domain.Priority.Critical &&
                t.Status != LOCC.Domain.TaskStatus.Completed &&
                t.Status != LOCC.Domain.TaskStatus.Cancelled);

        var activeCases = _db.Cases
            .Count(c =>
                c.CaseStatus == LOCC.Domain.CaseStatus.Suspected ||
                c.CaseStatus == LOCC.Domain.CaseStatus.Confirmed);

        var lowPpeItems = _db.Resources
            .Count(r =>
                r.ResourceType == LOCC.Domain.ResourceType.PPE &&
                r.DaysRemaining <= 3);

        var dimensions = new List<OperationalHealthDimensionDto>
        {
            BuildResponseHealth(openTasks, blockedTasks, criticalTasks),
            BuildClinicalHealth(activeCases),
            BuildResourceHealth(lowPpeItems),
            BuildWorkforceHealth(),
            BuildGovernanceHealth()
        };

        var overallStatus = GetOverallStatus(dimensions);

        return new OperationalHealthDto
        {
            OverallStatus = overallStatus,
            Trend = "Stable",
            Confidence = "Moderate",
            Dimensions = dimensions,
            OperationalInterpretation = BuildInterpretation(overallStatus),
            SuggestedFocus = BuildSuggestedFocus(dimensions)
        };
    }

    private static OperationalHealthDimensionDto BuildResponseHealth(
        int openTasks,
        int blockedTasks,
        int criticalTasks)
    {
        if (blockedTasks > 0 || criticalTasks >= 3)
        {
            return new OperationalHealthDimensionDto
            {
                Name = "Response",
                Status = "Under pressure",
                Reason = "Important work is blocked or several critical tasks are still open."
            };
        }

        if (openTasks >= 5)
        {
            return new OperationalHealthDimensionDto
            {
                Name = "Response",
                Status = "Monitor",
                Reason = "There is a moderate amount of open work."
            };
        }

        return new OperationalHealthDimensionDto
        {
            Name = "Response",
            Status = "Stable",
            Reason = "Current task workload appears manageable."
        };
    }

    private static OperationalHealthDimensionDto BuildClinicalHealth(int activeCases)
    {
        if (activeCases >= 5)
        {
            return new OperationalHealthDimensionDto
            {
                Name = "Clinical & IPC",
                Status = "Under pressure",
                Reason = "There are several active suspected or confirmed cases."
            };
        }

        if (activeCases >= 1)
        {
            return new OperationalHealthDimensionDto
            {
                Name = "Clinical & IPC",
                Status = "Monitor",
                Reason = "There are active suspected or confirmed cases."
            };
        }

        return new OperationalHealthDimensionDto
        {
            Name = "Clinical & IPC",
            Status = "Stable",
            Reason = "No active suspected or confirmed cases are currently recorded."
        };
    }

    private static OperationalHealthDimensionDto BuildResourceHealth(int lowPpeItems)
    {
        if (lowPpeItems >= 2)
        {
            return new OperationalHealthDimensionDto
            {
                Name = "Resources",
                Status = "Under pressure",
                Reason = "Several PPE items may run low soon."
            };
        }

        if (lowPpeItems == 1)
        {
            return new OperationalHealthDimensionDto
            {
                Name = "Resources",
                Status = "Monitor",
                Reason = "One PPE item may need attention soon."
            };
        }

        return new OperationalHealthDimensionDto
        {
            Name = "Resources",
            Status = "Stable",
            Reason = "PPE supplies appear stable."
        };
    }

    private static OperationalHealthDimensionDto BuildWorkforceHealth()
    {
        return new OperationalHealthDimensionDto
        {
            Name = "Workforce",
            Status = "Not yet available",
            Reason = "Workforce data has not been added yet."
        };
    }

    private static OperationalHealthDimensionDto BuildGovernanceHealth()
    {
        return new OperationalHealthDimensionDto
        {
            Name = "Governance",
            Status = "Not yet available",
            Reason = "Governance review data has not been added yet."
        };
    }

    private static string GetOverallStatus(List<OperationalHealthDimensionDto> dimensions)
    {
        if (dimensions.Any(d => d.Status == "Under pressure"))
        {
            return "Under pressure";
        }

        if (dimensions.Any(d => d.Status == "Monitor"))
        {
            return "Monitor";
        }

        return "Stable";
    }

    private static string BuildInterpretation(string overallStatus)
    {
        if (overallStatus == "Under pressure")
        {
            return "We are coping, but some areas need attention today.";
        }

        if (overallStatus == "Monitor")
        {
            return "The response appears manageable, but some areas should be watched.";
        }

        return "The response appears stable at this time.";
    }

    private static List<string> BuildSuggestedFocus(List<OperationalHealthDimensionDto> dimensions)
    {
        var focus = new List<string>();

        if (dimensions.Any(d => d.Name == "Response" && d.Status == "Under pressure"))
        {
            focus.Add("Review blocked or critical tasks.");
        }

        if (dimensions.Any(d => d.Name == "Clinical & IPC" && d.Status != "Stable"))
        {
            focus.Add("Review active cases and IPC controls.");
        }

        if (dimensions.Any(d => d.Name == "Resources" && d.Status != "Stable"))
        {
            focus.Add("Review PPE supplies and expected use.");
        }

        if (focus.Count == 0)
        {
            focus.Add("Continue monitoring the response.");
        }

        return focus;
    }
}
