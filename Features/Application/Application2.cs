namespace JobPortal.Features.Application
{
    public class Application2
    {
        public string ApplicationId { get; set; }
        public int UserId { get; set; }
        public int JobId { get; set; }
        public int ResumeId { get; set; }
        public string? ResumeSnapshot { get; set; }
        public string? CoverLetter { get; set; }
        public string Status { get; set; } = "PENDING";
        public DateTime AppliedDate { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? JobTitle { get; set; }
        public string? CompanyName { get; set; }
        public string? ApplicantFullName { get; set; }
        public string? ApplicantEmail { get; set; }
    }
}