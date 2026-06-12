namespace JobPortal.Features.JobPosting;

internal sealed class JobConfig
{
    public const int    MaxDescriptionLength = 2000;
    public const int    MinSkillCount        = 1;
    public const int    MaxSavedJobs         = 50;
    public const int    MaxTitleLength       = 200;
    public const int    MinTitleLength       = 3;
    public const int    MaxCoverLetterLength = 1000;
    public const int    MaxExpiryDays        = 365;
}
