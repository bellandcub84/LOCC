namespace LOCC.Application.Calculators.PPE;

public class PPEOutbreakProfileService
{
    public PPEOutbreakProfile GetProfile(string outbreakType)
    {
        return (outbreakType ?? string.Empty).Trim().ToLower() switch
        {
            "covid-19" or "covid" => new PPEOutbreakProfile
            {
                OutbreakType = "COVID-19",
                RespiratoryPpeRequired = true,
                AgpRespiratoryEscalationRequired = true,
                SurgicalMaskMultiplier = 1.2m,
                N95Multiplier = 1.5m,
                EyeProtectionMultiplier = 1.3m,
                GownMultiplier = 1.1m,
                GloveMultiplier = 1.1m
            },

            "influenza" or "flu" => new PPEOutbreakProfile
            {
                OutbreakType = "Influenza",
                RespiratoryPpeRequired = true,
                AgpRespiratoryEscalationRequired = true,
                SurgicalMaskMultiplier = 1.1m,
                N95Multiplier = 1.2m,
                EyeProtectionMultiplier = 1.1m,
                GownMultiplier = 1.0m,
                GloveMultiplier = 1.0m
            },

            "rsv" => new PPEOutbreakProfile
            {
                OutbreakType = "RSV",
                RespiratoryPpeRequired = true,
                AgpRespiratoryEscalationRequired = true,
                SurgicalMaskMultiplier = 1.1m,
                N95Multiplier = 1.2m,
                EyeProtectionMultiplier = 1.1m,
                GownMultiplier = 1.0m,
                GloveMultiplier = 1.0m
            },

            "norovirus" or "gastro" or "gastroenteritis" => new PPEOutbreakProfile
            {
                OutbreakType = "Norovirus/Gastroenteritis",
                RespiratoryPpeRequired = false,
                AgpRespiratoryEscalationRequired = false,
                SurgicalMaskMultiplier = 0m,
                N95Multiplier = 0m,
                EyeProtectionMultiplier = 0m,
                GownMultiplier = 1.4m,
                GloveMultiplier = 1.4m
            },

            "scabies" => new PPEOutbreakProfile
            {
                OutbreakType = "Scabies",
                RespiratoryPpeRequired = false,
                AgpRespiratoryEscalationRequired = false,
                SurgicalMaskMultiplier = 0m,
                N95Multiplier = 0m,
                EyeProtectionMultiplier = 0m,
                GownMultiplier = 1.5m,
                GloveMultiplier = 1.5m
            },

            _ => new PPEOutbreakProfile
            {
                OutbreakType = outbreakType ?? "Unknown",
                RespiratoryPpeRequired = false,
                AgpRespiratoryEscalationRequired = false,
                SurgicalMaskMultiplier = 0m,
                N95Multiplier = 0m,
                EyeProtectionMultiplier = 0m,
                GownMultiplier = 1.0m,
                GloveMultiplier = 1.0m
            }
        };
    }
}