namespace JobPortal.Shared.Exceptions;

internal class DuplicateApplicationException : JPNSException
{
    public DuplicateApplicationException(): base("You have already applied for this job.") { }
}
