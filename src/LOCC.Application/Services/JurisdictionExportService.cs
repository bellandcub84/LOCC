using System.Text;
using LOCC.Domain.Entities;

namespace LOCC.Application.Services;

public class JurisdictionExportService
{
    public string ExportSurveillanceCasesToCsv(
        IEnumerable<SurveillanceCase> cases,
        string jurisdiction)
    {
        return jurisdiction.ToUpperInvariant() switch
        {
            "WA" => BuildWaCsv(cases),
            "NSW" => BuildNswCsv(cases),
            "VIC" => BuildVicCsv(cases),
            _ => BuildGenericCsv(cases)
        };
    }

    private string BuildWaCsv(IEnumerable<SurveillanceCase> cases)
    {
        var csv = new StringBuilder();

        csv.AppendLine("Name,Person Type,Room,Zone,Case Status,Pathogen,Symptom Onset Date,Test Type,Test Date,Test Result,Isolation Start,Hospital Transferred,Deceased,Notification Status");

        foreach (var c in cases)
        {
            csv.AppendLine(string.Join(",",
                Escape(c.DisplayName),
                Escape(c.PersonType),
                Escape(c.RoomName),
                Escape(c.Zone),
                Escape(c.CaseStatus),
                Escape(c.Pathogen),
                FormatDate(c.SymptomOnsetDate),
                Escape(c.TestType),
                FormatDate(c.TestDate),
                Escape(c.TestResult),
                FormatDate(c.IsolationStartDate),
                c.HospitalTransferred,
                c.Deceased,
                Escape(c.PublicHealthNotificationStatus)
            ));
        }

        return csv.ToString();
    }

    private string BuildNswCsv(IEnumerable<SurveillanceCase> cases)
    {
        var csv = new StringBuilder();

        csv.AppendLine("Resident/Staff Name,Resident or Staff,Location,Case Classification,Date of Onset,Symptoms,Test Result,Date Tested,Isolation Commenced,PHU Notification Status");

        foreach (var c in cases)
        {
            csv.AppendLine(string.Join(",",
                Escape(c.DisplayName),
                Escape(c.PersonType),
                Escape(c.RoomName),
                Escape(c.CaseStatus),
                FormatDate(c.SymptomOnsetDate),
                Escape(c.Symptoms),
                Escape(c.TestResult),
                FormatDate(c.TestDate),
                FormatDate(c.IsolationStartDate),
                Escape(c.PublicHealthNotificationStatus)
            ));
        }

        return csv.ToString();
    }

    private string BuildVicCsv(IEnumerable<SurveillanceCase> cases)
    {
        var csv = new StringBuilder();

        csv.AppendLine("Name,Type,Room/Wing,Status,Pathogen,Onset Date,Test Type,Test Result,Hospitalised,Deceased,Recovery Date,Notes");

        foreach (var c in cases)
        {
            csv.AppendLine(string.Join(",",
                Escape(c.DisplayName),
                Escape(c.PersonType),
                Escape(c.Zone ?? c.RoomName),
                Escape(c.CaseStatus),
                Escape(c.Pathogen),
                FormatDate(c.SymptomOnsetDate),
                Escape(c.TestType),
                Escape(c.TestResult),
                c.HospitalTransferred,
                c.Deceased,
                FormatDate(c.RecoveryDate),
                Escape(c.Notes)
            ));
        }

        return csv.ToString();
    }

    private string BuildGenericCsv(IEnumerable<SurveillanceCase> cases)
    {
        var csv = new StringBuilder();

        csv.AppendLine("Name,Person Type,Room,Zone,Case Status,Pathogen,Symptom Onset Date,Test Result,Jurisdiction,Notification Status");

        foreach (var c in cases)
        {
            csv.AppendLine(string.Join(",",
                Escape(c.DisplayName),
                Escape(c.PersonType),
                Escape(c.RoomName),
                Escape(c.Zone),
                Escape(c.CaseStatus),
                Escape(c.Pathogen),
                FormatDate(c.SymptomOnsetDate),
                Escape(c.TestResult),
                Escape(c.Jurisdiction),
                Escape(c.PublicHealthNotificationStatus)
            ));
        }

        return csv.ToString();
    }

    private static string FormatDate(DateTime? date)
    {
        return date.HasValue ? date.Value.ToString("yyyy-MM-dd") : "";
    }

    private static string Escape(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "";
        }

        var escaped = value.Replace("\"", "\"\"");

        if (escaped.Contains(",") || escaped.Contains("\"") || escaped.Contains("\n"))
        {
            return $"\"{escaped}\"";
        }

        return escaped;
    }
}