namespace LOCC.Application.DTOs;

public class UpdateZoneRoomRequest
{
   public string? RiskLevel { get; set; }

    public bool? IsClosed { get; set; }

    public bool? IsIsolationRoom { get; set; }

    public bool? TerminalCleanCompleted { get; set; }

    public string? Notes { get; set; }

    public string? CohortStatus { get; set; }

    public string? CohortRationale { get; set; }
}