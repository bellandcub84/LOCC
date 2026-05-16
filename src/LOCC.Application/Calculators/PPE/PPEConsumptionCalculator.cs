using LOCC.Application.DTOs;

namespace LOCC.Application.Calculators.PPE;

public class PPEConsumptionCalculator
{
    private readonly PPEAssumptionSet _assumptions = new();
    private readonly PPEOutbreakProfileService _profileService = new();

    public PPEForecastResultDto Calculate(PPECalculatorRequestDto request)
    {
        var outbreakProfile = _profileService.GetProfile(request.OutbreakType);

        var contactDropletInterventions =
            CalculateInterventions(request.ContactDropletInterventions);

        var contactOnlyInterventions =
            CalculateInterventions(request.ContactOnlyInterventions);

        var visitorInteractions =
            request.VisitorsPerDay * _assumptions.VisitorInteractionsPerVisitor;

        var scenarios = new List<int>
        {
            0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50,
            55, 60, 65, 70, 75, 80, 85, 90, 95, 100
        };

        var singleAssistResidents =
            request.TotalResidents -
            (request.TwoPersonAssistResidents +
            request.ThreePlusPersonAssistResidents);

        return new PPEForecastResultDto
        {
            FacilityName = request.FacilityName,
            TotalResidents = request.TotalResidents,
            SingleAssistResidents = singleAssistResidents,
            Scenarios = scenarios.Select(percent =>
                CalculateScenario(
                    request,
                    percent,
                    contactDropletInterventions,
                    contactOnlyInterventions,
                    visitorInteractions,
                    outbreakProfile
                )
            ).ToList()
        };
    }

    private int ApplyMultiplier(int value, decimal multiplier)
    {
        return (int)Math.Ceiling(value * multiplier);
    }

    private int CalculateInterventions(PPEInterventionInputDto input)
    {
        if (input == null)
        {
            return 0;
        }

        return
            (input.RegularNebulisers * _assumptions.RegularNebuliserInteractionsPerDay) +
            (input.PrnNebulisers * _assumptions.PrnNebuliserInteractionsPerDay) +
            (input.RegularOxygen * _assumptions.RegularOxygenInteractionsPerDay) +
            (input.PrnOxygen * _assumptions.PrnOxygenInteractionsPerDay) +
            (input.CpapBipap * _assumptions.CpapBipapInteractionsPerDay) +
            (input.OralSuctioning * _assumptions.OralSuctioningInteractionsPerDay) +
            (input.AssistedFeeding * _assumptions.AssistedFeedingInteractionsPerDay);
    }

    private PPEForecastScenarioDto CalculateScenario(
        PPECalculatorRequestDto request,
        int percentAffected,
        int contactDropletInterventions,
        int contactOnlyInterventions,
        int visitorInteractions,
        PPEOutbreakProfile outbreakProfile)
    {
        var affectedResidents =
            (int)Math.Ceiling(request.TotalResidents * (percentAffected / 100.0));

        var outbreakMultiplier = affectedResidents;

        return new PPEForecastScenarioDto
        {
            PercentAffected = percentAffected,
            EstimatedAffectedResidents = affectedResidents,

            Gloves = ApplyMultiplier(
                (request.TotalResidents * _assumptions.BaseGlovesPerResidentPerDay) +
                request.StaffOnShift +
                contactDropletInterventions +
                contactOnlyInterventions +
                visitorInteractions +
                outbreakMultiplier,
                outbreakProfile.GloveMultiplier),

            Gowns = ApplyMultiplier(
                (contactDropletInterventions / 2) +
                contactOnlyInterventions +
                outbreakMultiplier,
                outbreakProfile.GownMultiplier),

            Aprons =
                contactDropletInterventions +
                contactOnlyInterventions +
                visitorInteractions,

            SurgicalMasks =
                CalculateSurgicalMasks(request, contactDropletInterventions, percentAffected, outbreakProfile),

            N95Respirators =
                CalculateN95s(request, percentAffected, outbreakProfile),

            EyeProtection =
                CalculateEyeProtection(request, contactDropletInterventions, percentAffected, outbreakProfile)
        };
    }

    private int CalculateSurgicalMasks(
        PPECalculatorRequestDto request,
        int contactDropletInterventions,
        int percentAffected,
        PPEOutbreakProfile outbreakProfile)
    {
        if (!outbreakProfile.RespiratoryPpeRequired)
        {
            return 0;
        }

        var baseValue = request.StaffOnShift + contactDropletInterventions + percentAffected;

        return ApplyMultiplier(baseValue, outbreakProfile.SurgicalMaskMultiplier);
    }

    private int CalculateN95s(
        PPECalculatorRequestDto request,
        int percentAffected,
        PPEOutbreakProfile outbreakProfile)
    {
        if (!outbreakProfile.AgpRespiratoryEscalationRequired)
        {
            return 0;
        }

        var interventions = request.ContactDropletInterventions;

        if (interventions == null)
        {
            return 0;
        }

        var agpCount =
            interventions.RegularNebulisers +
            interventions.PrnNebulisers +
            interventions.CpapBipap +
            interventions.OralSuctioning;

        var baseValue = agpCount + percentAffected;

        return ApplyMultiplier(baseValue, outbreakProfile.N95Multiplier);
    }
    private int CalculateEyeProtection(
        PPECalculatorRequestDto request,
        int contactDropletInterventions,
        int percentAffected,
        PPEOutbreakProfile outbreakProfile)
    {
        if (!outbreakProfile.RespiratoryPpeRequired)
        {
            return 0;
        }

        var baseValue = contactDropletInterventions + percentAffected;

        return ApplyMultiplier(baseValue, outbreakProfile.EyeProtectionMultiplier);
    }
}