using Microsoft.Data.SqlClient;
using System.Data;
using JobPortal.Features.JobPosting.Interface;
using JobPortal.Features.User.Candidate;
using JobPortal.Features.User.Candidate.Interface;
using JobPortal.Features.Application;
using JobPortal.Database;

namespace JobPortal.Features.User.Candidate.Repository
{
    public class JobSeekerRepository : IJobSeekerRepository
    {
        private readonly DatabaseConnection _db;

        public JobSeekerRepository(DatabaseConnection db)
        {
            _db = db;
        }


        public async Task<JobSeekerProfile?> GetProfileByUserIdAsync(int userId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT ProfileId, UserId, Headline, About, CurrentLocation, " +
                "ExperienceYears, LinkedInUrl, PortfolioUrl, UpdatedAt " +
                "FROM JPNS.JobSeekerProfiles WHERE UserId = @UserId", connection);

            command.Parameters.AddWithValue("@UserId", userId);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapProfile(reader);

            return null;
        }

        public async Task<int> InsertProfileAsync(JobSeekerProfile profile)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "INSERT INTO JPNS.JobSeekerProfiles " +
                "(UserId, Headline, About, CurrentLocation, ExperienceYears, LinkedInUrl, PortfolioUrl) " +
                "VALUES (@UserId, @Headline, @About, @CurrentLocation, @ExperienceYears, @LinkedInUrl, @PortfolioUrl); " +
                "SELECT SCOPE_IDENTITY();", connection);

            command.Parameters.AddWithValue("@UserId", profile.UserId);
            command.Parameters.AddWithValue("@Headline", (object?)profile.Headline ?? DBNull.Value);
            command.Parameters.AddWithValue("@About", (object?)profile.About ?? DBNull.Value);
            command.Parameters.AddWithValue("@CurrentLocation", (object?)profile.CurrentLocation ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExperienceYears", profile.ExperienceYears);
            command.Parameters.AddWithValue("@LinkedInUrl", (object?)profile.LinkedInUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@PortfolioUrl", (object?)profile.PortfolioUrl ?? DBNull.Value);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateProfileAsync(JobSeekerProfile profile)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "UPDATE JPNS.JobSeekerProfiles SET " +
                "Headline = @Headline, About = @About, CurrentLocation = @CurrentLocation, " +
                "ExperienceYears = @ExperienceYears, LinkedInUrl = @LinkedInUrl, " +
                "PortfolioUrl = @PortfolioUrl, UpdatedAt = GETDATE() " +
                "WHERE UserId = @UserId", connection);

            command.Parameters.AddWithValue("@UserId", profile.UserId);
            command.Parameters.AddWithValue("@Headline", (object?)profile.Headline ?? DBNull.Value);
            command.Parameters.AddWithValue("@About", (object?)profile.About ?? DBNull.Value);
            command.Parameters.AddWithValue("@CurrentLocation", (object?)profile.CurrentLocation ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExperienceYears", profile.ExperienceYears);
            command.Parameters.AddWithValue("@LinkedInUrl", (object?)profile.LinkedInUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@PortfolioUrl", (object?)profile.PortfolioUrl ?? DBNull.Value);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }


        public async Task<Resume?> GetResumeByIdAsync(int resumeId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT ResumeId, UserId, FileName, FileType, FileUrl, IsDefault, UploadedAt " +
                "FROM JPNS.Resumes WHERE ResumeId = @ResumeId", connection);

            command.Parameters.AddWithValue("@ResumeId", resumeId);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapResume(reader);

            return null;
        }

        public async Task<IEnumerable<Resume>> GetResumesByUserAsync(int userId)
        {
            var resumes = new List<Resume>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT ResumeId, UserId, FileName, FileType, FileUrl, IsDefault, UploadedAt " +
                "FROM JPNS.Resumes WHERE UserId = @UserId ORDER BY IsDefault DESC, UploadedAt DESC",
                connection);

            command.Parameters.AddWithValue("@UserId", userId);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                resumes.Add(MapResume(reader));

            return resumes;
        }

        public async Task<int> InsertResumeAsync(Resume resume)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "INSERT INTO JPNS.Resumes (UserId, FileName, FileType, FileUrl, IsDefault) " +
                "VALUES (@UserId, @FileName, @FileType, @FileUrl, @IsDefault); " +
                "SELECT SCOPE_IDENTITY();", connection);

            command.Parameters.AddWithValue("@UserId", resume.UserId);
            command.Parameters.AddWithValue("@FileName", resume.FileName);
            command.Parameters.AddWithValue("@FileType", resume.FileType);
            command.Parameters.AddWithValue("@FileUrl", resume.FileUrl);
            command.Parameters.AddWithValue("@IsDefault", resume.IsDefault);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateResumeAsync(Resume resume)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "UPDATE JPNS.Resumes SET FileName = @FileName, FileType = @FileType, " +
                "FileUrl = @FileUrl, IsDefault = @IsDefault WHERE ResumeId = @ResumeId",
                connection);

            command.Parameters.AddWithValue("@ResumeId", resume.ResumeId);
            command.Parameters.AddWithValue("@FileName", resume.FileName);
            command.Parameters.AddWithValue("@FileType", resume.FileType);
            command.Parameters.AddWithValue("@FileUrl", resume.FileUrl);
            command.Parameters.AddWithValue("@IsDefault", resume.IsDefault);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteResumeAsync(int resumeId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "DELETE FROM JPNS.Resumes WHERE ResumeId = @ResumeId", connection);

            command.Parameters.AddWithValue("@ResumeId", resumeId);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> SetDefaultResumeAsync(int userId, int resumeId)
        {
            await using var connection = await _db.OpenAsync();

            await using var clearCmd = new SqlCommand(
                "UPDATE JPNS.Resumes SET IsDefault = 0 WHERE UserId = @UserId", connection);
            clearCmd.Parameters.AddWithValue("@UserId", userId);
            await clearCmd.ExecuteNonQueryAsync();

            await using var setCmd = new SqlCommand(
                "UPDATE JPNS.Resumes SET IsDefault = 1 WHERE ResumeId = @ResumeId AND UserId = @UserId",
                connection);
            setCmd.Parameters.AddWithValue("@ResumeId", resumeId);
            setCmd.Parameters.AddWithValue("@UserId", userId);

            int rows = await setCmd.ExecuteNonQueryAsync();
            return rows > 0;
        }


        public async Task<Education?> GetEducationByIdAsync(int educationId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT EducationId, UserId, Degree, Institution, FieldOfStudy, " +
                "StartYear, EndYear, Grade, Description FROM JPNS.Education WHERE EducationId = @EducationId",
                connection);

            command.Parameters.AddWithValue("@EducationId", educationId);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapEducation(reader);

            return null;
        }

        public async Task<IEnumerable<Education>> GetEducationByUserAsync(int userId)
        {
            var list = new List<Education>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT EducationId, UserId, Degree, Institution, FieldOfStudy, " +
                "StartYear, EndYear, Grade, Description FROM JPNS.Education " +
                "WHERE UserId = @UserId ORDER BY EndYear DESC", connection);

            command.Parameters.AddWithValue("@UserId", userId);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                list.Add(MapEducation(reader));

            return list;
        }

        public async Task<int> InsertEducationAsync(Education education)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "INSERT INTO JPNS.Education (UserId, Degree, Institution, FieldOfStudy, StartYear, EndYear, Grade, Description) " +
                "VALUES (@UserId, @Degree, @Institution, @FieldOfStudy, @StartYear, @EndYear, @Grade, @Description); " +
                "SELECT SCOPE_IDENTITY();", connection);

            command.Parameters.AddWithValue("@UserId", education.UserId);
            command.Parameters.AddWithValue("@Degree", education.Degree);
            command.Parameters.AddWithValue("@Institution", education.Institution);
            command.Parameters.AddWithValue("@FieldOfStudy", (object?)education.FieldOfStudy ?? DBNull.Value);
            command.Parameters.AddWithValue("@StartYear", (object?)education.StartYear ?? DBNull.Value);
            command.Parameters.AddWithValue("@EndYear", (object?)education.EndYear ?? DBNull.Value);
            command.Parameters.AddWithValue("@Grade", (object?)education.Grade ?? DBNull.Value);
            command.Parameters.AddWithValue("@Description", (object?)education.Description ?? DBNull.Value);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateEducationAsync(Education education)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "UPDATE JPNS.Education SET Degree = @Degree, Institution = @Institution, " +
                "FieldOfStudy = @FieldOfStudy, StartYear = @StartYear, EndYear = @EndYear, " +
                "Grade = @Grade, Description = @Description WHERE EducationId = @EducationId",
                connection);

            command.Parameters.AddWithValue("@EducationId", education.EducationId);
            command.Parameters.AddWithValue("@Degree", education.Degree);
            command.Parameters.AddWithValue("@Institution", education.Institution);
            command.Parameters.AddWithValue("@FieldOfStudy", (object?)education.FieldOfStudy ?? DBNull.Value);
            command.Parameters.AddWithValue("@StartYear", (object?)education.StartYear ?? DBNull.Value);
            command.Parameters.AddWithValue("@EndYear", (object?)education.EndYear ?? DBNull.Value);
            command.Parameters.AddWithValue("@Grade", (object?)education.Grade ?? DBNull.Value);
            command.Parameters.AddWithValue("@Description", (object?)education.Description ?? DBNull.Value);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteEducationAsync(int educationId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "DELETE FROM JPNS.Education WHERE EducationId = @EducationId", connection);

            command.Parameters.AddWithValue("@EducationId", educationId);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }


        public async Task<WorkExperience?> GetExperienceByIdAsync(int experienceId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT ExperienceId, UserId, JobTitle, Company, Location, " +
                "StartDate, EndDate, IsCurrent, Description " +
                "FROM JPNS.WorkExperience WHERE ExperienceId = @ExperienceId", connection);

            command.Parameters.AddWithValue("@ExperienceId", experienceId);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapWorkExperience(reader);

            return null;
        }

        public async Task<IEnumerable<WorkExperience>> GetExperienceByUserAsync(int userId)
        {
            var list = new List<WorkExperience>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT ExperienceId, UserId, JobTitle, Company, Location, " +
                "StartDate, EndDate, IsCurrent, Description " +
                "FROM JPNS.WorkExperience WHERE UserId = @UserId ORDER BY IsCurrent DESC, StartDate DESC",
                connection);

            command.Parameters.AddWithValue("@UserId", userId);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                list.Add(MapWorkExperience(reader));

            return list;
        }

        public async Task<int> InsertExperienceAsync(WorkExperience experience)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "INSERT INTO JPNS.WorkExperience (UserId, JobTitle, Company, Location, StartDate, EndDate, IsCurrent, Description) " +
                "VALUES (@UserId, @JobTitle, @Company, @Location, @StartDate, @EndDate, @IsCurrent, @Description); " +
                "SELECT SCOPE_IDENTITY();", connection);

            command.Parameters.AddWithValue("@UserId", experience.UserId);
            command.Parameters.AddWithValue("@JobTitle", experience.JobTitle);
            command.Parameters.AddWithValue("@Company", experience.Company);
            command.Parameters.AddWithValue("@Location", (object?)experience.Location ?? DBNull.Value);
            command.Parameters.AddWithValue("@StartDate", experience.StartDate);
            command.Parameters.AddWithValue("@EndDate", (object?)experience.EndDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsCurrent", experience.IsCurrent);
            command.Parameters.AddWithValue("@Description", (object?)experience.Description ?? DBNull.Value);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateExperienceAsync(WorkExperience experience)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "UPDATE JPNS.WorkExperience SET JobTitle = @JobTitle, Company = @Company, " +
                "Location = @Location, StartDate = @StartDate, EndDate = @EndDate, " +
                "IsCurrent = @IsCurrent, Description = @Description " +
                "WHERE ExperienceId = @ExperienceId", connection);

            command.Parameters.AddWithValue("@ExperienceId", experience.ExperienceId);
            command.Parameters.AddWithValue("@JobTitle", experience.JobTitle);
            command.Parameters.AddWithValue("@Company", experience.Company);
            command.Parameters.AddWithValue("@Location", (object?)experience.Location ?? DBNull.Value);
            command.Parameters.AddWithValue("@StartDate", experience.StartDate);
            command.Parameters.AddWithValue("@EndDate", (object?)experience.EndDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsCurrent", experience.IsCurrent);
            command.Parameters.AddWithValue("@Description", (object?)experience.Description ?? DBNull.Value);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteExperienceAsync(int experienceId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "DELETE FROM JPNS.WorkExperience WHERE ExperienceId = @ExperienceId", connection);

            command.Parameters.AddWithValue("@ExperienceId", experienceId);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }


        public async Task<Project?> GetProjectByIdAsync(int projectId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT ProjectId, UserId, Title, Description, Technologies, ProjectUrl, StartDate, EndDate " +
                "FROM JPNS.Projects WHERE ProjectId = @ProjectId", connection);

            command.Parameters.AddWithValue("@ProjectId", projectId);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapProject(reader);

            return null;
        }

        public async Task<IEnumerable<Project>> GetProjectsByUserAsync(int userId)
        {
            var list = new List<Project>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT ProjectId, UserId, Title, Description, Technologies, ProjectUrl, StartDate, EndDate " +
                "FROM JPNS.Projects WHERE UserId = @UserId ORDER BY StartDate DESC", connection);

            command.Parameters.AddWithValue("@UserId", userId);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                list.Add(MapProject(reader));

            return list;
        }

        public async Task<int> InsertProjectAsync(Project project)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "INSERT INTO JPNS.Projects (UserId, Title, Description, Technologies, ProjectUrl, StartDate, EndDate) " +
                "VALUES (@UserId, @Title, @Description, @Technologies, @ProjectUrl, @StartDate, @EndDate); " +
                "SELECT SCOPE_IDENTITY();", connection);

            command.Parameters.AddWithValue("@UserId", project.UserId);
            command.Parameters.AddWithValue("@Title", project.Title);
            command.Parameters.AddWithValue("@Description", (object?)project.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Technologies", (object?)project.Technologies ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProjectUrl", (object?)project.ProjectUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@StartDate", (object?)project.StartDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@EndDate", (object?)project.EndDate ?? DBNull.Value);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateProjectAsync(Project project)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "UPDATE JPNS.Projects SET Title = @Title, Description = @Description, " +
                "Technologies = @Technologies, ProjectUrl = @ProjectUrl, " +
                "StartDate = @StartDate, EndDate = @EndDate WHERE ProjectId = @ProjectId", connection);

            command.Parameters.AddWithValue("@ProjectId", project.ProjectId);
            command.Parameters.AddWithValue("@Title", project.Title);
            command.Parameters.AddWithValue("@Description", (object?)project.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Technologies", (object?)project.Technologies ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProjectUrl", (object?)project.ProjectUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@StartDate", (object?)project.StartDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@EndDate", (object?)project.EndDate ?? DBNull.Value);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteProjectAsync(int projectId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "DELETE FROM JPNS.Projects WHERE ProjectId = @ProjectId", connection);

            command.Parameters.AddWithValue("@ProjectId", projectId);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }


        public async Task<Certificate?> GetCertificateByIdAsync(int certificateId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT CertificateId, UserId, CertName, IssuingOrg, IssueDate, ExpiryDate, " +
                "CertificateUrl, CredentialId FROM JPNS.Certificates WHERE CertificateId = @CertificateId",
                connection);

            command.Parameters.AddWithValue("@CertificateId", certificateId);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapCertificate(reader);

            return null;
        }

        public async Task<IEnumerable<Certificate>> GetCertificatesByUserAsync(int userId)
        {
            var list = new List<Certificate>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT CertificateId, UserId, CertName, IssuingOrg, IssueDate, ExpiryDate, " +
                "CertificateUrl, CredentialId FROM JPNS.Certificates " +
                "WHERE UserId = @UserId ORDER BY IssueDate DESC", connection);

            command.Parameters.AddWithValue("@UserId", userId);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                list.Add(MapCertificate(reader));

            return list;
        }

        public async Task<int> InsertCertificateAsync(Certificate certificate)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "INSERT INTO JPNS.Certificates (UserId, CertName, IssuingOrg, IssueDate, ExpiryDate, CertificateUrl, CredentialId) " +
                "VALUES (@UserId, @CertName, @IssuingOrg, @IssueDate, @ExpiryDate, @CertificateUrl, @CredentialId); " +
                "SELECT SCOPE_IDENTITY();", connection);

            command.Parameters.AddWithValue("@UserId", certificate.UserId);
            command.Parameters.AddWithValue("@CertName", certificate.CertName);
            command.Parameters.AddWithValue("@IssuingOrg", (object?)certificate.IssuingOrg ?? DBNull.Value);
            command.Parameters.AddWithValue("@IssueDate", (object?)certificate.IssueDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExpiryDate", (object?)certificate.ExpiryDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@CertificateUrl", (object?)certificate.CertificateUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@CredentialId", (object?)certificate.CredentialId ?? DBNull.Value);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateCertificateAsync(Certificate certificate)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "UPDATE JPNS.Certificates SET CertName = @CertName, IssuingOrg = @IssuingOrg, " +
                "IssueDate = @IssueDate, ExpiryDate = @ExpiryDate, " +
                "CertificateUrl = @CertificateUrl, CredentialId = @CredentialId " +
                "WHERE CertificateId = @CertificateId", connection);

            command.Parameters.AddWithValue("@CertificateId", certificate.CertificateId);
            command.Parameters.AddWithValue("@CertName", certificate.CertName);
            command.Parameters.AddWithValue("@IssuingOrg", (object?)certificate.IssuingOrg ?? DBNull.Value);
            command.Parameters.AddWithValue("@IssueDate", (object?)certificate.IssueDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExpiryDate", (object?)certificate.ExpiryDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@CertificateUrl", (object?)certificate.CertificateUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@CredentialId", (object?)certificate.CredentialId ?? DBNull.Value);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteCertificateAsync(int certificateId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "DELETE FROM JPNS.Certificates WHERE CertificateId = @CertificateId", connection);

            command.Parameters.AddWithValue("@CertificateId", certificateId);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }


        public async Task<IEnumerable<CandidateSkill>> GetSkillsByUserAsync(int userId)
        {
            var list = new List<CandidateSkill>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT cs.CandidateSkillId, cs.UserId, cs.SkillId, cs.ProficiencyLevel, " +
                "cs.YearsExperience, s.SkillName " +
                "FROM JPNS.CandidateSkills cs " +
                "JOIN JPNS.Skills s ON cs.SkillId = s.SkillId " +
                "WHERE cs.UserId = @UserId ORDER BY s.SkillName", connection);

            command.Parameters.AddWithValue("@UserId", userId);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new CandidateSkill
                {
                    CandidateSkillId = (int)reader["CandidateSkillId"],
                    UserId           = (int)reader["UserId"],
                    SkillId          = (int)reader["SkillId"],
                    SkillName        = reader["SkillName"].ToString()!,
                    ProficiencyLevel = reader["ProficiencyLevel"].ToString()!,
                    YearsExperience  = reader["YearsExperience"] == DBNull.Value ? null : (int?)reader["YearsExperience"]
                });
            }

            return list;
        }

        public async Task<int> AddSkillAsync(int userId, int skillId, string proficiencyLevel, int? yearsExperience)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "INSERT INTO JPNS.CandidateSkills (UserId, SkillId, ProficiencyLevel, YearsExperience) " +
                "VALUES (@UserId, @SkillId, @ProficiencyLevel, @YearsExperience); " +
                "SELECT SCOPE_IDENTITY();", connection);

            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@SkillId", skillId);
            command.Parameters.AddWithValue("@ProficiencyLevel", proficiencyLevel);
            command.Parameters.AddWithValue("@YearsExperience", (object?)yearsExperience ?? DBNull.Value);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateSkillAsync(int userId, int skillId, string proficiencyLevel, int? yearsExperience)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "UPDATE JPNS.CandidateSkills SET ProficiencyLevel = @ProficiencyLevel, " +
                "YearsExperience = @YearsExperience " +
                "WHERE UserId = @UserId AND SkillId = @SkillId", connection);

            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@SkillId", skillId);
            command.Parameters.AddWithValue("@ProficiencyLevel", proficiencyLevel);
            command.Parameters.AddWithValue("@YearsExperience", (object?)yearsExperience ?? DBNull.Value);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> RemoveSkillAsync(int userId, int skillId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "DELETE FROM JPNS.CandidateSkills WHERE UserId = @UserId AND SkillId = @SkillId",
                connection);

            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@SkillId", skillId);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }


        public async Task<int> ApplyForJobAsync(int userId, int jobId, int resumeId, string? coverLetter = null)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand("JPNS.usp_ApplyForJob", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@JobId", jobId);
            command.Parameters.AddWithValue("@ResumeId", resumeId);
            command.Parameters.AddWithValue("@CoverLetter", (object?)coverLetter ?? DBNull.Value);

            SqlParameter applicationIdParam = new SqlParameter("@ApplicationId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(applicationIdParam);

            try
            {
                await command.ExecuteNonQueryAsync();
                return (int)applicationIdParam.Value;
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error during ApplyForJob:");
                foreach (SqlError error in ex.Errors)
                    Console.WriteLine(error.Message);
                throw;
            }
        }

        public async Task<Application2?> GetApplicationByIdAsync(int applicationId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand(
                "SELECT a.ApplicationId, a.UserId, a.JobId, a.ResumeId, a.ResumeSnapshot, " +
                "a.CoverLetter, a.Status, a.AppliedDate, a.UpdatedAt, " +
                "j.Title AS JobTitle, c.CompanyName " +
                "FROM JPNS.Applications a " +
                "JOIN JPNS.JobListings j ON a.JobId = j.JobId " +
                "JOIN JPNS.CompanyProfiles c ON j.CompanyId = c.CompanyId " +
                "WHERE a.ApplicationId = @ApplicationId", connection);

            command.Parameters.AddWithValue("@ApplicationId", applicationId);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapApplication(reader);

            return null;
        }

        public async Task<IEnumerable<Application2>> GetApplicationsByUserAsync(int userId)
        {
            var list = new List<Application2>();

            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand("JPNS.usp_GetApplicationsByUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@UserId", userId);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Application2
                {
                    ApplicationId = reader["ApplicationId"].ToString()!,
                    JobId         = (int)reader["JobId"],
                    Status        = reader["Status"].ToString()!,
                    AppliedDate   = (DateTime)reader["AppliedDate"],
                    UpdatedAt     = (DateTime)reader["UpdatedAt"],
                    JobTitle      = reader["JobTitle"].ToString()!,
                    CompanyName   = reader["CompanyName"].ToString()!
                });
            }

            return list;
        }

        public async Task<bool> UpdateApplicationStatusAsync(int applicationId, string newStatus, int updatedByUserId)
        {
            await using var connection = await _db.OpenAsync();
            await using var command = new SqlCommand("JPNS.usp_UpdateApplicationStatus", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@ApplicationId", applicationId);
            command.Parameters.AddWithValue("@NewStatus", newStatus);
            command.Parameters.AddWithValue("@UpdatedByUserId", updatedByUserId);

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error during UpdateApplicationStatus:");
                foreach (SqlError error in ex.Errors)
                    Console.WriteLine(error.Message);
                throw;
            }
        }


        private static JobSeekerProfile MapProfile(SqlDataReader reader) => new()
        {
            ProfileId       = (int)reader["ProfileId"],
            UserId          = (int)reader["UserId"],
            Headline        = reader["Headline"] == DBNull.Value ? null : reader["Headline"].ToString(),
            About           = reader["About"] == DBNull.Value ? null : reader["About"].ToString(),
            CurrentLocation = reader["CurrentLocation"] == DBNull.Value ? null : reader["CurrentLocation"].ToString(),
            ExperienceYears = (int)reader["ExperienceYears"],
            LinkedInUrl     = reader["LinkedInUrl"] == DBNull.Value ? null : reader["LinkedInUrl"].ToString(),
            PortfolioUrl    = reader["PortfolioUrl"] == DBNull.Value ? null : reader["PortfolioUrl"].ToString(),
            UpdatedAt       = (DateTime)reader["UpdatedAt"]
        };

        private static Resume MapResume(SqlDataReader reader) => new()
        {
            ResumeId   = (int)reader["ResumeId"],
            UserId     = (int)reader["UserId"],
            FileName   = reader["FileName"].ToString()!,
            FileType   = reader["FileType"].ToString()!,
            FileUrl    = reader["FileUrl"].ToString()!,
            IsDefault  = (bool)reader["IsDefault"],
            UploadedAt = (DateTime)reader["UploadedAt"]
        };

        private static Education MapEducation(SqlDataReader reader) => new()
        {
            EducationId  = (int)reader["EducationId"],
            UserId       = (int)reader["UserId"],
            Degree       = reader["Degree"].ToString()!,
            Institution  = reader["Institution"].ToString()!,
            FieldOfStudy = reader["FieldOfStudy"] == DBNull.Value ? null : reader["FieldOfStudy"].ToString(),
            StartYear    = reader["StartYear"] == DBNull.Value ? null : (int?)reader["StartYear"],
            EndYear      = reader["EndYear"] == DBNull.Value ? null : (int?)reader["EndYear"],
            Grade        = reader["Grade"] == DBNull.Value ? null : reader["Grade"].ToString(),
            Description  = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString()
        };

        private static WorkExperience MapWorkExperience(SqlDataReader reader) => new()
        {
            ExperienceId = (int)reader["ExperienceId"],
            UserId       = (int)reader["UserId"],
            JobTitle     = reader["JobTitle"].ToString()!,
            Company      = reader["Company"].ToString()!,
            Location     = reader["Location"] == DBNull.Value ? null : reader["Location"].ToString(),
            StartDate    = (DateTime)reader["StartDate"],
            EndDate      = reader["EndDate"] == DBNull.Value ? null : (DateTime?)reader["EndDate"],
            IsCurrent    = (bool)reader["IsCurrent"],
            Description  = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString()
        };

        private static Project MapProject(SqlDataReader reader) => new()
        {
            ProjectId    = (int)reader["ProjectId"],
            UserId       = (int)reader["UserId"],
            Title        = reader["Title"].ToString()!,
            Description  = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
            Technologies = reader["Technologies"] == DBNull.Value ? null : reader["Technologies"].ToString(),
            ProjectUrl   = reader["ProjectUrl"] == DBNull.Value ? null : reader["ProjectUrl"].ToString(),
            StartDate    = reader["StartDate"] == DBNull.Value ? null : (DateTime?)reader["StartDate"],
            EndDate      = reader["EndDate"] == DBNull.Value ? null : (DateTime?)reader["EndDate"]
        };

        private static Certificate MapCertificate(SqlDataReader reader) => new()
        {
            CertificateId  = (int)reader["CertificateId"],
            UserId         = (int)reader["UserId"],
            CertName       = reader["CertName"].ToString()!,
            IssuingOrg     = reader["IssuingOrg"] == DBNull.Value ? null : reader["IssuingOrg"].ToString(),
            IssueDate      = reader["IssueDate"] == DBNull.Value ? null : (DateTime?)reader["IssueDate"],
            ExpiryDate     = reader["ExpiryDate"] == DBNull.Value ? null : (DateTime?)reader["ExpiryDate"],
            CertificateUrl = reader["CertificateUrl"] == DBNull.Value ? null : reader["CertificateUrl"].ToString(),
            CredentialId   = reader["CredentialId"] == DBNull.Value ? null : reader["CredentialId"].ToString()
        };

        private static Application2 MapApplication(SqlDataReader reader) => new()
        {
            ApplicationId  = reader["ApplicationId"].ToString()!,
            UserId         = (int)reader["UserId"],
            JobId          = (int)reader["JobId"],
            ResumeId       = (int)reader["ResumeId"],
            ResumeSnapshot = reader["ResumeSnapshot"] == DBNull.Value ? null : reader["ResumeSnapshot"].ToString(),
            CoverLetter    = reader["CoverLetter"] == DBNull.Value ? null : reader["CoverLetter"].ToString(),
            Status         = reader["Status"].ToString()!,
            AppliedDate    = (DateTime)reader["AppliedDate"],
            UpdatedAt      = (DateTime)reader["UpdatedAt"],
            JobTitle       = reader["JobTitle"] == DBNull.Value ? null : reader["JobTitle"].ToString(),
            CompanyName    = reader["CompanyName"] == DBNull.Value ? null : reader["CompanyName"].ToString()
        };
    }
}
