namespace LOCC.Application.Calculators.PPE;

public class PPEAssumptionSet
{
    public int RegularNebuliserInteractionsPerDay { get; set; } = 2;
    public int PrnNebuliserInteractionsPerDay { get; set; } = 2;

    public int RegularOxygenInteractionsPerDay { get; set; } = 2;
    public int PrnOxygenInteractionsPerDay { get; set; } = 4;

    public int CpapBipapInteractionsPerDay { get; set; } = 2;
    public int OralSuctioningInteractionsPerDay { get; set; } = 6;
    public int AssistedFeedingInteractionsPerDay { get; set; } = 3;

    public int VisitorInteractionsPerVisitor { get; set; } = 3;
    public int BaseGlovesPerResidentPerDay { get; set; } = 2;

    public int ToiletingContinenceInteractionsPerResident { get; set; } = 10;
    public int PersonalHygieneInteractionsPerResident { get; set; } = 1;
    public int AssistedFeedInteractionsPerResident { get; set; } = 6;
}