using System;
using System.Collections.Generic;

namespace LOCC.Domain
{
    // Enums used across the domain. Using a single file here for brevity.
    public enum RoleType { IncidentController, IPCLead, OperationsLead, LogisticsLead, CommunicationsAdmin, ExecutiveViewer, Nurse, CareWorker }
    public enum FacilityType { ResidentialAgedCare, RetirementVillage, Other }
    public enum OutbreakPhase { PreSuspect, Suspected, Declared, ActiveControl, Stabilising, ControlledReopening, ReturnToBAU, Closed }
    public enum AIIMSFunction { Control, Operations, Planning, Intelligence, Logistics, Communications, Recovery }
    public enum AlertType { ClusterSuspected, OutbreakRecommended, PPEWarning, StaffingWarning, PersonCentredRisk, RecoverySuggestion }
    public enum ResourceType { PPE, Medication, Consumable, Equipment }
    public enum PersonType { Resident, Staff }
    public enum CaseStatus { Suspected, Confirmed, Recovered, Deceased }
    public enum TestResult { Positive, Negative, Invalid, Pending }
    public enum Priority { Low, Medium, High, Critical }
    public enum TaskStatus { Pending, InProgress, Escalated, Blocked, Completed, Cancelled }
    public enum LocationType { Room, Wing, CommonArea }
    public enum ZoneStatus { Green, Amber, Red }
    public enum CohortingStatus { None, Cohorted, Isolated }
    public enum CleaningStatus { NotDue, Due, InProgress, Completed }
    public enum VaccinationStatus { Unknown, UpToDate, Partial, NotVaccinated }
    public enum AvailabilityStatus { Available, Unavailable, OnLeave }
    public enum InfectionStatus { NotInfected, Suspected, Confirmed, Recovered }
    public enum Outcome { Stable, Deteriorated, Transferred, Deceased }
    public enum RecoveryPhase { Stage1_ActiveControl, Stage2_Stabilising, Stage3_ControlledReopening, Stage4_ReturnToBAU, Stage5_Closed }
    public enum ClinicalConcernLevel { Low, Moderate, High }
    public enum RoomRiskLevel {  Low, Moderate, High, Critical}
       
    }