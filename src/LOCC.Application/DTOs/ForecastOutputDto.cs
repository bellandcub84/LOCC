namespace LOCC.Application.DTOs;

public class PPEForecastResultDto
{
    public string FacilityName { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    public int TotalResidents { get; set; }
    public int SingleAssistResidents { get; set; }

    public List<PPEForecastScenarioDto> Scenarios { get; set; } = new();
}

public class PPEForecastScenarioDto
{
    public int PercentAffected { get; set; }
    public int EstimatedAffectedResidents { get; set; }

    public int Gloves { get; set; }
    public int Gowns { get; set; }
    public int Aprons { get; set; }
    public int SurgicalMasks { get; set; }
    public int N95Respirators { get; set; }
    public int EyeProtection { get; set; }
}