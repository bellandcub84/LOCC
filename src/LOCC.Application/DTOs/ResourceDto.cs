namespace LOCC.Application.DTOs;

public class ResourceDto
{
    public Guid ResourceId { get; set; }

    public string ItemName { get; set; } = string.Empty;

    public string ResourceType { get; set; } = string.Empty;

    public int DaysRemaining { get; set; }

    public int ReorderThreshold { get; set; }

    public int CurrentStockLevel { get; set; }

    public double DailyUsageRate { get; set; }

    public int MinimumSafeStockLevel { get; set; }

    public int ProjectedDaysRemaining { get; set; }
}