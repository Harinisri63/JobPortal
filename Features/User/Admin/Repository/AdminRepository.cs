using Microsoft.Data.SqlClient;
using System.Data;
using JobPortal.Features.JobPosting;
using JobPortal.Features.User.Candidate;
using JobPortal.Features.Application;
using JobPortal.Features.Interview;
using JobPortal.Features.User.Admin.Interface;
using JobPortal.Database;

namespace JobPortal.Features.User.Admin.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DatabaseConnection _db;

        public AdminRepository(DatabaseConnection db)
        {
            _db = db;
        }


        public async Task<CompanyProfile?> GetCompanyByIdAsync(int companyId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT CompanyId, UserId, CompanyName, Industry, CompanySize, Website, " +
                "Location, Description, LogoUrl, IsVerified, CreatedAt, UpdatedAt " +
                "FROM JPNS.CompanyProfiles WHERE CompanyId = @CompanyId", connection);

            command.Parameters.AddWithValue("@CompanyId", companyId);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapCompany(reader);

            return null;
        }

        public async Task<IEnumerable<CompanyProfile>> GetAllCompaniesAsync()
        {
            var list = new List<CompanyProfile>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT CompanyId, UserId, CompanyName, Industry, CompanySize, Website, " +
                "Location, Description, LogoUrl, IsVerified, CreatedAt, UpdatedAt " +
                "FROM JPNS.CompanyProfiles ORDER BY CompanyName", connection);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                list.Add(MapCompany(reader));

            return list;
        }

        public async Task<IEnumerable<CompanyProfile>> GetVerifiedCompaniesAsync()
        {
            var list = new List<CompanyProfile>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT CompanyId, UserId, CompanyName, Industry, CompanySize, Website, " +
                "Location, Description, LogoUrl, IsVerified, CreatedAt, UpdatedAt " +
                "FROM JPNS.CompanyProfiles WHERE IsVerified = 1 ORDER BY CompanyName", connection);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                list.Add(MapCompany(reader));

            return list;
        }

        public async Task<int> InsertCompanyAsync(CompanyProfile company)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "INSERT INTO JPNS.CompanyProfiles " +
                "(UserId, CompanyName, Industry, CompanySize, Website, Location, Description, LogoUrl, IsVerified) " +
                "VALUES (@UserId, @CompanyName, @Industry, @CompanySize, @Website, @Location, @Description, @LogoUrl, @IsVerified); " +
                "SELECT SCOPE_IDENTITY();", connection);

            command.Parameters.AddWithValue("@UserId", company.UserId);
            command.Parameters.AddWithValue("@CompanyName", company.CompanyName);
            command.Parameters.AddWithValue("@Industry", (object?)company.Industry ?? DBNull.Value);
            command.Parameters.AddWithValue("@CompanySize", (object?)company.CompanySize ?? DBNull.Value);
            command.Parameters.AddWithValue("@Website", (object?)company.Website ?? DBNull.Value);
            command.Parameters.AddWithValue("@Location", (object?)company.Location ?? DBNull.Value);
            command.Parameters.AddWithValue("@Description", (object?)company.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@LogoUrl", (object?)company.LogoUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsVerified", company.IsVerified);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateCompanyAsync(CompanyProfile company)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "UPDATE JPNS.CompanyProfiles SET CompanyName = @CompanyName, Industry = @Industry, " +
                "CompanySize = @CompanySize, Website = @Website, Location = @Location, " +
                "Description = @Description, LogoUrl = @LogoUrl, UpdatedAt = GETDATE() " +
                "WHERE CompanyId = @CompanyId", connection);

            command.Parameters.AddWithValue("@CompanyId", company.CompanyId);
            command.Parameters.AddWithValue("@CompanyName", company.CompanyName);
            command.Parameters.AddWithValue("@Industry", (object?)company.Industry ?? DBNull.Value);
            command.Parameters.AddWithValue("@CompanySize", (object?)company.CompanySize ?? DBNull.Value);
            command.Parameters.AddWithValue("@Website", (object?)company.Website ?? DBNull.Value);
            command.Parameters.AddWithValue("@Location", (object?)company.Location ?? DBNull.Value);
            command.Parameters.AddWithValue("@Description", (object?)company.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@LogoUrl", (object?)company.LogoUrl ?? DBNull.Value);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteCompanyAsync(int companyId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "DELETE FROM JPNS.CompanyProfiles WHERE CompanyId = @CompanyId", connection);

            command.Parameters.AddWithValue("@CompanyId", companyId);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> SetCompanyVerifiedAsync(int companyId, bool isVerified)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "UPDATE JPNS.CompanyProfiles SET IsVerified = @IsVerified, UpdatedAt = GETDATE() " +
                "WHERE CompanyId = @CompanyId", connection);

            command.Parameters.AddWithValue("@CompanyId", companyId);
            command.Parameters.AddWithValue("@IsVerified", isVerified);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }


        public async Task<bool> ApproveJobAsync(int jobId, int adminUserId, string decision)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand("JPNS.usp_AdminApproveJob", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@JobId", jobId);
            command.Parameters.AddWithValue("@AdminUserId", adminUserId);
            command.Parameters.AddWithValue("@Decision", decision);

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error during ApproveJob:");
                foreach (SqlError error in ex.Errors)
                    Console.WriteLine(error.Message);
                throw;
            }
        }


        public async Task<IEnumerable<Application2>> GetApplicationsByJobAsync(int jobId)
        {
            var list = new List<Application2>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand("JPNS.usp_GetApplicationsByJob", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@JobId", jobId);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Application2
                {
                    ApplicationId      = reader["ApplicationId"].ToString()!,
                    UserId             = (int)reader["UserId"],
                    Status             = reader["Status"].ToString()!,
                    AppliedDate        = (DateTime)reader["AppliedDate"],
                    CoverLetter        = reader["CoverLetter"] == DBNull.Value ? null : reader["CoverLetter"].ToString(),
                    ApplicantFullName  = reader["FullName"].ToString()!,
                    ApplicantEmail     = reader["Email"].ToString()!
                });
            }

            return list;
        }


        public async Task<int> ScheduleInterviewAsync(int applicationId, int scheduledByUserId,
                                                        DateTime scheduledAt, string mode,
                                                        string? meetingLink = null, string? venue = null)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand("JPNS.usp_ScheduleInterview", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@ApplicationId", applicationId);
            command.Parameters.AddWithValue("@ScheduledByUserId", scheduledByUserId);
            command.Parameters.AddWithValue("@ScheduledAt", scheduledAt);
            command.Parameters.AddWithValue("@Mode", mode);
            command.Parameters.AddWithValue("@MeetingLink", (object?)meetingLink ?? DBNull.Value);
            command.Parameters.AddWithValue("@Venue", (object?)venue ?? DBNull.Value);

            SqlParameter interviewIdParam = new SqlParameter("@InterviewId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(interviewIdParam);

            try
            {
                await command.ExecuteNonQueryAsync();
                return (int)interviewIdParam.Value;
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error during ScheduleInterview:");
                foreach (SqlError error in ex.Errors)
                    Console.WriteLine(error.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Interview2>> GetInterviewsByApplicationAsync(int applicationId)
        {
            var list = new List<Interview2>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand("JPNS.usp_GetInterviewsByApplication", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@ApplicationId", applicationId);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Interview2
                {
                    InterviewId  = (int)reader["InterviewId"],
                    ScheduledAt  = (DateTime)reader["ScheduledAt"],
                    Mode         = reader["Mode"].ToString()!,
                    MeetingLink  = reader["MeetingLink"] == DBNull.Value ? null : reader["MeetingLink"].ToString(),
                    Venue        = reader["Venue"] == DBNull.Value ? null : reader["Venue"].ToString(),
                    Status       = reader["Status"].ToString()!,
                    Feedback     = reader["Feedback"] == DBNull.Value ? null : reader["Feedback"].ToString(),
                    CreatedAt    = (DateTime)reader["CreatedAt"]
                });
            }

            return list;
        }

        public async Task<bool> UpdateInterviewStatusAsync(int interviewId, string status, string? feedback = null)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "UPDATE JPNS.Interviews SET Status = @Status, Feedback = @Feedback, " +
                "UpdatedAt = GETDATE() WHERE InterviewId = @InterviewId", connection);

            command.Parameters.AddWithValue("@InterviewId", interviewId);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@Feedback", (object?)feedback ?? DBNull.Value);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }


        public async Task<int> SendNotificationAsync(int userId, string message,
                                                       string notificationType = "SYSTEM",
                                                       int? referenceId = null)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand("JPNS.usp_SendNotification", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@Message", message);
            command.Parameters.AddWithValue("@NotificationType", notificationType);
            command.Parameters.AddWithValue("@ReferenceId", (object?)referenceId ?? DBNull.Value);

            try
            {
                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error during SendNotification:");
                foreach (SqlError error in ex.Errors)
                    Console.WriteLine(error.Message);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationModel>> GetNotificationsAsync(int userId, bool unreadOnly = false)
        {
            var list = new List<NotificationModel>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand("JPNS.usp_GetNotifications", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@UnreadOnly", unreadOnly);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new NotificationModel
                {
                    NotificationId   = (int)reader["NotificationId"],
                    Message          = reader["Message"].ToString()!,
                    NotificationType = reader["NotificationType"].ToString()!,
                    ReferenceId      = reader["ReferenceId"] == DBNull.Value ? null : (int?)reader["ReferenceId"],
                    IsRead           = (bool)reader["IsRead"],
                    CreatedAt        = (DateTime)reader["CreatedAt"]
                });
            }

            return list;
        }

        public async Task<bool> MarkNotificationsReadAsync(int userId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand("JPNS.usp_MarkNotificationsRead", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@UserId", userId);

            await command.ExecuteNonQueryAsync();
            return true;
        }


        public async Task<IEnumerable<Feedback>> GetFeedbackByCompanyAsync(int companyId)
        {
            var list = new List<Feedback>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT f.FeedbackId, f.UserId, f.CompanyId, f.Rating, f.Review, " +
                "f.IsAnonymous, f.CreatedAt, c.CompanyName " +
                "FROM JPNS.Feedback f " +
                "JOIN JPNS.CompanyProfiles c ON f.CompanyId = c.CompanyId " +
                "WHERE f.CompanyId = @CompanyId ORDER BY f.CreatedAt DESC", connection);

            command.Parameters.AddWithValue("@CompanyId", companyId);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Feedback
                {
                    FeedbackId  = (int)reader["FeedbackId"],
                    UserId      = (int)reader["UserId"],
                    CompanyId   = (int)reader["CompanyId"],
                    CompanyName = reader["CompanyName"].ToString()!,
                    Rating      = (byte)reader["Rating"],
                    Review      = reader["Review"] == DBNull.Value ? null : reader["Review"].ToString(),
                    IsAnonymous = (bool)reader["IsAnonymous"],
                    CreatedAt   = (DateTime)reader["CreatedAt"]
                });
            }

            return list;
        }

        public async Task<int> InsertFeedbackAsync(Feedback feedback)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "INSERT INTO JPNS.Feedback (UserId, CompanyId, Rating, Review, IsAnonymous) " +
                "VALUES (@UserId, @CompanyId, @Rating, @Review, @IsAnonymous); " +
                "SELECT SCOPE_IDENTITY();", connection);

            command.Parameters.AddWithValue("@UserId", feedback.UserId);
            command.Parameters.AddWithValue("@CompanyId", feedback.CompanyId);
            command.Parameters.AddWithValue("@Rating", feedback.Rating);
            command.Parameters.AddWithValue("@Review", (object?)feedback.Review ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsAnonymous", feedback.IsAnonymous);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> DeleteFeedbackAsync(int feedbackId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "DELETE FROM JPNS.Feedback WHERE FeedbackId = @FeedbackId", connection);

            command.Parameters.AddWithValue("@FeedbackId", feedbackId);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }


        public async Task<Complaint?> GetComplaintByIdAsync(int complaintId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT ComplaintId, SubmittedByUserId, AgainstUserId, Subject, Description, " +
                "Status, AdminNotes, CreatedAt, ResolvedAt " +
                "FROM JPNS.Complaints WHERE ComplaintId = @ComplaintId", connection);

            command.Parameters.AddWithValue("@ComplaintId", complaintId);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapComplaint(reader);

            return null;
        }

        public async Task<IEnumerable<Complaint>> GetAllComplaintsAsync()
        {
            var list = new List<Complaint>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT ComplaintId, SubmittedByUserId, AgainstUserId, Subject, Description, " +
                "Status, AdminNotes, CreatedAt, ResolvedAt " +
                "FROM JPNS.Complaints ORDER BY CreatedAt DESC", connection);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                list.Add(MapComplaint(reader));

            return list;
        }

        public async Task<IEnumerable<Complaint>> GetComplaintsByStatusAsync(string status)
        {
            var list = new List<Complaint>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT ComplaintId, SubmittedByUserId, AgainstUserId, Subject, Description, " +
                "Status, AdminNotes, CreatedAt, ResolvedAt " +
                "FROM JPNS.Complaints WHERE Status = @Status ORDER BY CreatedAt DESC", connection);

            command.Parameters.AddWithValue("@Status", status);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                list.Add(MapComplaint(reader));

            return list;
        }

        public async Task<int> InsertComplaintAsync(Complaint complaint)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "INSERT INTO JPNS.Complaints (SubmittedByUserId, AgainstUserId, Subject, Description) " +
                "VALUES (@SubmittedByUserId, @AgainstUserId, @Subject, @Description); " +
                "SELECT SCOPE_IDENTITY();", connection);

            command.Parameters.AddWithValue("@SubmittedByUserId", complaint.SubmittedByUserId);
            command.Parameters.AddWithValue("@AgainstUserId", (object?)complaint.AgainstUserId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Subject", complaint.Subject);
            command.Parameters.AddWithValue("@Description", complaint.Description);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateComplaintStatusAsync(int complaintId, string status, string? adminNotes = null)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "UPDATE JPNS.Complaints SET Status = @Status, AdminNotes = @AdminNotes, " +
                "ResolvedAt = CASE WHEN @Status IN ('RESOLVED','CLOSED') THEN GETDATE() ELSE NULL END " +
                "WHERE ComplaintId = @ComplaintId", connection);

            command.Parameters.AddWithValue("@ComplaintId", complaintId);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@AdminNotes", (object?)adminNotes ?? DBNull.Value);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }


        public async Task<IEnumerable<Skill>> GetAllSkillsAsync()
        {
            var list = new List<Skill>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT SkillId, SkillName, Category FROM JPNS.Skills ORDER BY SkillName",
                connection);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Skill
                {
                    SkillId   = (int)reader["SkillId"],
                    SkillName = reader["SkillName"].ToString()!,
                    Category  = reader["Category"] == DBNull.Value ? null : reader["Category"].ToString()
                });
            }

            return list;
        }

        public async Task<int> InsertSkillAsync(string skillName, string? category)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "INSERT INTO JPNS.Skills (SkillName, Category) VALUES (@SkillName, @Category); " +
                "SELECT SCOPE_IDENTITY();", connection);

            command.Parameters.AddWithValue("@SkillName", skillName);
            command.Parameters.AddWithValue("@Category", (object?)category ?? DBNull.Value);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> DeleteSkillAsync(int skillId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "DELETE FROM JPNS.Skills WHERE SkillId = @SkillId", connection);

            command.Parameters.AddWithValue("@SkillId", skillId);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }


        private static CompanyProfile MapCompany(SqlDataReader reader) => new()
        {
            CompanyId   = (int)reader["CompanyId"],
            UserId      = (int)reader["UserId"],
            CompanyName = reader["CompanyName"].ToString()!,
            Industry    = reader["Industry"] == DBNull.Value ? null : reader["Industry"].ToString(),
            CompanySize = reader["CompanySize"] == DBNull.Value ? null : reader["CompanySize"].ToString(),
            Website     = reader["Website"] == DBNull.Value ? null : reader["Website"].ToString(),
            Location    = reader["Location"] == DBNull.Value ? null : reader["Location"].ToString(),
            Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
            LogoUrl     = reader["LogoUrl"] == DBNull.Value ? null : reader["LogoUrl"].ToString(),
            IsVerified  = (bool)reader["IsVerified"],
            CreatedAt   = (DateTime)reader["CreatedAt"],
            UpdatedAt   = (DateTime)reader["UpdatedAt"]
        };

        private static Complaint MapComplaint(SqlDataReader reader) => new()
        {
            ComplaintId        = (int)reader["ComplaintId"],
            SubmittedByUserId  = (int)reader["SubmittedByUserId"],
            AgainstUserId      = reader["AgainstUserId"] == DBNull.Value ? null : (int?)reader["AgainstUserId"],
            Subject            = reader["Subject"].ToString()!,
            Description        = reader["Description"].ToString()!,
            Status             = reader["Status"].ToString()!,
            AdminNotes         = reader["AdminNotes"] == DBNull.Value ? null : reader["AdminNotes"].ToString(),
            CreatedAt          = (DateTime)reader["CreatedAt"],
            ResolvedAt         = reader["ResolvedAt"] == DBNull.Value ? null : (DateTime?)reader["ResolvedAt"]
        };
    }
}
