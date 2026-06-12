using JobPortal.Features.User.Candidate;
using JobPortal.Features.Application;

namespace JobPortal.Features.User.Candidate.Interface
{
    public interface IJobSeekerRepository
    {
        Task<JobSeekerProfile?> GetProfileByUserIdAsync(int userId);
        Task<int> InsertProfileAsync(JobSeekerProfile profile);
        Task<bool> UpdateProfileAsync(JobSeekerProfile profile);

        Task<Resume?> GetResumeByIdAsync(int resumeId);
        Task<IEnumerable<Resume>> GetResumesByUserAsync(int userId);
        Task<int> InsertResumeAsync(Resume resume);
        Task<bool> UpdateResumeAsync(Resume resume);
        Task<bool> DeleteResumeAsync(int resumeId);
        Task<bool> SetDefaultResumeAsync(int userId, int resumeId);

        Task<Education?> GetEducationByIdAsync(int educationId);
        Task<IEnumerable<Education>> GetEducationByUserAsync(int userId);
        Task<int> InsertEducationAsync(Education education);
        Task<bool> UpdateEducationAsync(Education education);
        Task<bool> DeleteEducationAsync(int educationId);

        Task<WorkExperience?> GetExperienceByIdAsync(int experienceId);
        Task<IEnumerable<WorkExperience>> GetExperienceByUserAsync(int userId);
        Task<int> InsertExperienceAsync(WorkExperience experience);
        Task<bool> UpdateExperienceAsync(WorkExperience experience);
        Task<bool> DeleteExperienceAsync(int experienceId);

        Task<Project?> GetProjectByIdAsync(int projectId);
        Task<IEnumerable<Project>> GetProjectsByUserAsync(int userId);
        Task<int> InsertProjectAsync(Project project);
        Task<bool> UpdateProjectAsync(Project project);
        Task<bool> DeleteProjectAsync(int projectId);

        Task<Certificate?> GetCertificateByIdAsync(int certificateId);
        Task<IEnumerable<Certificate>> GetCertificatesByUserAsync(int userId);
        Task<int> InsertCertificateAsync(Certificate certificate);
        Task<bool> UpdateCertificateAsync(Certificate certificate);
        Task<bool> DeleteCertificateAsync(int certificateId);

        Task<IEnumerable<CandidateSkill>> GetSkillsByUserAsync(int userId);
        Task<int> AddSkillAsync(int userId, int skillId, string proficiencyLevel, int? yearsExperience);
        Task<bool> UpdateSkillAsync(int userId, int skillId, string proficiencyLevel, int? yearsExperience);
        Task<bool> RemoveSkillAsync(int userId, int skillId);

        Task<int> ApplyForJobAsync(int userId, int jobId, int resumeId, string? coverLetter = null);

        Task<Application2?> GetApplicationByIdAsync(int applicationId);
        Task<IEnumerable<Application2>> GetApplicationsByUserAsync(int userId);
        Task<bool> UpdateApplicationStatusAsync(int applicationId, string newStatus, int updatedByUserId);
    }
}
