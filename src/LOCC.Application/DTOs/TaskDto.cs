namespace LOCC.Application.DTOs;

public class TaskDto
{
    public Guid TaskId { get; set; }

    public string TaskDescription { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string OperationalArea { get; set; } = string.Empty;

    public DateTime? DueDateTime { get; set; }

    public string? GeneratedFrom { get; set; }
    
    public string? DecisionRationale { get; set; }
}