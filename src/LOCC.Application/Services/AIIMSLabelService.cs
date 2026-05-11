using LOCC.Domain;

namespace LOCC.Application.Services;

public static class AIIMSLabelService
{
    public static string GetOperationalLabel(AIIMSFunction function)
    {
        return function switch
        {
            AIIMSFunction.Control => "Outbreak Coordination",
            AIIMSFunction.Operations => "Resident Care & Outbreak Operations",
            AIIMSFunction.Planning => "Outbreak Monitoring & Reporting",
            AIIMSFunction.Logistics => "PPE & Supplies",
            AIIMSFunction.Communications => "Resident, Family & Staff Communications",
            AIIMSFunction.Recovery => "Recovery & Return to BAU",
            _ => function.ToString()
        };
    }
}