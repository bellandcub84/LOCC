using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOCC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    AlertId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    FacilityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    OutbreakId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.AlertId);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditLogId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    Actor = table.Column<string>(type: "TEXT", nullable: true),
                    Details = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditLogId);
                });

            migrationBuilder.CreateTable(
                name: "EvidenceSources",
                columns: table => new
                {
                    EvidenceSourceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    SourceType = table.Column<string>(type: "TEXT", nullable: true),
                    Citation = table.Column<string>(type: "TEXT", nullable: true),
                    Jurisdiction = table.Column<string>(type: "TEXT", nullable: true),
                    AppliesToModule = table.Column<string>(type: "TEXT", nullable: true),
                    LastReviewed = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvidenceSources", x => x.EvidenceSourceId);
                });

            migrationBuilder.CreateTable(
                name: "Facilities",
                columns: table => new
                {
                    FacilityId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FacilityName = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderName = table.Column<string>(type: "TEXT", nullable: true),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    TotalBeds = table.Column<int>(type: "INTEGER", nullable: false),
                    FacilityType = table.Column<int>(type: "INTEGER", nullable: false),
                    PublicHealthUnit = table.Column<string>(type: "TEXT", nullable: true),
                    KeyContacts = table.Column<string>(type: "TEXT", nullable: true),
                    DefaultOutbreakPlanLink = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facilities", x => x.FacilityId);
                });

            migrationBuilder.CreateTable(
                name: "FacilityRooms",
                columns: table => new
                {
                    FacilityRoomId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoomName = table.Column<string>(type: "TEXT", nullable: false),
                    Zone = table.Column<string>(type: "TEXT", nullable: false),
                    RiskLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    IsIsolationRoom = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasConfirmedCase = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasSuspectedCase = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsClosed = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    RiskZoneStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    LastExposureDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EnhancedPrecautionsRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    TerminalCleanRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    ZoningNotes = table.Column<string>(type: "TEXT", nullable: true),
                    CohortStatus = table.Column<string>(type: "TEXT", nullable: true),
                    CohortAssignedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CohortRationale = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityRooms", x => x.FacilityRoomId);
                });

            migrationBuilder.CreateTable(
                name: "SituationAwarenessItems",
                columns: table => new
                {
                    SituationAwarenessItemId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OutbreakId = table.Column<int>(type: "INTEGER", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<int>(type: "INTEGER", nullable: false),
                    Severity = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    SourceType = table.Column<string>(type: "TEXT", nullable: true),
                    SourceReference = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RecommendedAction = table.Column<string>(type: "TEXT", nullable: true),
                    Interpretation = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SituationAwarenessItems", x => x.SituationAwarenessItemId);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FacilityId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LocationName = table.Column<string>(type: "TEXT", nullable: false),
                    LocationType = table.Column<int>(type: "INTEGER", nullable: false),
                    Wing = table.Column<string>(type: "TEXT", nullable: true),
                    ZoneStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    CohortingStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    CleaningStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationId);
                    table.ForeignKey(
                        name: "FK_Locations_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "FacilityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutbreakEvents",
                columns: table => new
                {
                    OutbreakId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FacilityId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OutbreakType = table.Column<string>(type: "TEXT", nullable: false),
                    Pathogen = table.Column<string>(type: "TEXT", nullable: false),
                    DateFirstDetected = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateDeclared = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeclaredBy = table.Column<string>(type: "TEXT", nullable: true),
                    IncidentControllerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    AIIMSStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    OutbreakPhase = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentRiskLevel = table.Column<string>(type: "TEXT", nullable: true),
                    StandDownDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutbreakEvents", x => x.OutbreakId);
                    table.ForeignKey(
                        name: "FK_OutbreakEvents_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "FacilityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Residents",
                columns: table => new
                {
                    ResidentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FacilityId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    RoomId = table.Column<string>(type: "TEXT", nullable: true),
                    Wing = table.Column<string>(type: "TEXT", nullable: true),
                    MobilityStatus = table.Column<string>(type: "TEXT", nullable: true),
                    CognitionRisk = table.Column<string>(type: "TEXT", nullable: true),
                    RespiratoryRisk = table.Column<string>(type: "TEXT", nullable: true),
                    IsolationDistressRisk = table.Column<string>(type: "TEXT", nullable: true),
                    VaccinationStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    BaselineWellbeing = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentStatus = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Residents", x => x.ResidentId);
                    table.ForeignKey(
                        name: "FK_Residents_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "FacilityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    StaffId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FacilityId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    UsualWorkArea = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentWorkZone = table.Column<string>(type: "TEXT", nullable: true),
                    AvailabilityStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    InfectionStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    LastShiftDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FatigueRisk = table.Column<string>(type: "TEXT", nullable: true),
                    CanWorkInOutbreakZone = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.StaffId);
                    table.ForeignKey(
                        name: "FK_Staff_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "FacilityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cases",
                columns: table => new
                {
                    CaseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OutbreakId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PersonType = table.Column<int>(type: "INTEGER", nullable: false),
                    PersonId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CaseStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    OnsetDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LocationAtOnset = table.Column<string>(type: "TEXT", nullable: true),
                    LikelyExposureZone = table.Column<string>(type: "TEXT", nullable: true),
                    IsolationStartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RecoveryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Outcome = table.Column<int>(type: "INTEGER", nullable: true),
                    OutbreakEventOutbreakId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cases", x => x.CaseId);
                    table.ForeignKey(
                        name: "FK_Cases_OutbreakEvents_OutbreakEventOutbreakId",
                        column: x => x.OutbreakEventOutbreakId,
                        principalTable: "OutbreakEvents",
                        principalColumn: "OutbreakId");
                });

            migrationBuilder.CreateTable(
                name: "Communications",
                columns: table => new
                {
                    CommunicationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OutbreakId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Audience = table.Column<string>(type: "TEXT", nullable: true),
                    CommunicationType = table.Column<string>(type: "TEXT", nullable: true),
                    DateSent = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SentBy = table.Column<string>(type: "TEXT", nullable: true),
                    TemplateUsed = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    LinkedEvidence = table.Column<string>(type: "TEXT", nullable: true),
                    OutbreakEventOutbreakId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communications", x => x.CommunicationId);
                    table.ForeignKey(
                        name: "FK_Communications_OutbreakEvents_OutbreakEventOutbreakId",
                        column: x => x.OutbreakEventOutbreakId,
                        principalTable: "OutbreakEvents",
                        principalColumn: "OutbreakId");
                });

            migrationBuilder.CreateTable(
                name: "RecoveryBAUs",
                columns: table => new
                {
                    RecoveryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OutbreakId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RecoveryPhase = table.Column<int>(type: "INTEGER", nullable: false),
                    ActiveCasesScore = table.Column<double>(type: "REAL", nullable: false),
                    WorkforceScore = table.Column<double>(type: "REAL", nullable: false),
                    IPCComplianceScore = table.Column<double>(type: "REAL", nullable: false),
                    EnvironmentScore = table.Column<double>(type: "REAL", nullable: false),
                    ResidentWellbeingScore = table.Column<double>(type: "REAL", nullable: false),
                    BAUReadinessScore = table.Column<double>(type: "REAL", nullable: false),
                    RecommendedNextStep = table.Column<string>(type: "TEXT", nullable: true),
                    CanStandDown = table.Column<bool>(type: "INTEGER", nullable: false),
                    DebriefCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    QIActionsCreated = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecoveryBAUs", x => x.RecoveryId);
                    table.ForeignKey(
                        name: "FK_RecoveryBAUs_OutbreakEvents_OutbreakId",
                        column: x => x.OutbreakId,
                        principalTable: "OutbreakEvents",
                        principalColumn: "OutbreakId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    ResourceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OutbreakId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ResourceType = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemName = table.Column<string>(type: "TEXT", nullable: false),
                    CurrentStock = table.Column<int>(type: "INTEGER", nullable: false),
                    DailyBurnRate = table.Column<int>(type: "INTEGER", nullable: false),
                    DaysRemaining = table.Column<int>(type: "INTEGER", nullable: false),
                    ReorderThreshold = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    DailyUsageRate = table.Column<double>(type: "REAL", nullable: false),
                    CurrentStockLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    MinimumSafeStockLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    ProjectedDaysRemaining = table.Column<int>(type: "INTEGER", nullable: false),
                    OutbreakEventOutbreakId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.ResourceId);
                    table.ForeignKey(
                        name: "FK_Resources_OutbreakEvents_OutbreakEventOutbreakId",
                        column: x => x.OutbreakEventOutbreakId,
                        principalTable: "OutbreakEvents",
                        principalColumn: "OutbreakId");
                });

            migrationBuilder.CreateTable(
                name: "RiskAssessments",
                columns: table => new
                {
                    RiskAssessmentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OutbreakId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SignalType = table.Column<string>(type: "TEXT", nullable: false),
                    SignalSummary = table.Column<string>(type: "TEXT", nullable: false),
                    IPCInterpretation = table.Column<string>(type: "TEXT", nullable: false),
                    RiskLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    AssessedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OutbreakEventOutbreakId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskAssessments", x => x.RiskAssessmentId);
                    table.ForeignKey(
                        name: "FK_RiskAssessments_OutbreakEvents_OutbreakEventOutbreakId",
                        column: x => x.OutbreakEventOutbreakId,
                        principalTable: "OutbreakEvents",
                        principalColumn: "OutbreakId");
                });

            migrationBuilder.CreateTable(
                name: "SurveillanceCases",
                columns: table => new
                {
                    SurveillanceCaseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OutbreakId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PersonType = table.Column<string>(type: "TEXT", nullable: false),
                    ResidentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    StaffId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    RoomName = table.Column<string>(type: "TEXT", nullable: true),
                    Zone = table.Column<string>(type: "TEXT", nullable: true),
                    CaseStatus = table.Column<string>(type: "TEXT", nullable: false),
                    Pathogen = table.Column<string>(type: "TEXT", nullable: true),
                    SymptomOnsetDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Symptoms = table.Column<string>(type: "TEXT", nullable: true),
                    TestType = table.Column<string>(type: "TEXT", nullable: true),
                    TestDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TestResult = table.Column<string>(type: "TEXT", nullable: true),
                    VaccinationStatus = table.Column<string>(type: "TEXT", nullable: true),
                    AntiviralStatus = table.Column<string>(type: "TEXT", nullable: true),
                    HospitalTransferred = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deceased = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsolationStartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsolationEndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RecoveryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Jurisdiction = table.Column<string>(type: "TEXT", nullable: true),
                    PublicHealthNotificationStatus = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    OutbreakEventOutbreakId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveillanceCases", x => x.SurveillanceCaseId);
                    table.ForeignKey(
                        name: "FK_SurveillanceCases_OutbreakEvents_OutbreakEventOutbreakId",
                        column: x => x.OutbreakEventOutbreakId,
                        principalTable: "OutbreakEvents",
                        principalColumn: "OutbreakId");
                });

            migrationBuilder.CreateTable(
                name: "SymptomRecords",
                columns: table => new
                {
                    SymptomId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CaseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DateRecorded = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Fever = table.Column<bool>(type: "INTEGER", nullable: false),
                    Cough = table.Column<bool>(type: "INTEGER", nullable: false),
                    SoreThroat = table.Column<bool>(type: "INTEGER", nullable: false),
                    GastroSymptoms = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShortnessOfBreath = table.Column<bool>(type: "INTEGER", nullable: false),
                    BehaviourChange = table.Column<bool>(type: "INTEGER", nullable: false),
                    ClinicalConcernLevel = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SymptomRecords", x => x.SymptomId);
                    table.ForeignKey(
                        name: "FK_SymptomRecords_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "CaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestRecords",
                columns: table => new
                {
                    TestId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CaseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TestType = table.Column<string>(type: "TEXT", nullable: false),
                    TestDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PathogenTested = table.Column<string>(type: "TEXT", nullable: false),
                    Result = table.Column<int>(type: "INTEGER", nullable: false),
                    ResultDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EnteredBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRecords", x => x.TestId);
                    table.ForeignKey(
                        name: "FK_TestRecords_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "CaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recommendations",
                columns: table => new
                {
                    RecommendationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OutbreakId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RiskAssessmentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    RecommendationText = table.Column<string>(type: "TEXT", nullable: false),
                    Rationale = table.Column<string>(type: "TEXT", nullable: false),
                    SourceRule = table.Column<string>(type: "TEXT", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OutbreakEventOutbreakId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recommendations", x => x.RecommendationId);
                    table.ForeignKey(
                        name: "FK_Recommendations_OutbreakEvents_OutbreakEventOutbreakId",
                        column: x => x.OutbreakEventOutbreakId,
                        principalTable: "OutbreakEvents",
                        principalColumn: "OutbreakId");
                    table.ForeignKey(
                        name: "FK_Recommendations_RiskAssessments_RiskAssessmentId",
                        column: x => x.RiskAssessmentId,
                        principalTable: "RiskAssessments",
                        principalColumn: "RiskAssessmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Interventions",
                columns: table => new
                {
                    InterventionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OutbreakId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RecommendationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    InterventionType = table.Column<string>(type: "TEXT", nullable: false),
                    OperationalArea = table.Column<string>(type: "TEXT", nullable: false),
                    IntendedOutcome = table.Column<string>(type: "TEXT", nullable: false),
                    OutbreakEventOutbreakId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interventions", x => x.InterventionId);
                    table.ForeignKey(
                        name: "FK_Interventions_OutbreakEvents_OutbreakEventOutbreakId",
                        column: x => x.OutbreakEventOutbreakId,
                        principalTable: "OutbreakEvents",
                        principalColumn: "OutbreakId");
                    table.ForeignKey(
                        name: "FK_Interventions_Recommendations_RecommendationId",
                        column: x => x.RecommendationId,
                        principalTable: "Recommendations",
                        principalColumn: "RecommendationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskActions",
                columns: table => new
                {
                    TaskId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OutbreakId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AIIMSFunction = table.Column<int>(type: "INTEGER", nullable: false),
                    TaskCategory = table.Column<string>(type: "TEXT", nullable: true),
                    TaskDescription = table.Column<string>(type: "TEXT", nullable: false),
                    AssignedTo = table.Column<Guid>(type: "TEXT", nullable: true),
                    DueDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CompletionTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EvidenceRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    EvidenceUploaded = table.Column<string>(type: "TEXT", nullable: true),
                    GeneratedFrom = table.Column<string>(type: "TEXT", nullable: true),
                    DecisionRationale = table.Column<string>(type: "TEXT", nullable: true),
                    RiskAssessmentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    RecommendationId = table.Column<Guid>(type: "TEXT", nullable: true),
                    OutbreakEventOutbreakId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskActions", x => x.TaskId);
                    table.ForeignKey(
                        name: "FK_TaskActions_OutbreakEvents_OutbreakEventOutbreakId",
                        column: x => x.OutbreakEventOutbreakId,
                        principalTable: "OutbreakEvents",
                        principalColumn: "OutbreakId");
                    table.ForeignKey(
                        name: "FK_TaskActions_Recommendations_RecommendationId",
                        column: x => x.RecommendationId,
                        principalTable: "Recommendations",
                        principalColumn: "RecommendationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskActions_RiskAssessments_RiskAssessmentId",
                        column: x => x.RiskAssessmentId,
                        principalTable: "RiskAssessments",
                        principalColumn: "RiskAssessmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cases_OutbreakEventOutbreakId",
                table: "Cases",
                column: "OutbreakEventOutbreakId");

            migrationBuilder.CreateIndex(
                name: "IX_Communications_OutbreakEventOutbreakId",
                table: "Communications",
                column: "OutbreakEventOutbreakId");

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_OutbreakEventOutbreakId",
                table: "Interventions",
                column: "OutbreakEventOutbreakId");

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_RecommendationId",
                table: "Interventions",
                column: "RecommendationId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_FacilityId",
                table: "Locations",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_OutbreakEvents_FacilityId",
                table: "OutbreakEvents",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_OutbreakEventOutbreakId",
                table: "Recommendations",
                column: "OutbreakEventOutbreakId");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_RiskAssessmentId",
                table: "Recommendations",
                column: "RiskAssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_RecoveryBAUs_OutbreakId",
                table: "RecoveryBAUs",
                column: "OutbreakId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Residents_FacilityId",
                table: "Residents",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_OutbreakEventOutbreakId",
                table: "Resources",
                column: "OutbreakEventOutbreakId");

            migrationBuilder.CreateIndex(
                name: "IX_RiskAssessments_OutbreakEventOutbreakId",
                table: "RiskAssessments",
                column: "OutbreakEventOutbreakId");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_FacilityId",
                table: "Staff",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveillanceCases_OutbreakEventOutbreakId",
                table: "SurveillanceCases",
                column: "OutbreakEventOutbreakId");

            migrationBuilder.CreateIndex(
                name: "IX_SymptomRecords_CaseId",
                table: "SymptomRecords",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskActions_OutbreakEventOutbreakId",
                table: "TaskActions",
                column: "OutbreakEventOutbreakId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskActions_RecommendationId",
                table: "TaskActions",
                column: "RecommendationId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskActions_RiskAssessmentId",
                table: "TaskActions",
                column: "RiskAssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRecords_CaseId",
                table: "TestRecords",
                column: "CaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Communications");

            migrationBuilder.DropTable(
                name: "EvidenceSources");

            migrationBuilder.DropTable(
                name: "FacilityRooms");

            migrationBuilder.DropTable(
                name: "Interventions");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "RecoveryBAUs");

            migrationBuilder.DropTable(
                name: "Residents");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "SituationAwarenessItems");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropTable(
                name: "SurveillanceCases");

            migrationBuilder.DropTable(
                name: "SymptomRecords");

            migrationBuilder.DropTable(
                name: "TaskActions");

            migrationBuilder.DropTable(
                name: "TestRecords");

            migrationBuilder.DropTable(
                name: "Recommendations");

            migrationBuilder.DropTable(
                name: "Cases");

            migrationBuilder.DropTable(
                name: "RiskAssessments");

            migrationBuilder.DropTable(
                name: "OutbreakEvents");

            migrationBuilder.DropTable(
                name: "Facilities");
        }
    }
}
