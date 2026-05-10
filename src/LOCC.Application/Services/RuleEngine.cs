using System;
using System.Linq;
using System.Collections.Generic;
using LOCC.Infrastructure;
using LOCC.Domain.Entities;
using LOCC.Domain;
using DomainTaskStatus = LOCC.Domain.TaskStatus;

namespace LOCC.Application.Services
{
    public class RuleEvaluationResult
   {
        public List<Alert> Alerts { get; set; } = new();
        public List<TaskAction> Tasks { get; set; } = new();
        public RecoveryBAU? Recovery { get; set; }
   }

    public class RuleEngine
    {
        private readonly LoccDbContext _db;
        public RuleEngine(LoccDbContext db)
        {
            _db = db;
        }

        // Evaluate rules against the current state and persist alerts/tasks/recovery
        public RuleEvaluationResult EvaluateAll()
        {
            var result = new RuleEvaluationResult();

            // Example Rule 1: Cluster detection in same wing within 48 hours
            var recentCases = _db.Cases.Where(c => c.OnsetDate != null && c.OnsetDate > DateTime.UtcNow.AddHours(-48)).ToList();
            var residents = _db.Residents.ToList();
            var clusters = recentCases
                .Where(c => c.PersonType == PersonType.Resident)
                .GroupBy(c => c.LikelyExposureZone)
                .Where(g => g.Count() >= 2)
                .ToList();

            foreach (var cluster in clusters)
            {
                var alert = new Alert { AlertId = Guid.NewGuid(), Type = AlertType.ClusterSuspected, Message = $"Cluster suspected in zone {cluster.Key} with {cluster.Count()} linked symptomatic residents.", CreatedAt = DateTime.UtcNow };
                result.Alerts.Add(alert);
            }

            // Example Rule 2: Outbreak declaration support
            foreach (var cluster in clusters)
            {
                // Simpler: if cluster exists and any positive test or 3+ symptomatic
                var anyPositive = _db.TestRecords.Any(t => t.Result == TestResult.Positive && cluster.Select(c => c.CaseId).Contains(t.CaseId));
                if (anyPositive || cluster.Count() >= 3)
                {
                    var alert = new Alert { AlertId = Guid.NewGuid(), Type = AlertType.OutbreakRecommended, Message = $"Outbreak declaration recommended for zone {cluster.Key} (positive test or ≥3 linked symptomatic cases).", CreatedAt = DateTime.UtcNow };
                    result.Alerts.Add(alert);

                    // Auto-generate AIIMS task bundle
                    var tasks = GenerateInitialAIIMSTasks(_db.OutbreakEvents.First());
                    result.Tasks.AddRange(tasks);

                   // Create RecoveryBAU record if not exists
                    var ob = _db.OutbreakEvents.First();

                    var existingRecovery = _db.RecoveryBAUs
                        .FirstOrDefault(r => r.OutbreakId == ob.OutbreakId);

                    if (existingRecovery == null)
                    {
                        var recovery = CreateInitialRecoveryRecord(ob);
                        result.Recovery = recovery;
                    }
                    else
                    {
                        result.Recovery = existingRecovery;
                    }
                }
            }

            // PPE resource warning
            var lowResources = _db.Resources.Where(r => r.DaysRemaining <= r.ReorderThreshold).ToList();
            foreach (var res in lowResources)
            {
                var alert = new Alert { AlertId = Guid.NewGuid(), Type = AlertType.PPEWarning, Message = $"Resource {res.ItemName} low: {res.DaysRemaining} days remaining.", CreatedAt = DateTime.UtcNow };
                result.Alerts.Add(alert);

                // Create reorder task
                var task = new TaskAction { TaskId = Guid.NewGuid(), OutbreakId = res.OutbreakId, AIIMSFunction = AIIMSFunction.Logistics, TaskDescription = $"Reorder {res.ItemName}", Priority = Priority.Critical, Status = DomainTaskStatus.Pending, DueDateTime = DateTime.UtcNow.AddDays(1) };
                result.Tasks.Add(task);
            }

            // Workforce risk: simple threshold 70% available
            var staff = _db.Staff.ToList();
            var available = staff.Count(s => s.AvailabilityStatus == AvailabilityStatus.Available);
            var availPercent = staff.Count == 0 ? 100 : (available * 100.0 / staff.Count);
            if (availPercent < 70)
            {
                var alert = new Alert { AlertId = Guid.NewGuid(), Type = AlertType.StaffingWarning, Message = $"Staff availability low: {availPercent:0.#}% available.", CreatedAt = DateTime.UtcNow };
                result.Alerts.Add(alert);

                var task = new TaskAction { TaskId = Guid.NewGuid(), OutbreakId = _db.OutbreakEvents.First().OutbreakId, AIIMSFunction = AIIMSFunction.Operations, TaskDescription = "Plan agency/backfill staffing", Priority = Priority.High, Status = DomainTaskStatus.Pending, DueDateTime = DateTime.UtcNow.AddDays(1) };
                result.Tasks.Add(task);
            }

            // Person-centred IPC: resident isolated >48h and high distress
            var caseInfo = _db.Cases.ToList();
            foreach (var c in caseInfo)
            {
                var resident = _db.Residents.FirstOrDefault(r => r.ResidentId == c.PersonId);
                if (resident != null && resident.IsolationDistressRisk == "High" && c.IsolationStartDate != null && c.IsolationStartDate < DateTime.UtcNow.AddHours(-48))
                {
                    var alert = new Alert { AlertId = Guid.NewGuid(), Type = AlertType.PersonCentredRisk, Message = $"Resident {resident.FirstName} {resident.LastName} at risk of isolation distress (>48h).", CreatedAt = DateTime.UtcNow };
                    result.Alerts.Add(alert);

                    var task = new TaskAction { TaskId = Guid.NewGuid(), OutbreakId = c.OutbreakId, AIIMSFunction = AIIMSFunction.Operations, TaskDescription = $"Review person-centred isolation support plan for {resident.FirstName} {resident.LastName}", Priority = Priority.Medium, Status = DomainTaskStatus.Pending, DueDateTime = DateTime.UtcNow.AddHours(24) };
                    result.Tasks.Add(task);
                }
            }

            // Persist alerts and tasks
            foreach (var a in result.Alerts)
            {
                _db.Alerts.Add(a);
                _db.AuditLogs.Add(new AuditLog { AuditLogId = Guid.NewGuid(), Timestamp = DateTime.UtcNow, Action = "RuleGeneratedAlert", Actor = "RuleEngine", Details = a.Message });
            }
            foreach (var t in result.Tasks)
            {
                _db.TaskActions.Add(t);
                _db.AuditLogs.Add(new AuditLog { AuditLogId = Guid.NewGuid(), Timestamp = DateTime.UtcNow, Action = "RuleGeneratedTask", Actor = "RuleEngine", Details = t.TaskDescription });
            }

            if (result.Recovery != null)
            {
                _db.RecoveryBAUs.Add(result.Recovery);
                _db.AuditLogs.Add(new AuditLog { AuditLogId = Guid.NewGuid(), Timestamp = DateTime.UtcNow, Action = "RuleCreatedRecovery", Actor = "RuleEngine", Details = "Initial recovery record created" });
            }

            _db.SaveChanges();

            // Calculate BAU readiness score if recovery exists
            if (result.Recovery != null)
            {
                result.Recovery.BAUReadinessScore = CalculateBAUScore(result.Recovery);
                _db.SaveChanges();
            }

            return result;
        }

