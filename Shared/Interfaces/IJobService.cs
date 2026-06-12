using JobPortal.Features.JobPosting;

namespace JobPortal.Shared.Interfaces;

internal interface IJobService
{
    List<JobListing> GetJobList();
    void ApplyForJob(string jobId);
}
