namespace JobPortal.Features.JobPosting;

internal abstract class BaseJobFilter
{
    public abstract List<JobListing> FilterJobs(List<JobListing> jobs);
    public abstract double CalculateRelevance(JobListing job);

    public string GetFilterSummary() => $"Filter: {GetType().Name}";
}
