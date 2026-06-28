using LOCC.Application.DTOs;
using LOCC.Infrastructure;

namespace LOCC.Application.Services;

public class EmergingRiskCalculator
{
    private readonly LoccDbContext _db;

    public EmergingRiskCalculator(LoccDbContext db)
    {
        _db = db;
    }

    public List<EmergingRiskDto> Calculate()
    {
        var risks = new List<EmergingRiskDto>();

        var activeCases = _db.Cases.Count(c =>
            c.CaseStatus == LOCC.Domain.CaseStatus.Suspected ||
            c.CaseStatus == LOCC.Domain.CaseStatus.Confirmed);

        var openCriticalTasks = _db.TaskActions.Count(t =>
            t.Priority == LOCC.Domain.Priority.Critical &&
            t.Status != LOCC.Domain.TaskStatus.Completed &&
            t.Status != LOCC.Domain.TaskStatus.Cancelled);

        var blockedTasks = _db.TaskActions.Count(t =>
            t.Status == LOCC.Domain.TaskStatus.Blocked);

        var lowPpeItems = _db.Resources.Count(r =>
            r.ResourceType == LOCC.Domain.ResourceType.PPE &&
            r.DaysRemaining <= 3);

        if (activeCases >= 3)
        {
            risks.Add(new EmergingRiskDto
            {
                Title = "Clinical demand may increase",
                Category = "Clinical & IPC",
                Likelihood = "Moderate",
                TimeHorizon = "Next 24 hours",
                Confidence = "Moderate",
                Status = "Watching",
                Evidence = "Several suspected or confirmed cases are currently active.",
                OperationalInterpretation = "Clinical and IPC workload may increase if active cases continue or new symptoms appear.",
                SuggestedPreparation = "Review resident monitoring, isolation arrangements and IPC controls."
            });
        }

        if (openCriticalTasks >= 3 || blockedTasks > 0)
        {
            risks.Add(new EmergingRiskDto
            {
                Title = "Operational workload may exceed current capacity",
                Category = "Response",
                Likelihood = "Moderate",
                TimeHorizon = "Today",
                Confidence = "Moderate",
                Status = "Watching",
                Evidence = "Critical or blocked work is still open.",
                OperationalInterpretation = "The team may come under pressure if urgent work remains unresolved.",
                SuggestedPreparation = "Review task ownership and unblock critical work where possible."
            });
        }

        if (lowPpeItems > 0)
        {
            risks.Add(new EmergingRiskDto
            {
                Title = "PPE supplies may need replenishment",
                Category = "Resources",
                Likelihood = lowPpeItems >= 2 ? "High" : "Moderate",
                TimeHorizon = "Next 72 hours",
                Confidence = "Moderate",
                Status = "Watching",
                Evidence = "One or more PPE items have limited days remaining.",
                OperationalInterpretation = "PPE supplies are currently available, but continued usage may create pressure over the next few days.",
                SuggestedPreparation = "Review PPE stock, expected use and upcoming deliveries."
            });
        }

        if (risks.Count == 0)
        {
            risks.Add(new EmergingRiskDto
            {
                Title = "No major emerging risks identified",
                Category = "Operational Brief",
                Likelihood = "Low",
                TimeHorizon = "Today",
                Confidence = "Moderate",
                Status = "Stable",
                Evidence = "Current cases, tasks and PPE indicators do not show a major emerging risk.",
                OperationalInterpretation = "No major emerging risks are visible from current LOCC data.",
                SuggestedPreparation = "Continue routine monitoring."
            });
        }

        return risks;
    }
}