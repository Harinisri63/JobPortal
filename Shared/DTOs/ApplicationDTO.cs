namespace JobPortal.Core.DTOs;

internal class ApplicationDTO
{
    public string ApplicationId { get; set; } = "";
    public string JobTitle      { get; set; } = "";
    public string Company       { get; set; } = "";
    public string Status        { get; set; } = "";
    public DateTime AppliedDate { get; set; }
    public string CandidateName { get; set; } = "";
    public string CandidateEmail{ get; set; } = "";
}
