using Microsoft.Data.SqlClient;
using System.Data;
using JobPortal.Features.JobPosting;
using JobPortal.Database;
using JobPortal.Features.JobPosting.Interface;
using JobPortal.Shared.Structs;
using JobPortal.Shared.Enums;

namespace JobPortal.Features.JobPosting.Repository
{
    public class JobRepository : IJobRepository
    {
        private readonly DatabaseConnection _db;

        public JobRepository(DatabaseConnection db)
        {
            _db = db;
        }

        private static JobStatus ParseJobStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status)) return JobStatus.PENDING_APPROVAL;
            return status.ToLower() switch
            {
                "pending" => JobStatus.PENDING_APPROVAL,
                "pending_approval" => JobStatus.PENDING_APPROVAL,
                "approved" => JobStatus.APPROVED,
                "active" => JobStatus.APPROVED,
                "rejected" => JobStatus.REJECTED,
                "closed" => JobStatus.CLOSED,
                "expired" => JobStatus.EXPIRED,
                _ => JobStatus.PENDING_APPROVAL
            };
        }


        public async Task<JobListing?> GetByIdAsync(int id)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT j.JobId, j.PostedByUserId, j.CompanyId, j.Title, j.Description, " +
                "j.Location, j.JobType, j.SalaryMin, j.SalaryMax, j.ExperienceRequired, " +
                "j.Status, j.TotalApplications, j.ExpiryDate, j.PostedDate, j.UpdatedAt, j.NumberOfRounds, " +
                "c.CompanyName " +
                "FROM JPNS.JobListings j " +
                "JOIN JPNS.CompanyProfiles c ON j.CompanyId = c.CompanyId " +
                "WHERE j.JobId = @JobId", connection);

            command.Parameters.AddWithValue("@JobId", id);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapJobListing(reader);

            return null;
        }

        public async Task<IEnumerable<JobListing>> GetAllAsync()
        {
            var jobs = new List<JobListing>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT j.JobId, j.PostedByUserId, j.CompanyId, j.Title, j.Description, " +
                "j.Location, j.JobType, j.SalaryMin, j.SalaryMax, j.ExperienceRequired, " +
                "j.Status, j.TotalApplications, j.ExpiryDate, j.PostedDate, j.UpdatedAt, j.NumberOfRounds, " +
                "c.CompanyName " +
                "FROM JPNS.JobListings j " +
                "JOIN JPNS.CompanyProfiles c ON j.CompanyId = c.CompanyId " +
                "ORDER BY j.PostedDate DESC", connection);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                jobs.Add(MapJobListing(reader));

            return jobs;
        }

        public async Task<int> InsertAsync(JobListing entity)
        {
            return await PostJobAsync(
                entity.PostedByUserId, entity.CompanyId, entity.Title,
                entity.Description, entity.Location, entity.JobType,
                entity.SalaryMin, entity.SalaryMax,
                entity.ExperienceRequired, entity.ExpiryDate, entity.NumberOfRounds);
        }

        public async Task<bool> UpdateAsync(JobListing entity)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "UPDATE JPNS.JobListings SET Title = @Title, Description = @Description, " +
                "Location = @Location, JobType = @JobType, SalaryMin = @SalaryMin, " +
                "SalaryMax = @SalaryMax, ExperienceRequired = @ExperienceRequired, " +
                "ExpiryDate = @ExpiryDate, NumberOfRounds = @NumberOfRounds, UpdatedAt = GETDATE() " +
                "WHERE JobId = @JobId", connection);

            command.Parameters.AddWithValue("@JobId", entity.JobId);
            command.Parameters.AddWithValue("@Title", entity.Title);
            command.Parameters.AddWithValue("@Description", entity.Description);
            command.Parameters.AddWithValue("@Location", entity.Location);
            command.Parameters.AddWithValue("@JobType", entity.JobType);
            command.Parameters.AddWithValue("@SalaryMin", (object?)entity.SalaryMin ?? DBNull.Value);
            command.Parameters.AddWithValue("@SalaryMax", (object?)entity.SalaryMax ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExperienceRequired", entity.ExperienceRequired);
            command.Parameters.AddWithValue("@ExpiryDate", entity.ExpiryDate);
            command.Parameters.AddWithValue("@NumberOfRounds", entity.NumberOfRounds);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "DELETE FROM JPNS.JobListings WHERE JobId = @JobId", connection);

            command.Parameters.AddWithValue("@JobId", id);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }


        public async Task<int> PostJobAsync(int postedByUserId, int companyId, string title,
                                             string description, string location, string jobType,
                                             decimal? salaryMin, decimal? salaryMax,
                                             int experienceRequired, DateTime expiryDate, int numberOfRounds = 3)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand("JPNS.usp_PostJob", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@PostedByUserId", postedByUserId);
            command.Parameters.AddWithValue("@CompanyId", companyId);
            command.Parameters.AddWithValue("@Title", title);
            command.Parameters.AddWithValue("@Description", description);
            command.Parameters.AddWithValue("@Location", location);
            command.Parameters.AddWithValue("@JobType", jobType);
            command.Parameters.AddWithValue("@SalaryMin", (object?)salaryMin ?? DBNull.Value);
            command.Parameters.AddWithValue("@SalaryMax", (object?)salaryMax ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExperienceRequired", experienceRequired);
            command.Parameters.AddWithValue("@ExpiryDate", expiryDate);
            command.Parameters.AddWithValue("@NumberOfRounds", numberOfRounds);

            SqlParameter jobIdParam = new SqlParameter("@JobId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(jobIdParam);

            try
            {
                await command.ExecuteNonQueryAsync();
                return (int)jobIdParam.Value;
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error during PostJob:");
                foreach (SqlError error in ex.Errors)
                    Console.WriteLine(error.Message);
                throw;
            }
        }

        public async Task<IEnumerable<JobListing>> SearchJobsAsync(string? keyword = null,
                                                                     decimal? salaryMin = null,
                                                                     decimal? salaryMax = null)
        {
            var jobs = new List<JobListing>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand("JPNS.usp_SearchJobs", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Keyword", (object?)keyword ?? DBNull.Value);
            command.Parameters.AddWithValue("@SalaryMin", (object?)salaryMin ?? DBNull.Value);
            command.Parameters.AddWithValue("@SalaryMax", (object?)salaryMax ?? DBNull.Value);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var job = new JobListing
                {
                    JobId               = reader["JobId"].ToString()!,
                    Title               = reader["Title"].ToString()!,
                    Location            = reader["Location"].ToString()!,
                    JobType             = reader["JobType"].ToString()!,
                    SalaryMin           = reader["SalaryMin"] == DBNull.Value ? null : (decimal?)reader["SalaryMin"],
                    SalaryMax           = reader["SalaryMax"] == DBNull.Value ? null : (decimal?)reader["SalaryMax"],
                    ExperienceRequired  = (int)reader["ExperienceRequired"],
                    ExpiryDate          = (DateTime)reader["ExpiryDate"],
                    TotalApplications   = (int)reader["TotalApplications"],
                    Status              = ParseJobStatus(reader["Status"].ToString()!),
                    CompanyName         = reader["CompanyName"].ToString()!,
                    NumberOfRounds      = reader["NumberOfRounds"] == DBNull.Value ? 3 : (int)reader["NumberOfRounds"]
                };
                decimal min = job.SalaryMin ?? 1;
                decimal max = job.SalaryMax ?? 1;
                if (min <= 0) min = 1;
                if (max < min) max = min;
                job.SalaryRange = new SalaryRange(min, max);
                jobs.Add(job);
            }

            return jobs;
        }

        public async Task<IEnumerable<JobListing>> GetPendingJobsAsync()
        {
            var jobs = new List<JobListing>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand("JPNS.usp_GetPendingJobs", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var job = new JobListing
                {
                    JobId       = reader["JobId"].ToString()!,
                    Title       = reader["Title"].ToString()!,
                    Location    = reader["Location"].ToString()!,
                    JobType     = reader["JobType"].ToString()!,
                    SalaryMin   = reader["SalaryMin"] == DBNull.Value ? null : (decimal?)reader["SalaryMin"],
                    SalaryMax   = reader["SalaryMax"] == DBNull.Value ? null : (decimal?)reader["SalaryMax"],
                    ExpiryDate  = (DateTime)reader["ExpiryDate"],
                    PostedDate  = (DateTime)reader["PostedDate"],
                    CompanyName = reader["CompanyName"].ToString()!,
                    Status      = JobStatus.PENDING_APPROVAL,
                    NumberOfRounds = reader["NumberOfRounds"] == DBNull.Value ? 3 : (int)reader["NumberOfRounds"]
                };
                decimal min = job.SalaryMin ?? 1;
                decimal max = job.SalaryMax ?? 1;
                if (min <= 0) min = 1;
                if (max < min) max = min;
                job.SalaryRange = new SalaryRange(min, max);
                jobs.Add(job);
            }

            return jobs;
        }

        public async Task<IEnumerable<JobListing>> GetByCompanyAsync(int companyId)
        {
            var jobs = new List<JobListing>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT j.JobId, j.PostedByUserId, j.CompanyId, j.Title, j.Description, " +
                "j.Location, j.JobType, j.SalaryMin, j.SalaryMax, j.ExperienceRequired, " +
                "j.Status, j.TotalApplications, j.ExpiryDate, j.PostedDate, j.UpdatedAt, j.NumberOfRounds, " +
                "c.CompanyName " +
                "FROM JPNS.JobListings j " +
                "JOIN JPNS.CompanyProfiles c ON j.CompanyId = c.CompanyId " +
                "WHERE j.CompanyId = @CompanyId " +
                "ORDER BY j.PostedDate DESC", connection);

            command.Parameters.AddWithValue("@CompanyId", companyId);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                jobs.Add(MapJobListing(reader));

            return jobs;
        }

        public async Task<IEnumerable<JobListing>> GetByStatusAsync(string status)
        {
            var jobs = new List<JobListing>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT j.JobId, j.PostedByUserId, j.CompanyId, j.Title, j.Description, " +
                "j.Location, j.JobType, j.SalaryMin, j.SalaryMax, j.ExperienceRequired, " +
                "j.Status, j.TotalApplications, j.ExpiryDate, j.PostedDate, j.UpdatedAt, j.NumberOfRounds, " +
                "c.CompanyName " +
                "FROM JPNS.JobListings j " +
                "JOIN JPNS.CompanyProfiles c ON j.CompanyId = c.CompanyId " +
                "WHERE j.Status = @Status " +
                "ORDER BY j.PostedDate DESC", connection);

            command.Parameters.AddWithValue("@Status", status);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                jobs.Add(MapJobListing(reader));

            return jobs;
        }


        public async Task<int> AddJobSkillAsync(int jobId, int skillId, bool isRequired = true)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "INSERT INTO JPNS.JobSkills (JobId, SkillId, IsRequired) " +
                "VALUES (@JobId, @SkillId, @IsRequired); " +
                "SELECT SCOPE_IDENTITY();", connection);

            command.Parameters.AddWithValue("@JobId", jobId);
            command.Parameters.AddWithValue("@SkillId", skillId);
            command.Parameters.AddWithValue("@IsRequired", isRequired);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> RemoveJobSkillAsync(int jobId, int skillId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "DELETE FROM JPNS.JobSkills WHERE JobId = @JobId AND SkillId = @SkillId", connection);

            command.Parameters.AddWithValue("@JobId", jobId);
            command.Parameters.AddWithValue("@SkillId", skillId);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<IEnumerable<JobSkill>> GetJobSkillsAsync(int jobId)
        {
            var skills = new List<JobSkill>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT js.JobSkillId, js.JobId, js.SkillId, js.IsRequired, s.SkillName " +
                "FROM JPNS.JobSkills js " +
                "JOIN JPNS.Skills s ON js.SkillId = s.SkillId " +
                "WHERE js.JobId = @JobId", connection);

            command.Parameters.AddWithValue("@JobId", jobId);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                skills.Add(new JobSkill
                {
                    JobSkillId  = (int)reader["JobSkillId"],
                    JobId       = (int)reader["JobId"],
                    SkillId     = (int)reader["SkillId"],
                    SkillName   = reader["SkillName"].ToString()!,
                    IsRequired  = (bool)reader["IsRequired"]
                });
            }

            return skills;
        }


        public async Task<int> SaveJobAsync(int userId, int jobId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "INSERT INTO JPNS.SavedJobs (UserId, JobId) VALUES (@UserId, @JobId); " +
                "SELECT SCOPE_IDENTITY();", connection);

            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@JobId", jobId);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UnsaveJobAsync(int userId, int jobId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "DELETE FROM JPNS.SavedJobs WHERE UserId = @UserId AND JobId = @JobId", connection);

            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@JobId", jobId);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<IEnumerable<SavedJob>> GetSavedJobsByUserAsync(int userId)
        {
            var saved = new List<SavedJob>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT sj.SavedJobId, sj.UserId, sj.JobId, sj.SavedAt, " +
                "j.Title AS JobTitle, c.CompanyName " +
                "FROM JPNS.SavedJobs sj " +
                "JOIN JPNS.JobListings j ON sj.JobId = j.JobId " +
                "JOIN JPNS.CompanyProfiles c ON j.CompanyId = c.CompanyId " +
                "WHERE sj.UserId = @UserId " +
                "ORDER BY sj.SavedAt DESC", connection);

            command.Parameters.AddWithValue("@UserId", userId);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                saved.Add(new SavedJob
                {
                    SavedJobId  = (int)reader["SavedJobId"],
                    UserId      = (int)reader["UserId"],
                    JobId       = (int)reader["JobId"],
                    JobTitle    = reader["JobTitle"].ToString()!,
                    CompanyName = reader["CompanyName"].ToString()!,
                    SavedAt     = (DateTime)reader["SavedAt"]
                });
            }

            return saved;
        }

        public async Task<bool> IsJobSavedAsync(int userId, int jobId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT COUNT(1) FROM JPNS.SavedJobs WHERE UserId = @UserId AND JobId = @JobId",
                connection);

            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@JobId", jobId);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }


        private static JobListing MapJobListing(SqlDataReader reader)
        {
            var job = new JobListing
            {
                JobId               = reader["JobId"].ToString()!,
                PostedByUserId      = (int)reader["PostedByUserId"],
                CompanyName         = reader["CompanyName"].ToString()!,
                Title               = reader["Title"].ToString()!,
                Description         = reader["Description"].ToString()!,
                Location            = reader["Location"].ToString()!,
                JobType             = reader["JobType"].ToString()!,
                SalaryMin           = reader["SalaryMin"] == DBNull.Value ? null : (decimal?)reader["SalaryMin"],
                SalaryMax           = reader["SalaryMax"] == DBNull.Value ? null : (decimal?)reader["SalaryMax"],
                ExperienceRequired  = (int)reader["ExperienceRequired"],
                Status              = ParseJobStatus(reader["Status"].ToString()!),
                TotalApplications   = (int)reader["TotalApplications"],
                ExpiryDate          = (DateTime)reader["ExpiryDate"],
                PostedDate          = (DateTime)reader["PostedDate"],
                UpdatedAt           = reader["UpdatedAt"] == DBNull.Value ? null : (DateTime?)reader["UpdatedAt"],
                NumberOfRounds      = reader["NumberOfRounds"] == DBNull.Value ? 3 : (int)reader["NumberOfRounds"]
            };
            decimal min = job.SalaryMin ?? 1;
            decimal max = job.SalaryMax ?? 1;
            if (min <= 0) min = 1;
            if (max < min) max = min;
            job.SalaryRange = new SalaryRange(min, max);
            return job;
        }
    }
}
