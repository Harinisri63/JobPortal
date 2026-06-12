using JobPortal.Features.JobPosting;

namespace JobPortal.Features.JobPosting
{
    public class SavedJob
    {
        public int SavedJobId { get; set; }

        public int UserId { get; set; }

        public int JobId { get; set; }

        public string? ConsoleJobId { get; set; }

        public string JobTitle { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public DateTime SavedAt { get; set; } = DateTime.Now;

        public JobListing? Job { get; set; }
    }
}