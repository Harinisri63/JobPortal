namespace JobPortal.Features.User.Candidate
{
    public class JobSeekerProfile
    {
        public int ProfileId { get; set; }
        public int UserId { get; set; }
        public string? Headline { get; set; }
        public string? About { get; set; }
        public string? CurrentLocation { get; set; }
        public int ExperienceYears { get; set; } = 0;
        public string? LinkedInUrl { get; set; }
        public string? PortfolioUrl { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
