namespace JobPortal.Features.User.Candidate;

public class Complaint
{
    public int ComplaintId { get; set; }
    public int SubmittedByUserId { get; set; }
    public int? AgainstUserId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "OPEN";
    public string? AdminNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
