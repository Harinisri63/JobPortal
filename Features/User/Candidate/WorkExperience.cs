namespace JobPortal.Features.User.Candidate
{
    public class WorkExperience
    {
        public int ExperienceId { get; set; }
        public int UserId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string? Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; } = false;
        public string? Description { get; set; }
    }
}
