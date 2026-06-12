namespace JobPortal.Shared.DTOs;

internal class ReportDTO
{
    public string ReportId      { get; set; } = Guid.NewGuid().ToString();
    public string ReportType    { get; set; } = "";
    public DateTime GeneratedAt { get; set; } = DateTime.Now;
    public string GeneratedBy   { get; set; } = "";
    public Dictionary<string, object> Data { get; set; } = new();
    public string Summary       { get; set; } = "";
}
