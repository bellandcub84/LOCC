using System;
using System.Collections.Generic;

namespace LOCC.Domain.Entities
{
    public class Facility
    {
        public Guid FacilityId { get; set; }
        public string FacilityName { get; set; } = string.Empty;
        public string? ProviderName { get; set; }
        public string? Address { get; set; }
        public int TotalBeds { get; set; }
        public LOCC.Domain.FacilityType FacilityType { get; set; }
        public string? PublicHealthUnit { get; set; }
        public string? KeyContacts { get; set; }
        public string? DefaultOutbreakPlanLink { get; set; }

        // Navigation
        public List<Resident> Residents { get; set; } = new();
        public List<Staff> Staff { get; set; } = new();
        public List<Location> Locations { get; set; } = new();
        public List<OutbreakEvent> OutbreakEvents { get; set; } = new();
    }

    public class Resident
    {
        public Guid ResidentId { get; set; }
        public Guid FacilityId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? RoomId { get; set; }
        public string? Wing { get; set; }
        public string? MobilityStatus { get; set; }
        public string? CognitionRisk { get; set; }
        public string? RespiratoryRisk { get; set; }
        public string? IsolationDistressRisk { get; set; }
        public LOCC.Domain.VaccinationStatus VaccinationStatus { get; set; }
        public string? BaselineWellbeing { get; set; }
        public string? CurrentStatus { get; set; }

        // Navigation
        public Facility? Facility { get; set; }
    }

    public class Staff
    {
        public Guid StaffId { get; set; }
        public Guid FacilityId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public LOCC.Domain.RoleType Role { get; set; }
        public string? UsualWorkArea { get; set; }
        public string? CurrentWorkZone { get; set; }
        public LOCC.Domain.AvailabilityStatus AvailabilityStatus { get; set; }
        public LOCC.Domain.InfectionStatus InfectionStatus { get; set; }
        public DateTime? LastShiftDate { get; set; }
        public string? FatigueRisk { get; set; }
        public bool CanWorkInOutbreakZone { get; set; }

        // Navigation
        public Facility? Facility { get; set; }
    }

    public class Location
    {
        public Guid LocationId { get; set; }
        public Guid FacilityId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public LOCC.Domain.LocationType LocationType { get; set; }
        public string? Wing { get; set; }
        public LOCC.Domain.ZoneStatus ZoneStatus { get; set; }
        public LOCC.Domain.CohortingStatus CohortingStatus { get; set; }
        public LOCC.Domain.CleaningStatus CleaningStatus { get; set; }

        public Facility? Facility { get; set; }
    }

    public class OutbreakEvent
    {
        public Guid OutbreakId { get; set; }
        public Guid FacilityId { get; set; }
        public string OutbreakType { get; set; } = string.Empty;
        public string Pathogen { get; set; } = string.Empty;
        public DateTime DateFirstDetected { get; set; }
        public DateTime? DateDeclared { get; set; }
        public string? DeclaredBy { get; set; }
        public Guid? IncidentControllerId { get; set; }
        public LOCC.Domain.AIIMSFunction AIIMSStatus { get; set; }
        public LOCC.Domain.OutbreakPhase OutbreakPhase { get; set; }
        public string? CurrentRiskLevel { get; set; }
        public DateTime? StandDownDate { get; set; }

        // Navigation
        public Facility? Facility { get; set; }
        public List<Case> Cases { get; set; } = new();
        public List<TaskAction> Tasks { get; set; } = new();
        public List<Resource> Resources { get; set; } = new();
        public List<Communication> Communications { get; set; } = new();
        public RecoveryBAU? RecoveryBAU { get; set; }
    }

    public class Case
    {
        public Guid CaseId { get; set; }
        public Guid OutbreakId { get; set; }
        public LOCC.Domain.PersonType PersonType { get; set; }
        public Guid PersonId { get; set; }
        public LOCC.Domain.CaseStatus CaseStatus { get; set; }
        public DateTime? OnsetDate { get; set; }
        public string? LocationAtOnset { get; set; }
        public string? LikelyExposureZone { get; set; }
        public DateTime? IsolationStartDate { get; set; }
        public DateTime? RecoveryDate { get; set; }
        public LOCC.Domain.Outcome? Outcome { get; set; }

        public OutbreakEvent? OutbreakEvent { get; set; }
        public List<SymptomRecord> SymptomRecords { get; set; } = new();
        public List<TestRecord> TestRecords { get; set; } = new();
    }

    public class SymptomRecord
    {
        public Guid SymptomId { get; set; }
        public Guid CaseId { get; set; }
        public DateTime DateRecorded { get; set; }
        public bool Fever { get; set; }
        public bool Cough { get; set; }
        public bool SoreThroat { get; set; }
        public bool GastroSymptoms { get; set; }
        public bool ShortnessOfBreath { get; set; }
        public bool BehaviourChange { get; set; }
        public LOCC.Domain.ClinicalConcernLevel ClinicalConcernLevel { get; set; }

        public Case? Case { get; set; }
    }

    public class TestRecord
    {
        public Guid TestId { get; set; }
        public Guid CaseId { get; set; }
        public string TestType { get; set; } = string.Empty; // RAT, PCR
        public DateTime TestDate { get; set; }
        public string PathogenTested { get; set; } = string.Empty;
        public LOCC.Domain.TestResult Result { get; set; }
        public DateTime? ResultDate { get; set; }
        public string? EnteredBy { get; set; }

