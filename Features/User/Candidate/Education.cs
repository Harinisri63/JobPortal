namespace JobPortal.Features.User.Candidate
{
    public class Education
    {
        public int EducationId { get; set; }
        public int UserId { get; set; }
        public string Degree { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string? FieldOfStudy { get; set; }
        public int? StartYear { get; set; }
        public int? EndYear { get; set; }
        public string? Grade { get; set; }
        public string? Description { get; set; }
    }
}
