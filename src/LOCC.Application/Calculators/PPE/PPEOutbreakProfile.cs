namespace LOCC.Application.Calculators.PPE;

public class PPEOutbreakProfile
{
    public string OutbreakType { get; set; } = string.Empty;

    public bool RespiratoryPpeRequired { get; set; }
    public bool AgpRespiratoryEscalationRequired { get; set; }

    public decimal SurgicalMaskMultiplier { get; set; } = 1.0m;
    public decimal N95Multiplier { get; set; } = 1.0m;
    public decimal EyeProtectionMultiplier { get; set; } = 1.0m;
    public decimal GownMultiplier { get; set; } = 1.0m;
    public decimal GloveMultiplier { get; set; } = 1.0m;
}