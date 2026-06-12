namespace JobPortal.Core.DTOs;

internal class JobSummaryDTO
{
    public string JobId    { get; set; } = "";
    public string Title    { get; set; } = "";
    public string Company  { get; set; } = "";
    public string Status   { get; set; } = "";
    public DateTime PostedDate { get; set; }
    public DateTime ExpiryDate { get; set; }
}
