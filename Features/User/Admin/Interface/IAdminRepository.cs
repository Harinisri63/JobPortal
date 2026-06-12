using JobPortal.Features.Application;
using JobPortal.Features.Interview;
using JobPortal.Features.User.Candidate;
using JobPortal.Features.JobPosting;
using JobPortal.Database;

namespace JobPortal.Features.User.Admin.Interface
{
    public interface IAdminRepository
    {
        Task<CompanyProfile?> GetCompanyByIdAsync(int companyId);
        Task<IEnumerable<CompanyProfile>> GetAllCompaniesAsync();
        Task<IEnumerable<CompanyProfile>> GetVerifiedCompaniesAsync();
        Task<int> InsertCompanyAsync(CompanyProfile company);
        Task<bool> UpdateCompanyAsync(CompanyProfile company);
        Task<bool> DeleteCompanyAsync(int companyId);
        Task<bool> SetCompanyVerifiedAsync(int companyId, bool isVerified);

        Task<bool> ApproveJobAsync(int jobId, int adminUserId, string decision);

        Task<IEnumerable<Application2>> GetApplicationsByJobAsync(int jobId);

        Task<int> ScheduleInterviewAsync(int applicationId, int scheduledByUserId,
                                          DateTime scheduledAt, string mode,
                                          string? meetingLink = null, string? venue = null);

        Task<IEnumerable<Interview2>> GetInterviewsByApplicationAsync(int applicationId);
        Task<bool> UpdateInterviewStatusAsync(int interviewId, string status, string? feedback = null);

        Task<int> SendNotificationAsync(int userId, string message,
                                         string notificationType = "SYSTEM", int? referenceId = null);

        Task<IEnumerable<NotificationModel>> GetNotificationsAsync(int userId, bool unreadOnly = false);
        Task<bool> MarkNotificationsReadAsync(int userId);

        Task<IEnumerable<Feedback>> GetFeedbackByCompanyAsync(int companyId);
        Task<int> InsertFeedbackAsync(Feedback feedback);
        Task<bool> DeleteFeedbackAsync(int feedbackId);

        Task<Complaint?> GetComplaintByIdAsync(int complaintId);
        Task<IEnumerable<Complaint>> GetAllComplaintsAsync();
        Task<IEnumerable<Complaint>> GetComplaintsByStatusAsync(string status);
        Task<int> InsertComplaintAsync(Complaint complaint);
        Task<bool> UpdateComplaintStatusAsync(int complaintId, string status, string? adminNotes = null);

        Task<IEnumerable<Skill>> GetAllSkillsAsync();
        Task<int> InsertSkillAsync(string skillName, string? category);
        Task<bool> DeleteSkillAsync(int skillId);
    }
}
