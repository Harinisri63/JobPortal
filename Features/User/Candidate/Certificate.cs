namespace JobPortal.Features.User.Candidate
{
    public class Certificate
    {
        public int CertificateId { get; set; }
        public int UserId { get; set; }
        public string CertName { get; set; } = string.Empty;
        public string? IssuingOrg { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? CertificateUrl { get; set; }
        public string? CredentialId { get; set; }
    }
}
