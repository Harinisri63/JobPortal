using System;
using JobPortal.Features.JobPosting;
using JobPortal.Shared.Interfaces;

namespace JobPortal.Features.JobPosting.Interface
{
    public interface IJobRepository : IRepository<JobListing>
    {
        Task<int> PostJobAsync(int postedByUserId, int companyId, string title,
                               string description, string location, string jobType,
                               decimal? salaryMin, decimal? salaryMax,
                               int experienceRequired, DateTime expiryDate, int numberOfRounds = 3);

        Task<IEnumerable<JobListing>> SearchJobsAsync(string? keyword = null,
                                                       decimal? salaryMin = null,
                                                       decimal? salaryMax = null);

        Task<IEnumerable<JobListing>> GetPendingJobsAsync();
        Task<IEnumerable<JobListing>> GetByCompanyAsync(int companyId);
        Task<IEnumerable<JobListing>> GetByStatusAsync(string status);

        Task<int> AddJobSkillAsync(int jobId, int skillId, bool isRequired = true);
        Task<bool> RemoveJobSkillAsync(int jobId, int skillId);
        Task<IEnumerable<JobSkill>> GetJobSkillsAsync(int jobId);

        Task<int> SaveJobAsync(int userId, int jobId);
        Task<bool> UnsaveJobAsync(int userId, int jobId);
        Task<IEnumerable<SavedJob>> GetSavedJobsByUserAsync(int userId);
        Task<bool> IsJobSavedAsync(int userId, int jobId);
    }
}
