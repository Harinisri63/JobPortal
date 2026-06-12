namespace JobPortal.Shared.Exceptions;

internal class JobNotAvailableException : JPNSException
{
    public JobNotAvailableException(string reason = "This job is no longer available."): base(reason) { }
}
