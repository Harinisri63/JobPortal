namespace JobPortal.Features.Interview
{
    public class Interview2
    {
        public int InterviewId { get; set; }
        public int ApplicationId { get; set; }
        public int ScheduledByUserId { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string Mode { get; set; } = string.Empty;
        public string? MeetingLink { get; set; }
        public string? Venue { get; set; }
        public string Status { get; set; } = "SCHEDULED";
        public string? Feedback { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}