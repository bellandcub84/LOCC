namespace LOCC.Application.Capabilities;

public static class CapabilityRegistry
{
    public static IReadOnlyList<CapabilityMetadata> All => new List<CapabilityMetadata>
    {
        new()
        {
            Id = "OA-001",
            Name = "Operational Dashboard",
            Domain = "Operational Awareness",
            OperationalQuestion = "What is happening right now?",
            Purpose = "Provides a clear operational view of the current outbreak response.",
            MaturityLevel = "Level 2 - Operational Awareness",
            Status = "In progress"
        },
        new()
        {
            Id = "OA-002",
            Name = "Operational Health",
            Domain = "Operational Awareness",
            OperationalQuestion = "How are we coping?",
            Purpose = "Shows whether the organisation is stable, under pressure, or needs attention.",
            MaturityLevel = "Level 3 - Decision Support",
            Status = "Implemented"
        },
        new()
        {
            Id = "OA-003",
            Name = "Situation Awareness",
            Domain = "Operational Awareness",
            OperationalQuestion = "What has changed?",
            Purpose = "Highlights recent changes that may need operational attention.",
            MaturityLevel = "Level 2 - Operational Awareness",
            Status = "Implemented"
        },
        new()
        {
            Id = "OA-004",
            Name = "Emerging Risks",
            Domain = "Operational Awareness",
            OperationalQuestion = "What could affect us next?",
            Purpose = "Identifies near-term operational risks that may need monitoring or preparation.",
            MaturityLevel = "Level 3 - Decision Support",
            Status = "Implemented"
        }
    };
}