namespace JobPortal.Features.User.Candidate
{
    public class Resume
    {
        public int ResumeId { get; set; }
        public int UserId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
        public DateTime UploadedAt { get; set; }
    }
}
