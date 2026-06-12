namespace JobPortal.Shared.Exceptions;

internal class ReportGenerationException : JPNSException
{
    public ReportGenerationException(string message) : base(message) { }
    public ReportGenerationException(string message, Exception inner) : base(message, inner) { }
}
