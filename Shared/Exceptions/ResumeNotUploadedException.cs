namespace JobPortal.Shared.Exceptions;

internal class ResumeNotUploadedException : JPNSException
{
    public ResumeNotUploadedException(): base("Please upload your resume before applying.") { }
}
