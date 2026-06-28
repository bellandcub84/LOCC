namespace LOCC.Application.DTOs;

public class EmergingRiskDto
{
    public string Title { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Likelihood { get; set; } = string.Empty;

    public string TimeHorizon { get; set; } = string.Empty;

    public string Confidence { get; set; } = string.Empty;

    public string Evidence { get; set; } = string.Empty;

    public string SuggestedPreparation { get; set; } = string.Empty;

    public string OperationalInterpretation { get; set; } = string.Empty;

    public string Status { get; set; } = "Watching";
}