namespace JobPortal.Shared.Exceptions;

internal class JobExpiredException : JPNSException
{
    public JobExpiredException(): base("This job posting has expired.") { }
}
