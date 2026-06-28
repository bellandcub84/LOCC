namespace LOCC.Application.DTOs;

public class SituationAwarenessItemDto
{
    public int SituationAwarenessItemId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Severity { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? RecommendedAction { get; set; }

    public string? OperationalInterpretation { get; set; }

    public DateTime CreatedAt { get; set; }
}