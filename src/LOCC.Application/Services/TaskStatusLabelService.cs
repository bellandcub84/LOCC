using DomainTaskStatus = LOCC.Domain.TaskStatus;

namespace LOCC.Application.Services;

public static class TaskStatusLabelService
{
    public static string GetDisplayLabel(DomainTaskStatus status)
    {
        return status switch
        {
            DomainTaskStatus.Pending => "Pending",
            DomainTaskStatus.InProgress => "In Progress",
            DomainTaskStatus.Escalated => "Escalated",
            DomainTaskStatus.Blocked => "Blocked",
            DomainTaskStatus.Completed => "Completed",
            DomainTaskStatus.Cancelled => "Cancelled",
            _ => status.ToString()
        };
    }
}