        private double CalculateBAUScore(RecoveryBAU r)
        {
            return 0.30 * r.ActiveCasesScore + 0.25 * r.WorkforceScore + 0.15 * r.IPCComplianceScore + 0.10 * r.EnvironmentScore + 0.20 * r.ResidentWellbeingScore;
        }

        private RecoveryBAU CreateInitialRecoveryRecord(OutbreakEvent outbreak)
        {
            // Initial domain scores defaults
            var r = new RecoveryBAU
            {
                RecoveryId = Guid.NewGuid(),
                OutbreakId = outbreak.OutbreakId,
                RecoveryPhase = RecoveryPhase.Stage1_ActiveControl,
                ActiveCasesScore = 40,
                WorkforceScore = 50,
                IPCComplianceScore = 60,
                EnvironmentScore = 50,
                ResidentWellbeingScore = 45,
                CanStandDown = false,
                DebriefCompleted = false,
                QIActionsCreated = 0
            };
            r.BAUReadinessScore = CalculateBAUScore(r);
            return r;
        }

        private List<TaskAction> GenerateInitialAIIMSTasks(OutbreakEvent outbreak)
        {
            var tasks = new List<TaskAction>();
            tasks.Add(new TaskAction { TaskId = Guid.NewGuid(), OutbreakId = outbreak.OutbreakId, AIIMSFunction = AIIMSFunction.Control, TaskDescription = "Assign Incident Controller", Priority = Priority.Critical, Status = DomainTaskStatus.Pending, DueDateTime = DateTime.UtcNow.AddHours(1) });
            tasks.Add(new TaskAction { TaskId = Guid.NewGuid(), OutbreakId = outbreak.OutbreakId, AIIMSFunction = AIIMSFunction.Intelligence, TaskDescription = "Confirm outbreak type and pathogen", Priority = Priority.Critical, Status = DomainTaskStatus.Pending, DueDateTime = DateTime.UtcNow.AddHours(4) });
            tasks.Add(new TaskAction { TaskId = Guid.NewGuid(), OutbreakId = outbreak.OutbreakId, AIIMSFunction = AIIMSFunction.Control, TaskDescription = "Notify IPC Lead", Priority = Priority.Critical, Status = DomainTaskStatus.Pending, DueDateTime = DateTime.UtcNow.AddHours(1) });
            tasks.Add(new TaskAction { TaskId = Guid.NewGuid(), OutbreakId = outbreak.OutbreakId, AIIMSFunction = AIIMSFunction.Operations, TaskDescription = "Establish affected zones", Priority = Priority.Critical, Status = DomainTaskStatus.Pending, DueDateTime = DateTime.UtcNow.AddHours(3) });
            tasks.Add(new TaskAction { TaskId = Guid.NewGuid(), OutbreakId = outbreak.OutbreakId, AIIMSFunction = AIIMSFunction.Intelligence, TaskDescription = "Commence enhanced surveillance", Priority = Priority.High, Status = DomainTaskStatus.Pending, DueDateTime = DateTime.UtcNow.AddHours(6) });
            tasks.Add(new TaskAction { TaskId = Guid.NewGuid(), OutbreakId = outbreak.OutbreakId, AIIMSFunction = AIIMSFunction.Logistics, TaskDescription = "Review PPE stock", Priority = Priority.Critical, Status = DomainTaskStatus.Pending, DueDateTime = DateTime.UtcNow.AddHours(2) });
            tasks.Add(new TaskAction { TaskId = Guid.NewGuid(), OutbreakId = outbreak.OutbreakId, AIIMSFunction = AIIMSFunction.Operations, TaskDescription = "Review staffing capacity", Priority = Priority.High, Status = DomainTaskStatus.Pending, DueDateTime = DateTime.UtcNow.AddHours(4) });
            tasks.Add(new TaskAction { TaskId = Guid.NewGuid(), OutbreakId = outbreak.OutbreakId, AIIMSFunction = AIIMSFunction.Communications, TaskDescription = "Prepare family communication", Priority = Priority.High, Status = DomainTaskStatus.Pending, DueDateTime = DateTime.UtcNow.AddHours(8) });
            tasks.Add(new TaskAction { TaskId = Guid.NewGuid(), OutbreakId = outbreak.OutbreakId, AIIMSFunction = AIIMSFunction.Communications, TaskDescription = "Prepare staff communication", Priority = Priority.High, Status = DomainTaskStatus.Pending, DueDateTime = DateTime.UtcNow.AddHours(6) });
            tasks.Add(new TaskAction { TaskId = Guid.NewGuid(), OutbreakId = outbreak.OutbreakId, AIIMSFunction = AIIMSFunction.Recovery, TaskDescription = "Create Recovery/BAU plan", Priority = Priority.High, Status = DomainTaskStatus.Pending, DueDateTime = DateTime.UtcNow.AddHours(12) });
            return tasks;
        }
    }
}
