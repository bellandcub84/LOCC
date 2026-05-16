namespace LOCC.Application.DTOs;

public class PPECalculatorRequestDto
{
    public string FacilityName { get; set; } = string.Empty;

    public int TotalResidents { get; set; }
    public int TwoPersonAssistResidents { get; set; }
    public int ThreePlusPersonAssistResidents { get; set; }

    public int ContactDropletResidents { get; set; }
    public int ContactOnlyResidents { get; set; }

    public int StaffOnShift { get; set; }
    public int VisitorsPerDay { get; set; }

    public PPEInterventionInputDto ContactDropletInterventions { get; set; } = new();
    public PPEInterventionInputDto ContactOnlyInterventions { get; set; } = new();

    public string OutbreakType { get; set; } = string.Empty;
}

public class PPEInterventionInputDto
{
    public int RegularNebulisers { get; set; }
    public int PrnNebulisers { get; set; }

    public int RegularOxygen { get; set; }
    public int PrnOxygen { get; set; }

    public int CpapBipap { get; set; }
    public int OralSuctioning { get; set; }
    public int AssistedFeeding { get; set; }
}