        public Case? Case { get; set; }
    }

public class TaskAction
{
    public Guid TaskId { get; set; }

    public Guid OutbreakId { get; set; }

    public LOCC.Domain.AIIMSFunction AIIMSFunction { get; set; }

    public string? TaskCategory { get; set; }

    public string TaskDescription { get; set; } = string.Empty;

    public Guid? AssignedTo { get; set; }

    public DateTime? DueDateTime { get; set; }

    public LOCC.Domain.Priority Priority { get; set; }

    public LOCC.Domain.TaskStatus Status { get; set; }

    public DateTime? CompletionTime { get; set; }

    public bool EvidenceRequired { get; set; }

    public string? EvidenceUploaded { get; set; }

    // NEW: explainability + source tracking
    public string? GeneratedFrom { get; set; }

    public string? DecisionRationale { get; set; }

    // FUTURE relationship placeholders
    public Guid? RiskAssessmentId { get; set; }

    public Guid? RecommendationId { get; set; }

    // Navigation
    public OutbreakEvent? OutbreakEvent { get; set; }

    public RiskAssessment? RiskAssessment { get; set; }
    
    public Recommendation? Recommendation { get; set; }
}

public class RiskAssessment
{
    public Guid RiskAssessmentId { get; set; }
    public Guid OutbreakId { get; set; }

    public string SignalType { get; set; } = string.Empty;
    public string SignalSummary { get; set; } = string.Empty;
    public string IPCInterpretation { get; set; } = string.Empty;

    public LOCC.Domain.Priority RiskLevel { get; set; }

    public DateTime AssessedAt { get; set; }

    public OutbreakEvent? OutbreakEvent { get; set; }
}

public class Recommendation
{
    public Guid RecommendationId { get; set; }
    public Guid OutbreakId { get; set; }

    public Guid? RiskAssessmentId { get; set; }

    public string RecommendationText { get; set; } = string.Empty;
    public string Rationale { get; set; } = string.Empty;
    public string SourceRule { get; set; } = string.Empty;

    public LOCC.Domain.Priority Priority { get; set; }

    public DateTime CreatedAt { get; set; }

    public OutbreakEvent? OutbreakEvent { get; set; }
    public RiskAssessment? RiskAssessment { get; set; }
}

public class Intervention
{
    public Guid InterventionId { get; set; }
    public Guid OutbreakId { get; set; }

    public Guid RecommendationId { get; set; }

    public string InterventionType { get; set; } = string.Empty;
    public string OperationalArea { get; set; } = string.Empty;
    public string IntendedOutcome { get; set; } = string.Empty;

    public OutbreakEvent? OutbreakEvent { get; set; }
    public Recommendation? Recommendation { get; set; }
}

    public class Resource
    {
        public Guid ResourceId { get; set; }
        public Guid OutbreakId { get; set; }
        public LOCC.Domain.ResourceType ResourceType { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int DailyBurnRate { get; set; }
        public int DaysRemaining { get; set; }
        public int ReorderThreshold { get; set; }
        public string? Status { get; set; }

        public OutbreakEvent? OutbreakEvent { get; set; }
    }

    public class Communication
    {
        public Guid CommunicationId { get; set; }
        public Guid OutbreakId { get; set; }
        public string? Audience { get; set; }
        public string? CommunicationType { get; set; }
        public DateTime? DateSent { get; set; }
        public string? SentBy { get; set; }
        public string? TemplateUsed { get; set; }
        public string? Status { get; set; }
        public string? LinkedEvidence { get; set; }

        public OutbreakEvent? OutbreakEvent { get; set; }
    }

    public class RecoveryBAU
    {
        public Guid RecoveryId { get; set; }
        public Guid OutbreakId { get; set; }
        public LOCC.Domain.RecoveryPhase RecoveryPhase { get; set; }
        public double ActiveCasesScore { get; set; }
        public double WorkforceScore { get; set; }
        public double IPCComplianceScore { get; set; }
        public double EnvironmentScore { get; set; }
        public double ResidentWellbeingScore { get; set; }
        public double BAUReadinessScore { get; set; }
        public string? RecommendedNextStep { get; set; }
        public bool CanStandDown { get; set; }
        public bool DebriefCompleted { get; set; }
        public int QIActionsCreated { get; set; }

        public OutbreakEvent? OutbreakEvent { get; set; }
    }

    public class EvidenceSource
    {
        public Guid EvidenceSourceId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? SourceType { get; set; }
        public string? Citation { get; set; }
        public string? Jurisdiction { get; set; }
        public string? AppliesToModule { get; set; }
        public DateTime? LastReviewed { get; set; }
        public string? Notes { get; set; }
    }

    public class Alert
    {
        public Guid AlertId { get; set; }
        public AlertType Type { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? FacilityId { get; set; }
        public Guid? OutbreakId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? Metadata { get; set; }
    }

    public class AuditLog
    {
        public Guid AuditLogId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? Actor { get; set; }
        public string? Details { get; set; }
    }
    public class FacilityRoom
    {
        public int FacilityRoomId { get; set; }

        public string RoomName { get; set; } = "";

        public string Zone { get; set; } = "";

        public RoomRiskLevel RiskLevel { get; set; }

        public bool IsIsolationRoom { get; set; }

        public bool HasConfirmedCase { get; set; }

        public bool HasSuspectedCase { get; set; }

        public bool IsClosed { get; set; }

        public string Notes { get; set; } = "";
    }
}