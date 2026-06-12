namespace JobPortal.Shared.Exceptions;

internal class JPNSException : Exception
{
    public JPNSException(string message) : base(message) { }
    public JPNSException(string message, Exception inner) : base(message, inner) { }
}
