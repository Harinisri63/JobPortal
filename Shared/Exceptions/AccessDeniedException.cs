namespace JobPortal.Shared.Exceptions;

internal class AccessDeniedException : JPNSException
{
    public AccessDeniedException(string message) : base(message) { }
}
