using System;
using System.Linq;
using LOCC.Domain;
using LOCC.Domain.Entities;

namespace LOCC.Infrastructure.Seed
{
    public static class SeedData
    {
        public static void EnsureSeedData(LoccDbContext context)
        {
            if (context.Facilities.Any()) return;

            var facility = new Facility
            {
                FacilityId = Guid.NewGuid(),
                FacilityName = "Rosewood Aged Care",
                ProviderName = "Rosewood Healthcare Pty Ltd",
                Address = "12 Example St, Melbourne VIC",
                TotalBeds = 85,
                FacilityType = FacilityType.ResidentialAgedCare,
                PublicHealthUnit = "Melbourne PHU",
                KeyContacts = "IC: James Wilson; IPC: Dr Sarah Chen",
                DefaultOutbreakPlanLink = "https://example.org/rosewood/outbreak-plan.pdf"
            };

            context.Facilities.Add(facility);

            var r1 = new Resident { ResidentId = Guid.NewGuid(), FacilityId = facility.FacilityId, FirstName = "Margaret", LastName = "Thompson", Wing = "East", IsolationDistressRisk = "High", VaccinationStatus = VaccinationStatus.UpToDate, BaselineWellbeing = "Good", CurrentStatus = "Isolated" };
            var r2 = new Resident { ResidentId = Guid.NewGuid(), FacilityId = facility.FacilityId, FirstName = "Robert", LastName = "Singh", Wing = "East", RespiratoryRisk = "High", VaccinationStatus = VaccinationStatus.Partial, BaselineWellbeing = "Fair", CurrentStatus = "Isolated" };
            var r3 = new Resident { ResidentId = Guid.NewGuid(), FacilityId = facility.FacilityId, FirstName = "Dorothy", LastName = "Martinez", Wing = "North", IsolationDistressRisk = "High", VaccinationStatus = VaccinationStatus.UpToDate, BaselineWellbeing = "Poor", CurrentStatus = "Observed" };
            var r4 = new Resident { ResidentId = Guid.NewGuid(), FacilityId = facility.FacilityId, FirstName = "Norman", LastName = "Jackson", Wing = "West", VaccinationStatus = VaccinationStatus.UpToDate, BaselineWellbeing = "Good", CurrentStatus = "Well" };
            var r5 = new Resident { ResidentId = Guid.NewGuid(), FacilityId = facility.FacilityId, FirstName = "Joyce", LastName = "Anderson", Wing = "East", RespiratoryRisk = "High", VaccinationStatus = VaccinationStatus.UpToDate, BaselineWellbeing = "Fair", CurrentStatus = "Isolated" };

            context.Residents.AddRange(r1, r2, r3, r4, r5);

            var sIC = new Staff { StaffId = Guid.NewGuid(), FacilityId = facility.FacilityId, FirstName = "James", LastName = "Wilson", Role = RoleType.IncidentController, AvailabilityStatus = AvailabilityStatus.Available, InfectionStatus = InfectionStatus.NotInfected, CanWorkInOutbreakZone = true };
            var sIPC = new Staff { StaffId = Guid.NewGuid(), FacilityId = facility.FacilityId, FirstName = "Sarah", LastName = "Chen", Role = RoleType.IPCLead, AvailabilityStatus = AvailabilityStatus.Available, InfectionStatus = InfectionStatus.NotInfected, CanWorkInOutbreakZone = true };
            var sOps = new Staff { StaffId = Guid.NewGuid(), FacilityId = facility.FacilityId, FirstName = "Michael", LastName = "Torres", Role = RoleType.OperationsLead, AvailabilityStatus = AvailabilityStatus.Available, InfectionStatus = InfectionStatus.NotInfected, CanWorkInOutbreakZone = true };

            context.Staff.AddRange(sIC, sIPC, sOps);

            var locEast = new Location
            {
                LocationId = Guid.NewGuid(),
                FacilityId = facility.FacilityId,
                LocationName = "East Wing",
                LocationType = LocationType.Wing,
                Wing = "East",
                ZoneStatus = ZoneStatus.Red,
                CohortingStatus = CohortingStatus.Isolated,
                CleaningStatus = CleaningStatus.InProgress
            };

            var locNorth = new Location
            {
                LocationId = Guid.NewGuid(),
                FacilityId = facility.FacilityId,
                LocationName = "North Wing",
                LocationType = LocationType.Wing,
                Wing = "North",
                ZoneStatus = ZoneStatus.Amber,
                CohortingStatus = CohortingStatus.Cohorted,
                CleaningStatus = CleaningStatus.Due
            };

            context.Locations.AddRange(locEast, locNorth);

            var outbreak = new OutbreakEvent
            {
                OutbreakId = Guid.NewGuid(),
                FacilityId = facility.FacilityId,
                OutbreakType = "Respiratory Cluster",
                Pathogen = "SARS-CoV-2",
                DateFirstDetected = DateTime.UtcNow.AddDays(-1),
                OutbreakPhase = OutbreakPhase.PreSuspect,
                AIIMSStatus = AIIMSFunction.Intelligence,
                CurrentRiskLevel = "Moderate"
            };

            context.OutbreakEvents.Add(outbreak);

            var c1 = new Case
            {
                CaseId = Guid.NewGuid(),
                OutbreakId = outbreak.OutbreakId,
                PersonType = PersonType.Resident,
                PersonId = r1.ResidentId,
                CaseStatus = CaseStatus.Suspected,
                OnsetDate = DateTime.UtcNow.AddHours(-20),
                LocationAtOnset = "East Wing",
                LikelyExposureZone = "East"
            };

            var c2 = new Case
            {
                CaseId = Guid.NewGuid(),
                OutbreakId = outbreak.OutbreakId,
                PersonType = PersonType.Resident,
                PersonId = r2.ResidentId,
                CaseStatus = CaseStatus.Suspected,
                OnsetDate = DateTime.UtcNow.AddHours(-16),
                LocationAtOnset = "East Wing",
                LikelyExposureZone = "East"
            };

            context.Cases.AddRange(c1, c2);

            context.TestRecords.AddRange(
                new TestRecord
                {
                    TestId = Guid.NewGuid(),
                    CaseId = c1.CaseId,
                    TestType = "RAT",
                    TestDate = DateTime.UtcNow.AddHours(-6),
                    PathogenTested = "SARS-CoV-2",
                    Result = TestResult.Positive,
                    ResultDate = DateTime.UtcNow.AddHours(-5),
                    EnteredBy = "Nurse A"
                },
                new TestRecord
                {
                    TestId = Guid.NewGuid(),
                    CaseId = c2.CaseId,
                    TestType = "RAT",
                    TestDate = DateTime.UtcNow.AddHours(-4),
                    PathogenTested = "SARS-CoV-2",
                    Result = TestResult.Pending,
                    EnteredBy = "Nurse B"
                }
            );

            context.Resources.AddRange(
                new Resource
                {
                    ResourceId = Guid.NewGuid(),
                    OutbreakId = outbreak.OutbreakId,
                    ResourceType = ResourceType.PPE,
                    ItemName = "N95 Masks",
                    CurrentStock = 30,
                    DailyBurnRate = 10,
                    DaysRemaining = 3,
                    ReorderThreshold = 5,
                    Status = "InUse"
                },
                new Resource
                {
                    ResourceId = Guid.NewGuid(),
                    OutbreakId = outbreak.OutbreakId,
                    ResourceType = ResourceType.PPE,
                    ItemName = "Gowns",
                    CurrentStock = 10,
                    DailyBurnRate = 5,
                    DaysRemaining = 2,
                    ReorderThreshold = 3,
                    Status = "InUse"
                }
            );

            context.AuditLogs.Add(
                new AuditLog
                {
                    AuditLogId = Guid.NewGuid(),
                    Timestamp = DateTime.UtcNow,
                    Action = "OutbreakSeeded",
                    Actor = "system",
                    Details = "Seeded outbreak with two suspected resident cases in East wing"
                }
            );

            context.EvidenceSources.Add(
                new EvidenceSource
                {
                    EvidenceSourceId = Guid.NewGuid(),
                    Title = "CDNA outbreak definitions - placeholder",
                    SourceType = "Guideline",
                    Citation = "CDNA",
                    Jurisdiction = "AU",
                    AppliesToModule = "OutbreakDeclaration"
                }
            );

            context.FacilityRooms.AddRange(
                new FacilityRoom
                {
                    RoomName = "Room 101",
                    Zone = "North Wing",
                    RiskLevel = RoomRiskLevel.Critical,
                    HasConfirmedCase = true,
                    IsIsolationRoom = true
                },
                new FacilityRoom
                {
                    RoomName = "Room 102",
                    Zone = "North Wing",
                    RiskLevel = RoomRiskLevel.High,
                    HasSuspectedCase = true
                },
                new FacilityRoom
                {
                    RoomName = "Room 201",
                    Zone = "South Wing",
                    RiskLevel = RoomRiskLevel.Low
                }
            );

            context.SituationAwarenessItems.AddRange(
                new SituationAwarenessItem
                {
                    Title = "Important work is blocked",
                    Summary = "A critical task has been marked as blocked.",
                    Category = SituationAwarenessCategory.TaskRisk,
                    Severity = SituationAwarenessSeverity.ActionRequired,
                    Status = SituationAwarenessStatus.New,
                    SourceType = "Task",
                    Interpretation = "Important work is stuck and may need support.",
                    RecommendedAction = "Review the blocked task and decide who can unblock it."
                },
                new SituationAwarenessItem
                {
                    Title = "PPE supplies need monitoring",
                    Summary = "Some PPE items may run low soon.",
                    Category = SituationAwarenessCategory.ResourcePressure,
                    Severity = SituationAwarenessSeverity.Monitor,
                    Status = SituationAwarenessStatus.New,
                    SourceType = "Resource",
                    Interpretation = "Supplies may not last as long as planned.",
                    RecommendedAction = "Review current stock and expected use."
                }
            );

            context.SaveChanges();
        }
    }
}