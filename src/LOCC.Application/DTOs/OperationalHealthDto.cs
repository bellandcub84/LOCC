namespace LOCC.Application.DTOs;

public class OperationalHealthDto
{
    public string Question { get; set; } = "How are we coping?";

    public string OverallStatus { get; set; } = string.Empty;

    public string Trend { get; set; } = "Stable";

    public string Confidence { get; set; } = "Moderate";

    public string OperationalInterpretation { get; set; } = string.Empty;

    public List<string> SuggestedFocus { get; set; } = [];

    public List<OperationalHealthDimensionDto> Dimensions { get; set; } = [];
}

public class OperationalHealthDimensionDto
{
    public string Name { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;
}