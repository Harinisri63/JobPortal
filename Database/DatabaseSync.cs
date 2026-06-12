using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;
using JobPortal.Features.Admin;
using JobPortal.Features.Candidate;
using JobPortal.Features.JobPosting;
using JobPortal.Features.Application;
using JobPortal.Features.Interview;
using JobPortal.Features.Notifications;
using JobPortal.Shared.Enums;
using JobPortal.Shared.Structs;
using JobPortal.Features.User.Candidate;

namespace JobPortal.Database;
internal static class DatabaseSync
{
    private static string? _connectionString;
    private static bool _enabled = false;

    public static bool IsEnabled => _enabled;


    public static void Initialize()
    {
        try
        {
            string settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (!File.Exists(settingsPath))
                settingsPath = "appsettings.json";

            if (!File.Exists(settingsPath))
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  [DB] appsettings.json not found — running in file-only mode.");
                Console.ResetColor();
                return;
            }

            string json = File.ReadAllText(settingsPath);
            using JsonDocument doc = JsonDocument.Parse(json);
            string? cs = doc.RootElement
                .GetProperty("ConnectionStrings")
                .GetProperty("DefaultConnection")
                .GetString();

            if (string.IsNullOrWhiteSpace(cs))
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  [DB] Connection string is empty — running in file-only mode.");
                Console.ResetColor();
                return;
            }

            try
            {
                using var testConn = new SqlConnection(cs);
                testConn.Open();

                using var schemaCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'JPNS'",
                    testConn);
                int schemaCount = (int)schemaCmd.ExecuteScalar()!;

                if (schemaCount == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("  [DB] ERROR: 'JPNS' schema not found in the database.");
                    Console.WriteLine("  [DB] Please run JPNS/SQL/Database.sql in SSMS first.");
                    Console.WriteLine("  [DB] Running in file-only mode.");
                    Console.ResetColor();
                    return;
                }

                using var tableCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
                    "WHERE TABLE_SCHEMA = 'JPNS' AND TABLE_NAME IN " +
                    "('Users','JobSeekerProfiles','CompanyProfiles','JobListings','Applications','Resumes')",
                    testConn);
                int tableCount = (int)tableCmd.ExecuteScalar()!;

                if (tableCount < 6)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  [DB] ERROR: Only {tableCount}/6 required tables found.");
                    Console.WriteLine("  [DB] Please run JPNS/SQL/Database.sql in SSMS first.");
                    Console.WriteLine("  [DB] Running in file-only mode.");
                    Console.ResetColor();
                    return;
                }

                // Run Migrations and Deploy Stored Procedures
                using (var migrationCmd = new SqlCommand(@"
                    IF NOT EXISTS (
                        SELECT 1 FROM sys.columns 
                        WHERE object_id = OBJECT_ID('JPNS.JobListings') AND name = 'NumberOfRounds'
                    )
                    BEGIN
                        ALTER TABLE JPNS.JobListings ADD NumberOfRounds INT NOT NULL CONSTRAINT DF_JobListings_NumberOfRounds DEFAULT 3
                    END", testConn))
                {
                    migrationCmd.ExecuteNonQuery();
                }

                using (var roundsMigrationCmd = new SqlCommand(@"
                    IF NOT EXISTS (
                        SELECT 1 FROM sys.columns 
                        WHERE object_id = OBJECT_ID('JPNS.JobSeekerProfiles') AND name = 'ClearedRounds'
                    )
                    BEGIN
                        ALTER TABLE JPNS.JobSeekerProfiles ADD ClearedRounds INT NOT NULL CONSTRAINT DF_JobSeekerProfiles_ClearedRounds DEFAULT 0
                    END
                    IF NOT EXISTS (
                        SELECT 1 FROM sys.columns 
                        WHERE object_id = OBJECT_ID('JPNS.JobSeekerProfiles') AND name = 'TotalRounds'
                    )
                    BEGIN
                        ALTER TABLE JPNS.JobSeekerProfiles ADD TotalRounds INT NOT NULL CONSTRAINT DF_JobSeekerProfiles_TotalRounds DEFAULT 3
                    END
                    IF NOT EXISTS (
                        SELECT 1 FROM sys.columns 
                        WHERE object_id = OBJECT_ID('JPNS.JobSeekerProfiles') AND name = 'CurrentRound'
                    )
                    BEGIN
                        ALTER TABLE JPNS.JobSeekerProfiles ADD CurrentRound INT NOT NULL CONSTRAINT DF_JobSeekerProfiles_CurrentRound DEFAULT 0
                    END
                    IF NOT EXISTS (
                        SELECT 1 FROM sys.columns 
                        WHERE object_id = OBJECT_ID('JPNS.JobSeekerProfiles') AND name = 'RejectedRound'
                    )
                    BEGIN
                        ALTER TABLE JPNS.JobSeekerProfiles ADD RejectedRound INT NULL
                    END
                    IF NOT EXISTS (
                        SELECT 1 FROM sys.columns 
                        WHERE object_id = OBJECT_ID('JPNS.JobSeekerProfiles') AND name = 'CandidateStatus'
                    )
                    BEGIN
                        ALTER TABLE JPNS.JobSeekerProfiles ADD CandidateStatus NVARCHAR(50) NULL
                    END", testConn))
                {
                    roundsMigrationCmd.ExecuteNonQuery();
                }

                using (var compTableCmd = new SqlCommand(@"
                    IF OBJECT_ID('JPNS.Complaints', 'U') IS NULL
                    BEGIN
                        CREATE TABLE JPNS.Complaints (
                            ComplaintId       INT             IDENTITY(1,1) NOT NULL,
                            SubmittedByUserId INT             NOT NULL,
                            AgainstUserId     INT             NULL,
                            Subject           NVARCHAR(200)   NOT NULL,
                            Description       NVARCHAR(MAX)   NOT NULL,
                            Status            NVARCHAR(20)    NOT NULL CONSTRAINT DF_Comp_Status DEFAULT 'OPEN'
                                              CONSTRAINT CHK_Comp_Status CHECK (Status IN ('OPEN','IN_REVIEW','RESOLVED','CLOSED')),
                            AdminNotes        NVARCHAR(MAX)   NULL,
                            CreatedAt         DATETIME2       NOT NULL CONSTRAINT DF_Comp_CA DEFAULT GETDATE(),
                            ResolvedAt        DATETIME2       NULL,
                            CONSTRAINT PK_Complaints   PRIMARY KEY (ComplaintId),
                            CONSTRAINT FK_Comp_SubmBy  FOREIGN KEY (SubmittedByUserId) REFERENCES JPNS.Users(UserId),
                            CONSTRAINT FK_Comp_Against FOREIGN KEY (AgainstUserId)     REFERENCES JPNS.Users(UserId)
                        );
                    END", testConn))
                {
                    compTableCmd.ExecuteNonQuery();
                }

                string raiseComplaintSp = @"
                    CREATE OR ALTER PROCEDURE JPNS.usp_RaiseComplaint
                        @SubmittedByUserId INT,
                        @AgainstUserId INT,
                        @Subject NVARCHAR(200),
                        @Description NVARCHAR(MAX),
                        @ComplaintId INT OUTPUT
                    AS
                    BEGIN
                        INSERT INTO JPNS.Complaints (SubmittedByUserId, AgainstUserId, Subject, Description, Status, CreatedAt)
                        VALUES (@SubmittedByUserId, @AgainstUserId, @Subject, @Description, 'OPEN', GETDATE());
                        SET @ComplaintId = SCOPE_IDENTITY();
                    END";
                using (var spCmd1 = new SqlCommand(raiseComplaintSp, testConn)) { spCmd1.ExecuteNonQuery(); }

                string resolveComplaintSp = @"
                    CREATE OR ALTER PROCEDURE JPNS.usp_ResolveComplaint
                        @ComplaintId INT,
                        @AdminNotes NVARCHAR(MAX)
                    AS
                    BEGIN
                        UPDATE JPNS.Complaints
                        SET Status = 'RESOLVED',
                            AdminNotes = @AdminNotes,
                            ResolvedAt = GETDATE()
                        WHERE ComplaintId = @ComplaintId;
                    END";
                using (var spCmd2 = new SqlCommand(resolveComplaintSp, testConn)) { spCmd2.ExecuteNonQuery(); }

                string getComplaintsByUserSp = @"
                    CREATE OR ALTER PROCEDURE JPNS.usp_GetComplaintsByUser
                        @UserId INT
                    AS
                    BEGIN
                        SELECT ComplaintId, SubmittedByUserId, AgainstUserId, Subject, Description, Status, AdminNotes, CreatedAt, ResolvedAt
                        FROM JPNS.Complaints
                        WHERE SubmittedByUserId = @UserId
                        ORDER BY CreatedAt DESC;
                    END";
                using (var spCmd3 = new SqlCommand(getComplaintsByUserSp, testConn)) { spCmd3.ExecuteNonQuery(); }

                string getAllComplaintsSp = @"
                    CREATE OR ALTER PROCEDURE JPNS.usp_GetAllComplaints
                    AS
                    BEGIN
                        SELECT ComplaintId, SubmittedByUserId, AgainstUserId, Subject, Description, Status, AdminNotes, CreatedAt, ResolvedAt
                        FROM JPNS.Complaints
                        ORDER BY CreatedAt DESC;
                    END";
                using (var spCmd4 = new SqlCommand(getAllComplaintsSp, testConn)) { spCmd4.ExecuteNonQuery(); }

                string updateInterviewStatusSp = @"
                    CREATE OR ALTER PROCEDURE JPNS.usp_UpdateInterviewStatus
                        @InterviewId INT,
                        @Status NVARCHAR(20),
                        @Feedback NVARCHAR(MAX) = NULL
                    AS
                    BEGIN
                        UPDATE JPNS.Interviews
                        SET Status = @Status,
                            Feedback = @Feedback,
                            UpdatedAt = GETDATE()
                        WHERE InterviewId = @InterviewId;
                    END";
                using (var spCmd5 = new SqlCommand(updateInterviewStatusSp, testConn)) { spCmd5.ExecuteNonQuery(); }

                string postJobSp = @"
                    CREATE OR ALTER PROCEDURE JPNS.usp_PostJob
                        @PostedByUserId     INT,
                        @CompanyId          INT,
                        @Title              NVARCHAR(200),
                        @Description        NVARCHAR(MAX),
                        @Location           NVARCHAR(150),
                        @JobType            NVARCHAR(30),
                        @SalaryMin          DECIMAL(12,2) = NULL,
                        @SalaryMax          DECIMAL(12,2) = NULL,
                        @ExperienceRequired INT = 0,
                        @ExpiryDate         DATETIME2,
                        @NumberOfRounds     INT = 3,
                        @JobId              INT OUTPUT
                    AS BEGIN
                        SET NOCOUNT ON;
                        BEGIN TRY
                            IF NOT EXISTS (SELECT 1 FROM JPNS.Users WHERE UserId = @PostedByUserId AND Role = 'admin')
                                RAISERROR('Only admin accounts may post jobs.', 16, 1);

                            IF NOT EXISTS (SELECT 1 FROM JPNS.CompanyProfiles WHERE CompanyId = @CompanyId AND IsVerified = 1)
                                RAISERROR('Company profile not found or not verified.', 16, 1);

                            IF @ExpiryDate <= GETDATE()
                                RAISERROR('Expiry date must be a future date.', 16, 1);

                            IF @SalaryMax IS NOT NULL AND @SalaryMax < @SalaryMin
                                RAISERROR('Salary Max must be >= Salary Min.', 16, 1);

                            BEGIN TRANSACTION;
                                INSERT INTO JPNS.JobListings
                                    (PostedByUserId, CompanyId, Title, Description, Location, JobType,
                                     SalaryMin, SalaryMax, ExperienceRequired, ExpiryDate, NumberOfRounds)
                                VALUES
                                    (@PostedByUserId, @CompanyId, @Title, @Description, @Location, @JobType,
                                     @SalaryMin, @SalaryMax, @ExperienceRequired, @ExpiryDate, @NumberOfRounds);

                                SET @JobId = SCOPE_IDENTITY();
                            COMMIT TRANSACTION;

                            SELECT @JobId AS JobId, 'pending' AS Status;
                        END TRY
                        BEGIN CATCH
                            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
                            THROW;
                        END CATCH
                    END";
                using (var spCmd6 = new SqlCommand(postJobSp, testConn)) { spCmd6.ExecuteNonQuery(); }

                string searchJobsSp = @"
                    CREATE OR ALTER PROCEDURE JPNS.usp_SearchJobs
                        @Keyword   NVARCHAR(200) = NULL,
                        @SalaryMin DECIMAL(12,2) = NULL,
                        @SalaryMax DECIMAL(12,2) = NULL
                    AS BEGIN
                        SET NOCOUNT ON;
                        SELECT j.JobId, j.Title, j.Location, j.JobType, j.SalaryMin, j.SalaryMax,
                               j.ExperienceRequired, j.ExpiryDate, j.TotalApplications, j.Status,
                               j.NumberOfRounds, c.CompanyName
                        FROM JPNS.JobListings j
                        JOIN JPNS.CompanyProfiles c ON j.CompanyId = c.CompanyId
                        WHERE j.Status = 'approved' AND j.ExpiryDate > GETDATE()
                          AND (@Keyword IS NULL OR j.Title LIKE '%' + @Keyword + '%'
                                                OR j.Description LIKE '%' + @Keyword + '%')
                          AND (@SalaryMin IS NULL OR j.SalaryMax >= @SalaryMin)
                          AND (@SalaryMax IS NULL OR j.SalaryMin <= @SalaryMax)
                        ORDER BY j.PostedDate DESC;
                    END";
                using (var spCmd7 = new SqlCommand(searchJobsSp, testConn)) { spCmd7.ExecuteNonQuery(); }

                string getPendingJobsSp = @"
                    CREATE OR ALTER PROCEDURE JPNS.usp_GetPendingJobs AS BEGIN
                        SET NOCOUNT ON;
                        SELECT j.JobId, j.Title, j.Location, j.JobType, j.SalaryMin, j.SalaryMax,
                               j.ExpiryDate, j.PostedDate, j.NumberOfRounds, c.CompanyName
                        FROM JPNS.JobListings j
                        JOIN JPNS.CompanyProfiles c ON j.CompanyId = c.CompanyId
                        WHERE j.Status = 'pending'
                        ORDER BY j.PostedDate ASC;
                    END";
                using (var spCmd8 = new SqlCommand(getPendingJobsSp, testConn)) { spCmd8.ExecuteNonQuery(); }

                // Clean up duplicate job listings in the database using the open testConn
                try
                {
                    using var cleanupCmd = new SqlCommand(@"
                        WITH CTE AS (
                            SELECT JobId,
                                   ROW_NUMBER() OVER (PARTITION BY PostedByUserId, Title ORDER BY JobId) AS rn
                            FROM JPNS.JobListings
                        )
                        DELETE FROM JPNS.JobListings
                        WHERE JobId IN (SELECT JobId FROM CTE WHERE rn > 1)
                          AND JobId NOT IN (SELECT DISTINCT JobId FROM JPNS.Applications)
                          AND JobId NOT IN (SELECT DISTINCT JobId FROM JPNS.SavedJobs);", testConn);
                    cleanupCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  [DB CLEANUP ERROR] Failed to clean up duplicate jobs: {ex.Message}");
                }

                // Clean up duplicate company profiles and ensure we only have "JPNS"
                try
                {
                    // 1. Update JobListings to point to the primary CompanyId
                    using (var updateJobsCmd = new SqlCommand(@"
                        UPDATE j
                        SET j.CompanyId = min_c.MinCompanyId
                        FROM JPNS.JobListings j
                        JOIN JPNS.CompanyProfiles c ON j.CompanyId = c.CompanyId
                        JOIN (
                            SELECT CompanyName, MIN(CompanyId) AS MinCompanyId
                            FROM JPNS.CompanyProfiles
                            GROUP BY CompanyName
                        ) min_c ON c.CompanyName = min_c.CompanyName
                        WHERE j.CompanyId <> min_c.MinCompanyId;", testConn))
                    {
                        updateJobsCmd.ExecuteNonQuery();
                    }

                    // 2. Update Feedback to point to the primary CompanyId
                    using (var updateFeedbackCmd = new SqlCommand(@"
                        UPDATE f
                        SET f.CompanyId = min_c.MinCompanyId
                        FROM JPNS.Feedback f
                        JOIN JPNS.CompanyProfiles c ON f.CompanyId = c.CompanyId
                        JOIN (
                            SELECT CompanyName, MIN(CompanyId) AS MinCompanyId
                            FROM JPNS.CompanyProfiles
                            GROUP BY CompanyName
                        ) min_c ON c.CompanyName = min_c.CompanyName
                        WHERE f.CompanyId <> min_c.MinCompanyId;", testConn))
                    {
                        updateFeedbackCmd.ExecuteNonQuery();
                    }

                    // 3. Delete duplicate CompanyProfiles
                    using (var deleteDupsCmd = new SqlCommand(@"
                        DELETE cp
                        FROM JPNS.CompanyProfiles cp
                        LEFT JOIN (
                            SELECT CompanyName, MIN(CompanyId) AS MinCompanyId
                            FROM JPNS.CompanyProfiles
                            GROUP BY CompanyName
                        ) min_c ON cp.CompanyId = min_c.MinCompanyId
                        WHERE min_c.MinCompanyId IS NULL;", testConn))
                    {
                        deleteDupsCmd.ExecuteNonQuery();
                    }

                    // 4. Delete feedbacks for other companies
                    using (var cleanFeedbackCmd = new SqlCommand(@"
                        DELETE FROM JPNS.Feedback 
                        WHERE CompanyId IN (SELECT CompanyId FROM JPNS.CompanyProfiles WHERE CompanyName <> 'JPNS');", testConn))
                    {
                        cleanFeedbackCmd.ExecuteNonQuery();
                    }

                    // 5. Delete jobs for other companies
                    using (var cleanJobsCmd = new SqlCommand(@"
                        DELETE FROM JPNS.JobListings 
                        WHERE CompanyId IN (SELECT CompanyId FROM JPNS.CompanyProfiles WHERE CompanyName <> 'JPNS');", testConn))
                    {
                        cleanJobsCmd.ExecuteNonQuery();
                    }

                    // 6. Delete other companies from CompanyProfiles
                    using (var cleanCompCmd = new SqlCommand(@"
                        DELETE FROM JPNS.CompanyProfiles 
                        WHERE CompanyName <> 'JPNS';", testConn))
                    {
                        cleanCompCmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  [DB CLEANUP ERROR] Failed to clean up / de-duplicate company profiles: {ex.Message}");
                }
            }
            catch (SqlException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  [DB] ERROR: Cannot connect to SQL Server.");
                Console.WriteLine($"  [DB] {ex.Message}");
                Console.WriteLine("  [DB] Check your connection string in appsettings.json.");
                Console.WriteLine("  [DB] Running in file-only mode.");
                Console.ResetColor();
                return;
            }

            _connectionString = cs;
            _enabled = true;

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  [DB] Sync init failed — {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine("  [DB] Running in file-only mode.");
            Console.ResetColor();
        }
    }


    public static void SyncJobSeekers(List<JobSeeker> seekers)
    {
        if (!_enabled) return;
        Task.Run(async () =>
        {
            foreach (JobSeeker s in seekers)
            {
                try
                {
                    await UpsertJobSeekerAsync(s);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  [DB ERROR] SyncJobSeekers '{s.Email}': {ex.Message}");
                    Console.ResetColor();
                }
            }
        });
    }

    public static void SyncAdmins(List<Admin1> admins)
    {
        if (!_enabled) return;
        Task.Run(async () =>
        {
            foreach (Admin1 a in admins)
            {
                try
                {
                    await UpsertAdminAsync(a);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  [DB ERROR] SyncAdmins '{a.Email}': {ex.Message}");
                    Console.ResetColor();
                }
            }
        });
    }

    public static void SyncJobs(List<JobListing> jobs)
    {
        if (!_enabled) return;
        Task.Run(async () =>
        {
            foreach (JobListing j in jobs)
            {
                try
                {
                    await UpsertJobListingAsync(j);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  [DB ERROR] SyncJobs '{j.Title}': {ex.Message}");
                    Console.ResetColor();
                }
            }
        });
    }

    public static void SyncApplications(List<Application1> applications)
    {
        if (!_enabled) return;
        Task.Run(async () =>
        {
            foreach (Application1 app in applications)
            {
                try
                {
                    await UpsertApplicationAsync(app);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  [DB ERROR] SyncApplications '{app.JobSeekerId}': {ex.Message}");
                    Console.ResetColor();
                }
            }
        });
    }

    public static void SyncResumes(List<JobSeeker> seekers)
    {
        if (!_enabled) return;

        Task.Run(async () =>
        {
            foreach (var seeker in seekers)
            {
                try
                {
                    await UpsertResumeAsync(seeker);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        });
    }

    public static void SyncSavedJobs(List<JobSeeker> seekers)
    {
        if (!_enabled) return;

        Task.Run(async () =>
        {
            foreach (var seeker in seekers)
            {
                try
                {
                    await SyncSavedJobsForUser(seeker);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  [DB ERROR] SyncSavedJobs '{seeker.Email}': {ex.Message}");
                    Console.ResetColor();
                }
            }
        });
    }

    public static void SyncInterviews(List<Interview1> interviews)
    {
        if (!_enabled) return;

        Task.Run(async () =>
        {
            foreach (var interview in interviews)
            {
                try
                {
                    await UpsertInterviewAsync(interview);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  [DB ERROR] Interview Sync Failed: {ex.Message}");
                    Console.ResetColor();
                }
            }
        });
    }

    public static void SyncNotifications(List<Notification> notifications)
    {
        if (!_enabled) return;

        Task.Run(async () =>
        {
            foreach (var notification in notifications)
            {
                try
                {
                    await UpsertNotificationAsync(notification);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  [DB ERROR] Notification Sync Failed: {ex.Message}");
                    Console.ResetColor();
                }
            }
        });
    }


    private static async Task UpsertJobSeekerAsync(JobSeeker seeker)
    {
        if (seeker.Projects == null) seeker.Projects = new List<Project>();
        if (seeker.Certificates == null) seeker.Certificates = new List<Certificate>();
        if (seeker.EducationList == null) seeker.EducationList = new List<Education>();
        if (seeker.ExperienceList == null) seeker.ExperienceList = new List<WorkExperience>();
        if (seeker.SkillsList == null) seeker.SkillsList = new List<CandidateSkill>();
        if (seeker.Feedbacks == null) seeker.Feedbacks = new List<Feedback>();

        // On-the-fly population of detailed skills list from comma-separated Skills string
        if (seeker.SkillsList.Count == 0 && !string.IsNullOrWhiteSpace(seeker.Skills))
        {
            var skills = seeker.Skills.Split(',');
            foreach (var sk in skills)
            {
                string trimmed = sk.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                {
                    seeker.SkillsList.Add(new CandidateSkill
                    {
                        SkillName = trimmed,
                        ProficiencyLevel = "Intermediate",
                        YearsExperience = 3
                    });
                }
            }
        }

        // On-the-fly population of detailed work experience list from Experience summary string
        if (seeker.ExperienceList.Count == 0 && !string.IsNullOrWhiteSpace(seeker.Experience))
        {
            string expText = seeker.Experience.Trim();
            int years = 2; // default
            if (int.TryParse(expText, out int parsedYears))
            {
                years = parsedYears;
            }
            seeker.ExperienceList.Add(new WorkExperience
            {
                JobTitle = "Software Developer",
                Company = "Previous Employer",
                StartDate = DateTime.Now.AddYears(-years),
                EndDate = DateTime.Now,
                IsCurrent = false
            });
        }

        try
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            object lastLoginParam = seeker.LastLoginAt == DateTime.MinValue
                ? DBNull.Value
                : (object)seeker.LastLoginAt;

            string mergeSql = @"
            MERGE JPNS.Users AS target
            USING (SELECT @Email AS Email) AS source
            ON target.Email = source.Email

            WHEN MATCHED THEN
                UPDATE SET
                    FullName            = @FullName,
                    MobileNumber        = @MobileNumber,
                    PasswordHash        = @PasswordHash,
                    IsActive            = @IsActive,
                    IsEmailVerified     = @IsEmailVerified,
                    IsLocked            = @IsLocked,
                    LockedUntil         = @LockedUntil,
                    FailedLoginAttempts = @FailedAttempts,
                    LastLoginAt         = @LastLoginAt,
                    UpdatedAt           = GETDATE()

            WHEN NOT MATCHED THEN
                INSERT (
                    FullName,
                    Email,
                    Username,
                    PasswordHash,
                    Salt,
                    MobileNumber,
                    Role,
                    IsActive,
                    IsEmailVerified,
                    IsLocked,
                    LockedUntil,
                    FailedLoginAttempts,
                    LastLoginAt
                )
                VALUES (
                    @FullName,
                    @Email,
                    @Email,
                    @PasswordHash,
                    'DEFAULT_SALT',
                    @MobileNumber,
                    'jobseeker',
                    @IsActive,
                    @IsEmailVerified,
                    @IsLocked,
                    @LockedUntil,
                    @FailedAttempts,
                    @LastLoginAt
                );

            SELECT UserId
            FROM JPNS.Users
            WHERE Email = @Email;";

            await using var cmd = new SqlCommand(mergeSql, conn);

            cmd.Parameters.AddWithValue("@FullName", seeker.Name);
            cmd.Parameters.AddWithValue("@Email", seeker.Email.ToLowerInvariant());
            cmd.Parameters.AddWithValue("@PasswordHash", seeker.PasswordHash);
            cmd.Parameters.AddWithValue("@MobileNumber",
                string.IsNullOrWhiteSpace(seeker.Phone)
                    ? DBNull.Value
                    : (object)seeker.Phone);

            cmd.Parameters.AddWithValue("@IsActive", seeker.IsActive);
            cmd.Parameters.AddWithValue("@IsEmailVerified", seeker.IsVerified);
            cmd.Parameters.AddWithValue("@IsLocked", !seeker.IsActive);
            cmd.Parameters.AddWithValue("@LockedUntil", DBNull.Value);
            cmd.Parameters.AddWithValue("@FailedAttempts", seeker.FailedAttempts);
            cmd.Parameters.AddWithValue("@LastLoginAt", lastLoginParam);

            object? result = await cmd.ExecuteScalarAsync();

            if (result == null || result == DBNull.Value)
            {
                Console.WriteLine($"[DB] No UserId returned for {seeker.Email}");
                return;
            }

            int userId = Convert.ToInt32(result);


            string profileSql = @"
            IF EXISTS (SELECT 1 FROM JPNS.JobSeekerProfiles WHERE UserId = @UserId)
            BEGIN
                UPDATE JPNS.JobSeekerProfiles
                SET Headline = @Headline,
                    About = @About,
                    CurrentLocation = @Location,
                    ClearedRounds = @ClearedRounds,
                    TotalRounds = @TotalRounds,
                    CurrentRound = @CurrentRound,
                    RejectedRound = @RejectedRound,
                    CandidateStatus = @CandidateStatus,
                    UpdatedAt = GETDATE()
                WHERE UserId = @UserId
            END
            ELSE
            BEGIN
                INSERT INTO JPNS.JobSeekerProfiles
                (
                    UserId,
                    Headline,
                    About,
                    CurrentLocation,
                    ExperienceYears,
                    ClearedRounds,
                    TotalRounds,
                    CurrentRound,
                    RejectedRound,
                    CandidateStatus
                )
                VALUES
                (
                    @UserId,
                    @Headline,
                    @About,
                    @Location,
                    @ExperienceYears,
                    @ClearedRounds,
                    @TotalRounds,
                    @CurrentRound,
                    @RejectedRound,
                    @CandidateStatus
                )
            END";

            await using var profileCmd = new SqlCommand(profileSql, conn);

            profileCmd.Parameters.AddWithValue("@UserId", userId);
            profileCmd.Parameters.AddWithValue("@Headline",
                string.IsNullOrWhiteSpace(seeker.Skills)
                    ? DBNull.Value
                    : (object)seeker.Skills);

            profileCmd.Parameters.AddWithValue("@About",
                string.IsNullOrWhiteSpace(seeker.Education)
                    ? DBNull.Value
                    : (object)seeker.Education);

            profileCmd.Parameters.AddWithValue("@Location",
                string.IsNullOrWhiteSpace(seeker.Location)
                    ? DBNull.Value
                    : (object)seeker.Location);

            profileCmd.Parameters.AddWithValue("@ExperienceYears", 0);
            profileCmd.Parameters.AddWithValue("@ClearedRounds", seeker.ClearedRounds);
            profileCmd.Parameters.AddWithValue("@TotalRounds", seeker.TotalRounds);
            profileCmd.Parameters.AddWithValue("@CurrentRound", seeker.CurrentRound);
            profileCmd.Parameters.AddWithValue("@RejectedRound", (object)seeker.RejectedRound ?? DBNull.Value);
            profileCmd.Parameters.AddWithValue("@CandidateStatus", string.IsNullOrEmpty(seeker.CandidateStatus) ? DBNull.Value : (object)seeker.CandidateStatus);

            await profileCmd.ExecuteNonQueryAsync();

            await SyncProjectsAsync(seeker.Projects, userId, conn);
            await SyncCertificatesAsync(seeker.Certificates, userId, conn);
            await SyncEducationAsync(seeker.EducationList, userId, conn);
            await SyncWorkExperienceAsync(seeker.ExperienceList, userId, conn);
            await SyncCandidateSkillsAsync(seeker.SkillsList, userId, conn);
            await SyncFeedbackAsync(seeker.Feedbacks, userId, conn);
        }
        catch (SqlException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[SQL ERROR] {seeker.Email}");

            foreach (SqlError error in ex.Errors)
            {
                Console.WriteLine(error.Message);
            }

            Console.ResetColor();
            throw;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[GENERAL ERROR] {seeker.Email}");
            Console.WriteLine(ex.Message);
            Console.ResetColor();
            throw;
        }
    }

    private static async Task UpsertAdminAsync(Admin1 admin)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        string mergeSql = @"
        MERGE JPNS.Users AS target
        USING (SELECT @Email AS Email) AS source ON target.Email = source.Email
        WHEN MATCHED THEN
            UPDATE SET
                FullName        = @FullName,
                PasswordHash    = @PasswordHash,
                IsActive        = @IsActive,
                UpdatedAt       = GETDATE()
        WHEN NOT MATCHED THEN
            INSERT (FullName, Email, Username, PasswordHash, Salt, Role, IsActive, IsEmailVerified)
            VALUES (@FullName, @Email, @Email, @PasswordHash, '', 'admin', @IsActive, 1);

        SELECT UserId FROM JPNS.Users WHERE Email = @Email;";

        await using var cmd = new SqlCommand(mergeSql, conn);
        cmd.Parameters.AddWithValue("@FullName",     admin.Name);
        cmd.Parameters.AddWithValue("@Email",        admin.Email.ToLowerInvariant());
        cmd.Parameters.AddWithValue("@PasswordHash", admin.PasswordHash);
        cmd.Parameters.AddWithValue("@IsActive",     admin.IsActive);

        object? result = await cmd.ExecuteScalarAsync();
        if (result == null || result == DBNull.Value) return;
        int userId = Convert.ToInt32(result);

        string companySql = @"
            IF NOT EXISTS (SELECT 1 FROM JPNS.CompanyProfiles WHERE CompanyName = @CompanyName)
                INSERT INTO JPNS.CompanyProfiles (UserId, CompanyName, IsVerified)
                VALUES (@UserId, @CompanyName, @IsVerified)
            ELSE
                UPDATE JPNS.CompanyProfiles
                SET IsVerified = @IsVerified, UpdatedAt = GETDATE()
                WHERE CompanyName = @CompanyName;";

        await using var companyCmd = new SqlCommand(companySql, conn);
        companyCmd.Parameters.AddWithValue("@UserId",      userId);
        companyCmd.Parameters.AddWithValue("@CompanyName", admin.Company);
        companyCmd.Parameters.AddWithValue("@IsVerified",  admin.IsApproved ? 1 : 0);
        await companyCmd.ExecuteNonQueryAsync();
    }

    private static async Task UpsertJobListingAsync(JobListing job)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        int? companyId = null;
        int? postedByUserId = null;

        await using var compCmd = new SqlCommand("SELECT TOP 1 CompanyId, UserId FROM JPNS.CompanyProfiles ORDER BY CompanyId", conn);
        await using var compReader = await compCmd.ExecuteReaderAsync();
        if (await compReader.ReadAsync())
        {
            companyId      = (int)compReader["CompanyId"];
            postedByUserId = (int)compReader["UserId"];
        }
        await compReader.CloseAsync();

        if (companyId == null) return;

        string status = job.Status.ToString().ToLower() switch
        {
            "pending_approval" => "pending",
            "approved"         => "approved",
            "active"           => "approved",
            "closed"           => "closed",
            "rejected"         => "rejected",
            "expired"          => "closed",
            _                  => "pending"
        };

        object expiryParam = job.ExpiryDate == DateTime.MinValue
            ? (object)DateTime.Now.AddDays(30)
            : job.ExpiryDate;

        string mergeSql = @"
            IF EXISTS (
                SELECT 1 FROM JPNS.JobListings
                WHERE PostedByUserId = @PostedByUserId AND Title = @Title
            )
                UPDATE JPNS.JobListings SET
                    Title              = @Title,
                    Description        = @Description,
                    Location           = @Location,
                    SalaryMin          = @SalaryMin,
                    SalaryMax          = @SalaryMax,
                    ExperienceRequired = @ExperienceRequired,
                    Status             = @Status,
                    ExpiryDate         = @ExpiryDate,
                    NumberOfRounds     = @NumberOfRounds,
                    UpdatedAt          = GETDATE()
                WHERE PostedByUserId = @PostedByUserId AND Title = @Title
            ELSE
                INSERT INTO JPNS.JobListings
                    (PostedByUserId, CompanyId, Title, Description, Location, JobType,
                     SalaryMin, SalaryMax, ExperienceRequired, Status, ExpiryDate, PostedDate, NumberOfRounds)
                VALUES
                    (@PostedByUserId, @CompanyId, @Title, @Description, @Location, 'Full-time',
                     @SalaryMin, @SalaryMax, @ExperienceRequired, @Status, @ExpiryDate, @PostedDate, @NumberOfRounds);";

        await using var jobCmd = new SqlCommand(mergeSql, conn);
        jobCmd.Parameters.AddWithValue("@PostedByUserId",     postedByUserId!);
        jobCmd.Parameters.AddWithValue("@CompanyId",          companyId!);
        jobCmd.Parameters.AddWithValue("@Title",              job.Title);
        jobCmd.Parameters.AddWithValue("@Description",        job.Description);
        jobCmd.Parameters.AddWithValue("@Location",           job.Location);
        jobCmd.Parameters.AddWithValue("@SalaryMin",          (object)job.SalaryRange.Min);
        jobCmd.Parameters.AddWithValue("@SalaryMax",          (object)job.SalaryRange.Max);
        jobCmd.Parameters.AddWithValue("@ExperienceRequired", job.ExperienceRequired);
        jobCmd.Parameters.AddWithValue("@Status",             status);
        jobCmd.Parameters.AddWithValue("@ExpiryDate",         expiryParam);
        jobCmd.Parameters.AddWithValue("@PostedDate",         job.PostedDate == DateTime.MinValue ? (object)DateTime.Now : job.PostedDate);
        jobCmd.Parameters.AddWithValue("@NumberOfRounds",     job.NumberOfRounds);
        await jobCmd.ExecuteNonQueryAsync();

        int dbJobId;
        await using (var getJobIdCmd = new SqlCommand(
            @"SELECT TOP 1 JobId
              FROM JPNS.JobListings
              WHERE Title = @Title
              ORDER BY JobId DESC", conn))
        {
            getJobIdCmd.Parameters.AddWithValue("@Title", job.Title);

            object? result = await getJobIdCmd.ExecuteScalarAsync();

            if (result == null || result == DBNull.Value)
                return;

            dbJobId = Convert.ToInt32(result);
        }

        if (job.RequiredSkills != null)
        {
            foreach (var skill in job.RequiredSkills)
            {
                if (string.IsNullOrWhiteSpace(skill))
                    continue;

                string trimmedSkill = skill.Trim();

                int skillId;
                await using (var ensureSkillCmd = new SqlCommand(@"
                IF NOT EXISTS (SELECT 1 FROM JPNS.Skills WHERE SkillName = @SkillName)
                    INSERT INTO JPNS.Skills (SkillName) VALUES (@SkillName);
                SELECT SkillId FROM JPNS.Skills WHERE SkillName = @SkillName;", conn))
                {
                    ensureSkillCmd.Parameters.AddWithValue("@SkillName", trimmedSkill);
                    object? skillResult = await ensureSkillCmd.ExecuteScalarAsync();
                    if (skillResult == null || skillResult == DBNull.Value)
                        continue;
                    skillId = Convert.ToInt32(skillResult);
                }

                await using var jobSkillCmd = new SqlCommand(@"
                IF NOT EXISTS (
                    SELECT 1 FROM JPNS.JobSkills
                    WHERE JobId = @JobId AND SkillId = @SkillId
                )
                INSERT INTO JPNS.JobSkills (JobId, SkillId, IsRequired)
                VALUES (@JobId, @SkillId, 1)", conn);

                jobSkillCmd.Parameters.AddWithValue("@JobId",   dbJobId);
                jobSkillCmd.Parameters.AddWithValue("@SkillId", skillId);
                await jobSkillCmd.ExecuteNonQueryAsync();
            }
        }
    }

    private static async Task UpsertApplicationAsync(Application1 app)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        string email = GetEmailForSeekerId(app.JobSeekerId);
        if (string.IsNullOrEmpty(email))
        {
            Console.WriteLine($"[DB ERROR] Job seeker email not resolved for: {app.JobSeekerId}");
            return;
        }

        await using var uidCmd = new SqlCommand(
            @"SELECT UserId FROM JPNS.Users WHERE Email = @Email",
            conn);

        uidCmd.Parameters.AddWithValue("@Email", email.ToLowerInvariant());

        object? uidResult = await uidCmd.ExecuteScalarAsync();

        if (uidResult == null || uidResult == DBNull.Value)
        {
            Console.WriteLine($"[DB ERROR] User not found in database: {email}");
            return;
        }

        int userId = Convert.ToInt32(uidResult);

        int jobId = 0;
        if (!int.TryParse(app.JobId, out jobId))
        {
            var jobFromStore = FileStorage.DataStore.Jobs.Find(j => j.JobId == app.JobId);
            if (jobFromStore != null)
            {
                await using var jobTitleCmd = new SqlCommand("SELECT JobId FROM JPNS.JobListings WHERE Title = @Title", conn);
                jobTitleCmd.Parameters.AddWithValue("@Title", jobFromStore.Title);
                object? jobTitleResult = await jobTitleCmd.ExecuteScalarAsync();
                if (jobTitleResult != null && jobTitleResult != DBNull.Value)
                {
                    jobId = Convert.ToInt32(jobTitleResult);
                }
            }
        }

        if (jobId == 0)
        {
            Console.WriteLine($"[DB ERROR] Invalid JobId: {app.JobId}");
            return;
        }

        int resumeId = await EnsurePlaceholderResumeAsync(conn, userId);
        string status = app.Status.ToString().ToUpper() switch
        {
            "PENDING"              => "PENDING",
            "SHORTLISTED"          => "SHORTLISTED",
            "INTERVIEW_SCHEDULED"  => "SHORTLISTED",
            "REJECTED"             => "REJECTED",
            "HIRED"                => "HIRED",
            _                      => "PENDING"
        };

        string sql = @"
            IF NOT EXISTS
            (
                SELECT 1
                FROM JPNS.Applications
                WHERE UserId = @UserId
                  AND JobId = @JobId
            )
            BEGIN
                INSERT INTO JPNS.Applications
                (
                    UserId,
                    JobId,
                    ResumeId,
                    CoverLetter,
                    Status,
                    AppliedDate
                )
                VALUES
                (
                    @UserId,
                    @JobId,
                    @ResumeId,
                    @CoverLetter,
                    @Status,
                    @AppliedDate
                )
            END
            ELSE
            BEGIN
                UPDATE JPNS.Applications
                SET
                    Status = @Status,
                    UpdatedAt = GETDATE()
                WHERE UserId = @UserId
                  AND JobId = @JobId
            END";

        await using var cmd = new SqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@UserId",   userId);
        cmd.Parameters.AddWithValue("@JobId",    jobId);
        cmd.Parameters.AddWithValue("@ResumeId", resumeId);

        cmd.Parameters.AddWithValue(
            "@CoverLetter",
            string.IsNullOrWhiteSpace(app.CoverLetter)? DBNull.Value: (object)app.CoverLetter);

        cmd.Parameters.AddWithValue("@Status", status);

        cmd.Parameters.AddWithValue(
            "@AppliedDate",
            app.AppliedDate == DateTime.MinValue
                ? DateTime.Now
                : app.AppliedDate);

        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task UpsertResumeAsync(JobSeeker seeker)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var userCmd = new SqlCommand(
            "SELECT UserId FROM JPNS.Users WHERE Email=@Email",
            conn);

        userCmd.Parameters.AddWithValue("@Email", seeker.Email.ToLowerInvariant());

        object? result = await userCmd.ExecuteScalarAsync();

        if (result == null || result == DBNull.Value)
            return;

        int userId = Convert.ToInt32(result);

        await using var cmd = new SqlCommand(@"
        IF NOT EXISTS
        (
            SELECT 1
            FROM JPNS.Resumes
            WHERE UserId=@UserId
        )
        INSERT INTO JPNS.Resumes
        (
            UserId,
            FileName,
            FileType,
            FileUrl
        )
        VALUES
        (
            @UserId,
            @FileName,
            'PDF',
            @FileUrl
        )", conn);

        cmd.Parameters.AddWithValue("@UserId", userId);

        cmd.Parameters.AddWithValue(
            "@FileName",
            string.IsNullOrWhiteSpace(seeker.ResumeUrl)
                ? "resume.pdf"
                : Path.GetFileName(seeker.ResumeUrl));

        cmd.Parameters.AddWithValue(
            "@FileUrl",
            string.IsNullOrWhiteSpace(seeker.ResumeUrl)
                ? "/resumes/resume.pdf"
                : seeker.ResumeUrl);

        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task SyncSavedJobsForUser(JobSeeker seeker)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var userCmd = new SqlCommand(
            "SELECT UserId FROM JPNS.Users WHERE Email=@Email",
            conn);

        userCmd.Parameters.AddWithValue("@Email", seeker.Email.ToLowerInvariant());

        object? userObj = await userCmd.ExecuteScalarAsync();

        if (userObj == null || userObj == DBNull.Value)
            return;

        int userId = Convert.ToInt32(userObj);

        foreach (var consoleJobId in seeker.SavedJobs)
        {
            if (!int.TryParse(consoleJobId, out int dbJobId))
                continue;

            await using var checkJobCmd = new SqlCommand(
                "SELECT COUNT(1) FROM JPNS.JobListings WHERE JobId = @JobId", conn);
            checkJobCmd.Parameters.AddWithValue("@JobId", dbJobId);
            object? jobExists = await checkJobCmd.ExecuteScalarAsync();
            if (Convert.ToInt32(jobExists) == 0)
                continue;

            await using var cmd = new SqlCommand(@"
            IF NOT EXISTS
            (
                SELECT 1
                FROM JPNS.SavedJobs
                WHERE UserId=@UserId
                  AND JobId=@JobId
            )
            INSERT INTO JPNS.SavedJobs
            (
                UserId,
                JobId
            )
            VALUES
            (
                @UserId,
                @JobId
            )", conn);

            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@JobId",  dbJobId);

            await cmd.ExecuteNonQueryAsync();
        }
    }

    private static async Task UpsertInterviewAsync(Interview1 interview)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        Application1? consoleApp = null;
        foreach (Application1 a in FileStorage.DataStore.Applications)
        {
            if (a.ApplicationId == interview.ApplicationId)
            {
                consoleApp = a;
                break;
            }
        }

        if (consoleApp == null)
        {
            Console.WriteLine($"[DB ERROR] Console Application not found: {interview.ApplicationId}");
            return;
        }

        string email = GetEmailForSeekerId(consoleApp.JobSeekerId);
        if (string.IsNullOrEmpty(email))
        {
            Console.WriteLine($"[DB ERROR] Seeker email not resolved for interview: {consoleApp.JobSeekerId}");
            return;
        }

        await using var userCmd = new SqlCommand(
            "SELECT UserId FROM JPNS.Users WHERE Email = @Email", conn);
        userCmd.Parameters.AddWithValue("@Email", email.ToLowerInvariant());
        object? userObj = await userCmd.ExecuteScalarAsync();
        if (userObj == null || userObj == DBNull.Value)
        {
            Console.WriteLine($"[DB ERROR] User not in DB: {email}");
            return;
        }
        int userId = Convert.ToInt32(userObj);

        int jobId = 0;
        if (!int.TryParse(consoleApp.JobId, out jobId))
        {
            var jobFromStore = FileStorage.DataStore.Jobs.Find(j => j.JobId == consoleApp.JobId);
            if (jobFromStore != null)
            {
                await using var jobTitleCmd = new SqlCommand("SELECT JobId FROM JPNS.JobListings WHERE Title = @Title", conn);
                jobTitleCmd.Parameters.AddWithValue("@Title", jobFromStore.Title);
                object? jobTitleResult = await jobTitleCmd.ExecuteScalarAsync();
                if (jobTitleResult != null && jobTitleResult != DBNull.Value)
                {
                    jobId = Convert.ToInt32(jobTitleResult);
                }
            }
        }

        if (jobId == 0)
        {
            Console.WriteLine($"[DB ERROR] Invalid JobId for interview: {consoleApp.JobId}");
            return;
        }

        await using var checkJobCmd = new SqlCommand(
            "SELECT COUNT(1) FROM JPNS.JobListings WHERE JobId = @JobId", conn);
        checkJobCmd.Parameters.AddWithValue("@JobId", jobId);
        if (Convert.ToInt32(await checkJobCmd.ExecuteScalarAsync()) == 0)
        {
            Console.WriteLine($"[DB ERROR] JobId {jobId} not found in DB for interview sync.");
            return;
        }
        await using var appCmd = new SqlCommand(
            "SELECT TOP 1 ApplicationId FROM JPNS.Applications WHERE UserId = @UserId AND JobId = @JobId",
            conn);
        appCmd.Parameters.AddWithValue("@UserId", userId);
        appCmd.Parameters.AddWithValue("@JobId",  jobId);
        object? appObj = await appCmd.ExecuteScalarAsync();
        if (appObj == null || appObj == DBNull.Value)
        {
            Console.WriteLine($"[DB ERROR] DB Application not found for UserId={userId}, JobId={jobId}");
            return;
        }
        int dbApplicationId = Convert.ToInt32(appObj);

        await using var adminCmd = new SqlCommand(
            "SELECT TOP 1 UserId FROM JPNS.Users WHERE Role = 'admin' ORDER BY UserId", conn);
        object? adminObj = await adminCmd.ExecuteScalarAsync();
        if (adminObj == null || adminObj == DBNull.Value)
        {
            Console.WriteLine("[DB ERROR] No admin user found for ScheduledByUserId.");
            return;
        }
        int scheduledByUserId = Convert.ToInt32(adminObj);

        int dbInterviewId = 0;
        bool hasDbId = int.TryParse(interview.InterviewId, out dbInterviewId);

        string sql;
        if (hasDbId)
        {
            sql = @"
                UPDATE JPNS.Interviews
                SET ScheduledAt = @ScheduledAt,
                    Mode        = @Mode,
                    MeetingLink = @MeetingLink,
                    Venue       = @Venue,
                    Status      = @Status,
                    UpdatedAt   = GETDATE()
                WHERE InterviewId = @InterviewId";
        }
        else
        {
            sql = @"
                IF EXISTS
                (
                    SELECT 1
                    FROM JPNS.Interviews
                    WHERE ApplicationId = @ApplicationId
                      AND Status        = 'SCHEDULED'
                )
                BEGIN
                    UPDATE JPNS.Interviews
                    SET ScheduledAt = @ScheduledAt,
                        Mode        = @Mode,
                        MeetingLink = @MeetingLink,
                        Venue       = @Venue,
                        Status      = @Status,
                        UpdatedAt   = GETDATE()
                    WHERE ApplicationId = @ApplicationId
                      AND Status        = 'SCHEDULED';

                    SELECT TOP 1 InterviewId FROM JPNS.Interviews
                    WHERE ApplicationId = @ApplicationId
                      AND Status        = 'SCHEDULED';
                END
                ELSE
                BEGIN
                    INSERT INTO JPNS.Interviews
                    (
                        ApplicationId,
                        ScheduledByUserId,
                        ScheduledAt,
                        Mode,
                        MeetingLink,
                        Venue,
                        Status,
                        CreatedAt
                    )
                    VALUES
                    (
                        @ApplicationId,
                        @ScheduledByUserId,
                        @ScheduledAt,
                        @Mode,
                        @MeetingLink,
                        @Venue,
                        @Status,
                        @CreatedAt
                    );
                    SELECT SCOPE_IDENTITY();
                END";
        }

        await using var cmd = new SqlCommand(sql, conn);

        if (hasDbId)
        {
            cmd.Parameters.AddWithValue("@InterviewId", dbInterviewId);
        }

        cmd.Parameters.AddWithValue("@ApplicationId",     dbApplicationId);
        cmd.Parameters.AddWithValue("@ScheduledByUserId", scheduledByUserId);
        cmd.Parameters.AddWithValue("@ScheduledAt",       interview.ScheduledAt);
        cmd.Parameters.AddWithValue("@Mode",              interview.Mode);

        cmd.Parameters.AddWithValue(
            "@MeetingLink",
            string.IsNullOrWhiteSpace(interview.MeetingLink)
                ? DBNull.Value
                : (object)interview.MeetingLink);

        cmd.Parameters.AddWithValue(
            "@Venue",
            string.IsNullOrWhiteSpace(interview.Venue)
                ? DBNull.Value
                : (object)interview.Venue);

        string ivStatus = interview.Status.ToString().ToUpper() switch
        {
            "SCHEDULED"  => "SCHEDULED",
            "COMPLETED"  => "COMPLETED",
            "CANCELLED"  => "CANCELLED",
            "NO_SHOW"    => "NO_SHOW",
            _            => "SCHEDULED"
        };
        cmd.Parameters.AddWithValue("@Status",    ivStatus);
        cmd.Parameters.AddWithValue("@CreatedAt", interview.CreatedAt);

        if (hasDbId)
        {
            await cmd.ExecuteNonQueryAsync();
        }
        else
        {
            object? result = await cmd.ExecuteScalarAsync();
            if (result != null && result != DBNull.Value)
            {
                interview.InterviewId = result.ToString()!;
            }
        }
    }

    private static async Task UpsertNotificationAsync(Notification notification)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        string email = "";
        if (notification.UserId.Contains("@"))
        {
            email = notification.UserId;
        }
        else if (notification.UserId.StartsWith("ADM", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var admin in FileStorage.DataStore.Admins)
            {
                if (admin.AdminId.Equals(notification.UserId, StringComparison.OrdinalIgnoreCase) || admin.Id.Equals(notification.UserId, StringComparison.OrdinalIgnoreCase))
                {
                    email = admin.Email;
                    break;
                }
            }
        }
        else if (notification.UserId.StartsWith("JSK", StringComparison.OrdinalIgnoreCase))
        {
            email = GetEmailForSeekerId(notification.UserId);
        }
        else
        {
            foreach (var seeker in FileStorage.DataStore.JobSeekers)
            {
                if (seeker.SeekerId.Equals(notification.UserId, StringComparison.OrdinalIgnoreCase) || seeker.Email.Equals(notification.UserId, StringComparison.OrdinalIgnoreCase))
                {
                    email = seeker.Email;
                    break;
                }
            }
            if (string.IsNullOrEmpty(email))
            {
                foreach (var admin in FileStorage.DataStore.Admins)
                {
                    if (admin.AdminId.Equals(notification.UserId, StringComparison.OrdinalIgnoreCase) || admin.Email.Equals(notification.UserId, StringComparison.OrdinalIgnoreCase))
                    {
                        email = admin.Email;
                        break;
                    }
                }
            }
        }

        if (string.IsNullOrEmpty(email))
        {
            return;
        }

        await using var userCmd = new SqlCommand(
            "SELECT UserId FROM JPNS.Users WHERE Email = @Email", conn);
        userCmd.Parameters.AddWithValue("@Email", email.ToLowerInvariant());
        object? userObj = await userCmd.ExecuteScalarAsync();
        if (userObj == null || userObj == DBNull.Value)
            return;

        int userId = Convert.ToInt32(userObj);

        string fullMessage = string.IsNullOrWhiteSpace(notification.Title) ? notification.Message : $"[{notification.Title}] {notification.Message}";

        if (fullMessage.Length > 4000)
            fullMessage = fullMessage.Substring(0, 4000);

        string sql = @"
        IF NOT EXISTS (
            SELECT 1 FROM JPNS.Notifications 
            WHERE UserId = @UserId 
              AND Message = @Message 
              AND CreatedAt = @CreatedAt
        )
            INSERT INTO JPNS.Notifications (UserId, Message, NotificationType, IsRead, CreatedAt)
            VALUES (@UserId, @Message, @NotificationType, @IsRead, @CreatedAt)
        ELSE
            UPDATE JPNS.Notifications
            SET IsRead = @IsRead
            WHERE UserId = @UserId 
              AND Message = @Message 
              AND CreatedAt = @CreatedAt;";

        await using var cmd = new SqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@UserId",           userId);
        cmd.Parameters.AddWithValue("@Message",          fullMessage);
        cmd.Parameters.AddWithValue("@NotificationType", notification.Type);
        cmd.Parameters.AddWithValue("@IsRead",           notification.IsRead);
        cmd.Parameters.AddWithValue("@CreatedAt",        notification.CreatedAt);

        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task<int> EnsurePlaceholderResumeAsync(
    SqlConnection conn,
    int userId)
    {
        await using var checkCmd =
            new SqlCommand(
                "SELECT TOP 1 ResumeId FROM JPNS.Resumes WHERE UserId=@UserId",
                conn);

        checkCmd.Parameters.AddWithValue("@UserId", userId);

        object? result = await checkCmd.ExecuteScalarAsync();

        if (result != null && result != DBNull.Value)
            return Convert.ToInt32(result);

        await using var insertCmd =
            new SqlCommand(@"
            INSERT INTO JPNS.Resumes
            (
                UserId,
                FileName,
                FileType,
                FileUrl
            )
            VALUES
            (
                @UserId,
                'resume.pdf',
                'PDF',
                '/resumes/resume.pdf'
            );

            SELECT SCOPE_IDENTITY();", conn);

        insertCmd.Parameters.AddWithValue("@UserId", userId);

        object? newId = await insertCmd.ExecuteScalarAsync();

        return Convert.ToInt32(newId);
    }


    private static string ExtractServer(string connStr)
    {
        foreach (string part in connStr.Split(';'))
        {
            string trimmed = part.Trim();
            if (trimmed.StartsWith("Server=", StringComparison.OrdinalIgnoreCase) ||
                trimmed.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
            {
                int eq = trimmed.IndexOf('=');
                if (eq >= 0) return trimmed.Substring(eq + 1).Trim();
            }
        }
        return "(unknown)";
    }

    public static void SyncComplaints(List<Complaint> complaints)
    {
        if (!_enabled) return;
        Task.Run(async () =>
        {
            foreach (Complaint c in complaints)
            {
                try
                {
                    await UpsertComplaintAsync(c);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  [DB ERROR] SyncComplaints ID '{c.ComplaintId}': {ex.Message}");
                    Console.ResetColor();
                }
            }
        });
    }

    private static async Task UpsertComplaintAsync(Complaint c)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        if (c.ComplaintId <= 0)
        {
            await using var cmd = new SqlCommand("JPNS.usp_RaiseComplaint", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@SubmittedByUserId", c.SubmittedByUserId);
            cmd.Parameters.AddWithValue("@AgainstUserId", (object?)c.AgainstUserId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Subject", c.Subject);
            cmd.Parameters.AddWithValue("@Description", c.Description);

            var outParam = new SqlParameter("@ComplaintId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outParam);

            await cmd.ExecuteNonQueryAsync();
            c.ComplaintId = (int)outParam.Value;
        }
        else
        {
            await using var cmd = new SqlCommand(@"
                UPDATE JPNS.Complaints
                SET Status = @Status,
                    AdminNotes = @AdminNotes,
                    ResolvedAt = @ResolvedAt
                WHERE ComplaintId = @ComplaintId", conn);
            cmd.Parameters.AddWithValue("@ComplaintId", c.ComplaintId);
            cmd.Parameters.AddWithValue("@Status", c.Status);
            cmd.Parameters.AddWithValue("@AdminNotes", (object?)c.AdminNotes ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ResolvedAt", (object?)c.ResolvedAt ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }
    }

    public static void LoadAllFromDb()
    {
        if (!_enabled) return;
        
        try
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            
            // 1. Load Job Seekers
            var seekers = new List<JobSeeker>();
            using (var cmd = new SqlCommand(@"
                SELECT u.UserId, u.FullName, u.Email, u.PasswordHash, u.MobileNumber, u.IsActive, u.IsEmailVerified, u.FailedLoginAttempts, u.LastLoginAt, u.CreatedAt,
                       p.Headline, p.About, p.CurrentLocation, p.ExperienceYears, p.ClearedRounds, p.TotalRounds,
                       p.CurrentRound, p.RejectedRound, p.CandidateStatus,
                       r.FileUrl
                FROM JPNS.Users u
                LEFT JOIN JPNS.JobSeekerProfiles p ON u.UserId = p.UserId
                LEFT JOIN JPNS.Resumes r ON u.UserId = r.UserId
                WHERE u.Role = 'jobseeker'", conn))
            {
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int userId = (int)reader["UserId"];
                    string seekerId = "JSK-2026-" + userId.ToString("D4");
                    string name = reader["FullName"].ToString()!;
                    string email = reader["Email"].ToString()!;
                    string passwordHash = reader["PasswordHash"].ToString()!;
                    string phone = reader["MobileNumber"] == DBNull.Value ? "" : reader["MobileNumber"].ToString()!;
                    bool isActive = (bool)reader["IsActive"];
                    bool isVerified = (bool)reader["IsEmailVerified"];
                    DateTime registeredAt = (DateTime)reader["CreatedAt"];
                    DateTime lastLoginAt = reader["LastLoginAt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["LastLoginAt"];
                    
                    string headline = reader["Headline"] == DBNull.Value ? "" : reader["Headline"].ToString()!;
                    string about = reader["About"] == DBNull.Value ? "" : reader["About"].ToString()!;
                    string location = reader["CurrentLocation"] == DBNull.Value ? "" : reader["CurrentLocation"].ToString()!;
                    string resumeUrl = reader["FileUrl"] == DBNull.Value ? "" : reader["FileUrl"].ToString()!;
                    int failedAttempts = reader["FailedLoginAttempts"] == DBNull.Value ? 0 : (int)reader["FailedLoginAttempts"];
                    int clearedRounds = reader["ClearedRounds"] == DBNull.Value ? 0 : (int)reader["ClearedRounds"];
                    int totalRounds = reader["TotalRounds"] == DBNull.Value ? 3 : (int)reader["TotalRounds"];
                    int currentRound = reader["CurrentRound"] == DBNull.Value ? 0 : (int)reader["CurrentRound"];
                    int? rejectedRound = reader["RejectedRound"] == DBNull.Value ? null : (int?)reader["RejectedRound"];
                    string? candidateStatus = reader["CandidateStatus"] == DBNull.Value ? null : reader["CandidateStatus"].ToString();
                    
                    var s = new JobSeeker();
                    s.SeekerId = seekerId;
                    s.Id = seekerId;
                    s.Name = name;
                    s.Email = email;
                    s.PasswordHash = passwordHash;
                    s.Phone = phone;
                    s.IsActive = isActive;
                    s.IsVerified = isVerified;
                    s.RegisteredAt = registeredAt;
                    s.LastLoginAt = lastLoginAt;
                    s.Skills = headline;
                    s.Education = about;
                    s.Location = location;
                    s.ResumeUrl = resumeUrl;
                    s.FailedAttempts = failedAttempts;
                    s.ClearedRounds = clearedRounds;
                    s.TotalRounds = totalRounds;
                    s.CurrentRound = currentRound;
                    s.RejectedRound = rejectedRound;
                    s.CandidateStatus = candidateStatus;
                    s.SavedJobs = new List<string>();
                    
                    seekers.Add(s);
                }
            }
            
            // Populate Saved Jobs and Detail Collections for Seekers
            foreach (var s in seekers)
            {
                string[] parts = s.SeekerId.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int userId))
                {
                    using (var cmd = new SqlCommand("SELECT JobId FROM JPNS.SavedJobs WHERE UserId = @UserId", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            s.SavedJobs.Add(reader["JobId"].ToString()!);
                        }
                    }

                    // 1. Load Projects
                    using (var cmd = new SqlCommand("SELECT Title, Description, Technologies, ProjectUrl, StartDate, EndDate FROM JPNS.Projects WHERE UserId = @UserId", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            s.Projects.Add(new Project
                            {
                                Title = reader["Title"].ToString()!,
                                Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                                Technologies = reader["Technologies"] == DBNull.Value ? null : reader["Technologies"].ToString(),
                                ProjectUrl = reader["ProjectUrl"] == DBNull.Value ? null : reader["ProjectUrl"].ToString(),
                                StartDate = reader["StartDate"] == DBNull.Value ? DateTime.Now : (DateTime)reader["StartDate"],
                                EndDate = reader["EndDate"] == DBNull.Value ? null : (DateTime?)reader["EndDate"]
                            });
                        }
                    }

                    // 2. Load Certificates
                    using (var cmd = new SqlCommand("SELECT CertName, IssuingOrg, IssueDate, ExpiryDate, CertificateUrl, CredentialId FROM JPNS.Certificates WHERE UserId = @UserId", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            s.Certificates.Add(new Certificate
                            {
                                CertName = reader["CertName"].ToString()!,
                                IssuingOrg = reader["IssuingOrg"] == DBNull.Value ? null : reader["IssuingOrg"].ToString(),
                                IssueDate = reader["IssueDate"] == DBNull.Value ? DateTime.Now : (DateTime)reader["IssueDate"],
                                ExpiryDate = reader["ExpiryDate"] == DBNull.Value ? null : (DateTime?)reader["ExpiryDate"],
                                CertificateUrl = reader["CertificateUrl"] == DBNull.Value ? null : reader["CertificateUrl"].ToString(),
                                CredentialId = reader["CredentialId"] == DBNull.Value ? null : reader["CredentialId"].ToString()
                            });
                        }
                    }

                    // 3. Load Education
                    using (var cmd = new SqlCommand("SELECT Degree, Institution, Grade FROM JPNS.Education WHERE UserId = @UserId", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            s.EducationList.Add(new Education
                            {
                                Degree = reader["Degree"].ToString()!,
                                Institution = reader["Institution"].ToString()!,
                                Grade = reader["Grade"] == DBNull.Value ? null : reader["Grade"].ToString()
                            });
                        }
                    }

                    // 4. Load Work Experience
                    using (var cmd = new SqlCommand("SELECT JobTitle, Company, StartDate, EndDate, IsCurrent FROM JPNS.WorkExperience WHERE UserId = @UserId", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            s.ExperienceList.Add(new WorkExperience
                            {
                                JobTitle = reader["JobTitle"].ToString()!,
                                Company = reader["Company"].ToString()!,
                                StartDate = reader["StartDate"] == DBNull.Value ? DateTime.Now : (DateTime)reader["StartDate"],
                                EndDate = reader["EndDate"] == DBNull.Value ? null : (DateTime?)reader["EndDate"],
                                IsCurrent = (bool)reader["IsCurrent"]
                            });
                        }
                    }

                    // 5. Load Skills
                    using (var cmd = new SqlCommand(@"
                        SELECT s.SkillName, cs.ProficiencyLevel, cs.YearsExperience
                        FROM JPNS.CandidateSkills cs
                        JOIN JPNS.Skills s ON cs.SkillId = s.SkillId
                        WHERE cs.UserId = @UserId", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            s.SkillsList.Add(new CandidateSkill
                            {
                                SkillName = reader["SkillName"].ToString()!,
                                ProficiencyLevel = reader["ProficiencyLevel"].ToString()!,
                                YearsExperience = reader["YearsExperience"] == DBNull.Value ? null : (int?)reader["YearsExperience"]
                            });
                        }
                    }

                    // 6. Load Feedback
                    using (var cmd = new SqlCommand(@"
                        SELECT cp.CompanyName, f.Rating, f.Review, f.IsAnonymous, f.CreatedAt
                        FROM JPNS.Feedback f
                        JOIN JPNS.CompanyProfiles cp ON f.CompanyId = cp.CompanyId
                        WHERE f.UserId = @UserId", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            s.Feedbacks.Add(new Feedback
                            {
                                CompanyName = reader["CompanyName"].ToString()!,
                                Rating = (byte)reader["Rating"],
                                Review = reader["Review"] == DBNull.Value ? null : reader["Review"].ToString(),
                                IsAnonymous = (bool)reader["IsAnonymous"],
                                CreatedAt = (DateTime)reader["CreatedAt"]
                            });
                        }
                    }
                }
            }
            
            // 2. Load Admins
            var admins = new List<Admin1>();
            using (var cmd = new SqlCommand(@"
                SELECT u.UserId, u.FullName, u.Email, u.PasswordHash, u.IsActive, u.CreatedAt, u.LastLoginAt,
                       c.CompanyName, c.IsVerified
                FROM JPNS.Users u
                LEFT JOIN JPNS.CompanyProfiles c ON u.UserId = c.UserId
                WHERE u.Role = 'admin'", conn))
            {
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int userId = (int)reader["UserId"];
                    string adminId = "ADM" + userId.ToString("D3");
                    string name = reader["FullName"].ToString()!;
                    string email = reader["Email"].ToString()!;
                    string passwordHash = reader["PasswordHash"].ToString()!;
                    bool isActive = (bool)reader["IsActive"];
                    DateTime createdAt = (DateTime)reader["CreatedAt"];
                    DateTime lastLoginAt = reader["LastLoginAt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["LastLoginAt"];
                    string companyName = reader["CompanyName"] == DBNull.Value ? "" : reader["CompanyName"].ToString()!;
                    bool isVerified = reader["IsVerified"] == DBNull.Value ? false : (bool)reader["IsVerified"];
                    
                    var a = new Admin1();
                    a.AdminId = adminId;
                    a.Id = adminId;
                    a.Name = name;
                    a.Email = email;
                    a.PasswordHash = passwordHash;
                    a.IsActive = isActive;
                    a.CreatedAt = createdAt;
                    a.LastLoginAt = lastLoginAt;
                    a.Company = companyName;
                    a.IsApproved = isVerified;
                    
                    admins.Add(a);
                }
            }
            
            // 3. Load Jobs
            var jobs = new List<JobListing>();
            using (var cmd = new SqlCommand(@"
                SELECT j.JobId, j.PostedByUserId, j.CompanyId, j.Title, j.Description, j.Location, j.JobType, j.SalaryMin, j.SalaryMax, j.ExperienceRequired, j.Status, j.TotalApplications, j.ExpiryDate, j.PostedDate, j.UpdatedAt, j.NumberOfRounds, c.CompanyName
                FROM JPNS.JobListings j
                JOIN JPNS.CompanyProfiles c ON j.CompanyId = c.CompanyId", conn))
            {
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int jobId = (int)reader["JobId"];
                    int postedByUserId = (int)reader["PostedByUserId"];
                    int companyId = (int)reader["CompanyId"];
                    string title = reader["Title"].ToString()!;
                    string description = reader["Description"].ToString()!;
                    string location = reader["Location"].ToString()!;
                    string jobType = reader["JobType"].ToString()!;
                    decimal? salaryMin = reader["SalaryMin"] == DBNull.Value ? null : (decimal?)reader["SalaryMin"];
                    decimal? salaryMax = reader["SalaryMax"] == DBNull.Value ? null : (decimal?)reader["SalaryMax"];
                    int expReq = (int)reader["ExperienceRequired"];
                    string statusStr = reader["Status"].ToString()!;
                    int totalApps = (int)reader["TotalApplications"];
                    DateTime expiryDate = (DateTime)reader["ExpiryDate"];
                    DateTime postedDate = (DateTime)reader["PostedDate"];
                    DateTime? updatedAt = reader["UpdatedAt"] == DBNull.Value ? null : (DateTime?)reader["UpdatedAt"];
                    int numRounds = reader["NumberOfRounds"] == DBNull.Value ? 3 : (int)reader["NumberOfRounds"];
                    string companyName = reader["CompanyName"].ToString()!;
                    
                    var job = new JobListing();
                    job.JobId = jobId.ToString();
                    job.AdminId = "ADM" + postedByUserId.ToString("D3");
                    job.PostedByUserId = postedByUserId;
                    job.CompanyId = companyId;
                    job.Title = title;
                    job.Description = description;
                    job.Company = companyName;
                    job.CompanyName = companyName;
                    job.Location = location;
                    job.JobType = jobType;
                    job.SalaryMin = salaryMin;
                    job.SalaryMax = salaryMax;
                    job.ExperienceRequired = expReq;
                    
                    if (statusStr.ToLower() == "approved" || statusStr.ToLower() == "active")
                        job.Status = JobStatus.APPROVED;
                    else if (statusStr.ToLower() == "rejected")
                        job.Status = JobStatus.REJECTED;
                    else if (statusStr.ToLower() == "closed")
                        job.Status = JobStatus.CLOSED;
                    else
                        job.Status = JobStatus.PENDING_APPROVAL;
                    
                    job.IsActive = (job.Status == JobStatus.APPROVED);
                    job.TotalApplications = totalApps;
                    job.ExpiryDate = expiryDate;
                    job.PostedDate = postedDate;
                    job.UpdatedAt = updatedAt;
                    job.NumberOfRounds = numRounds;
                    
                    decimal min = salaryMin ?? 1;
                    decimal max = salaryMax ?? 1;
                    if (min <= 0) min = 1;
                    if (max < min) max = min;
                    job.SalaryRange = new SalaryRange(min, max);
                    
                    job.RequiredSkills = new List<string>();
                    
                    jobs.Add(job);
                }
            }
            
            // Populate skills for Jobs
            foreach (var job in jobs)
            {
                if (int.TryParse(job.JobId, out int jobId))
                {
                    using var cmd = new SqlCommand(@"
                        SELECT s.SkillName
                        FROM JPNS.JobSkills js
                        JOIN JPNS.Skills s ON js.SkillId = s.SkillId
                        WHERE js.JobId = @JobId", conn);
                    cmd.Parameters.AddWithValue("@JobId", jobId);
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        job.RequiredSkills.Add(reader["SkillName"].ToString()!);
                    }
                }
            }
            
            // 4. Load Applications
            var apps = new List<Application1>();
            using (var cmd = new SqlCommand(@"
                SELECT ApplicationId, UserId, JobId, ResumeId, ResumeSnapshot, CoverLetter, Status, AppliedDate
                FROM JPNS.Applications", conn))
            {
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int appId = (int)reader["ApplicationId"];
                    int userId = (int)reader["UserId"];
                    int jobId = (int)reader["JobId"];
                    string resumeSnapshot = reader["ResumeSnapshot"] == DBNull.Value ? "" : reader["ResumeSnapshot"].ToString()!;
                    string coverLetter = reader["CoverLetter"] == DBNull.Value ? "" : reader["CoverLetter"].ToString()!;
                    string statusStr = reader["Status"].ToString()!;
                    DateTime appliedDate = (DateTime)reader["AppliedDate"];
                    
                    var app = new Application1();
                    app.ApplicationId = appId.ToString();
                    app.JobSeekerId = "JSK-2026-" + userId.ToString("D4");
                    app.JobId = jobId.ToString();
                    app.ResumeSnapshot = resumeSnapshot;
                    app.CoverLetter = coverLetter;
                    app.AppliedDate = appliedDate;
                    
                    if (Enum.TryParse<ApplicationStatus>(statusStr, true, out var appStatus))
                    {
                        app.Status = appStatus;
                    }
                    else
                    {
                        app.Status = ApplicationStatus.PENDING;
                    }
                    
                    apps.Add(app);
                }
            }
            
            // 5. Load Interviews
            var interviews = new List<Interview1>();
            using (var cmd = new SqlCommand(@"
                SELECT i.InterviewId, i.ApplicationId, i.ScheduledAt, i.Mode, i.MeetingLink, i.Venue, i.Status, i.CreatedAt,
                       a.UserId, a.JobId
                FROM JPNS.Interviews i
                JOIN JPNS.Applications a ON i.ApplicationId = a.ApplicationId", conn))
            {
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int interviewId = (int)reader["InterviewId"];
                    int dbAppId = (int)reader["ApplicationId"];
                    DateTime scheduledAt = (DateTime)reader["ScheduledAt"];
                    string mode = reader["Mode"].ToString()!;
                    string meetingLink = reader["MeetingLink"] == DBNull.Value ? "" : reader["MeetingLink"].ToString()!;
                    string venue = reader["Venue"] == DBNull.Value ? "" : reader["Venue"].ToString()!;
                    string statusStr = reader["Status"].ToString()!;
                    DateTime createdAt = (DateTime)reader["CreatedAt"];
                    
                    var iv = new Interview1();
                    iv.InterviewId = interviewId.ToString();
                    iv.ApplicationId = dbAppId.ToString();
                    iv.ScheduledAt = scheduledAt;
                    iv.Mode = mode;
                    iv.MeetingLink = meetingLink;
                    iv.Venue = venue;
                    iv.CreatedAt = createdAt;
                    
                    if (Enum.TryParse<InterviewStatus>(statusStr, true, out var ivStatus))
                    {
                        iv.Status = ivStatus;
                    }
                    else
                    {
                        iv.Status = InterviewStatus.SCHEDULED;
                    }
                    
                    interviews.Add(iv);
                }
            }
            
            // 6. Load Notifications
            var notifications = new List<Notification>();
            using (var cmd = new SqlCommand(@"
                SELECT n.NotificationId, n.UserId, n.Message, n.NotificationType, n.IsRead, n.CreatedAt,
                       u.Role, u.Email
                FROM JPNS.Notifications n
                JOIN JPNS.Users u ON n.UserId = u.UserId", conn))
            {
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int notifId = (int)reader["NotificationId"];
                    int userId = (int)reader["UserId"];
                    string message = reader["Message"].ToString()!;
                    string notifType = reader["NotificationType"].ToString()!;
                    bool isRead = (bool)reader["IsRead"];
                    DateTime createdAt = (DateTime)reader["CreatedAt"];
                    string role = reader["Role"].ToString()!;
                    string email = reader["Email"].ToString()!;

                    string mappedUserId = "";
                    if (role.Equals("admin", StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (var admin in admins)
                        {
                            if (admin.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                            {
                                mappedUserId = admin.AdminId;
                                break;
                            }
                        }
                        if (string.IsNullOrEmpty(mappedUserId))
                        {
                            mappedUserId = "ADM" + userId.ToString("D3");
                        }
                    }
                    else
                    {
                        mappedUserId = "JSK-2026-" + userId.ToString("D4");
                    }

                    var n = new Notification
                    {
                        UserId = mappedUserId,
                        Message = message,
                        Type = notifType,
                        IsRead = isRead,
                        CreatedAt = createdAt
                    };

                    if (message.StartsWith("[") && message.Contains("]"))
                    {
                        int endBracket = message.IndexOf(']');
                        n.Title = message.Substring(1, endBracket - 1);
                        n.Message = message.Substring(endBracket + 1).Trim();
                    }
                    else
                    {
                        n.Title = notifType;
                    }

                    notifications.Add(n);
                }
            }

            // 7. Load Complaints
            var complaints = new List<Complaint>();
            using (var cmd = new SqlCommand(@"
                SELECT ComplaintId, SubmittedByUserId, AgainstUserId, Subject, Description, Status, AdminNotes, CreatedAt, ResolvedAt
                FROM JPNS.Complaints", conn))
            {
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var c = new Complaint
                    {
                        ComplaintId = (int)reader["ComplaintId"],
                        SubmittedByUserId = (int)reader["SubmittedByUserId"],
                        AgainstUserId = reader["AgainstUserId"] == DBNull.Value ? null : (int?)reader["AgainstUserId"],
                        Subject = reader["Subject"].ToString()!,
                        Description = reader["Description"].ToString()!,
                        Status = reader["Status"].ToString()!,
                        AdminNotes = reader["AdminNotes"] == DBNull.Value ? null : reader["AdminNotes"].ToString(),
                        CreatedAt = (DateTime)reader["CreatedAt"],
                        ResolvedAt = reader["ResolvedAt"] == DBNull.Value ? null : (DateTime?)reader["ResolvedAt"]
                    };
                    complaints.Add(c);
                }
            }
            
            // Update application status to INTERVIEW_SCHEDULED if a scheduled interview exists
            foreach (var iv in interviews)
            {
                if (iv.Status == InterviewStatus.SCHEDULED)
                {
                    var matchingApp = apps.Find(a => a.ApplicationId == iv.ApplicationId);
                    if (matchingApp != null)
                    {
                        matchingApp.Status = ApplicationStatus.INTERVIEW_SCHEDULED;
                    }
                }
            }

            // Reconcile/Merge candidate status and interview progress for all seekers
            foreach (var seeker in seekers)
            {
                var seekerApps = new List<Application1>();
                foreach (var app in apps)
                {
                    if (app.JobSeekerId == seeker.SeekerId)
                    {
                        seekerApps.Add(app);
                    }
                }

                if (seekerApps.Count > 0)
                {
                    Application1? bestApp = null;
                    foreach (var app in seekerApps)
                    {
                        if (bestApp == null)
                        {
                            bestApp = app;
                        }
                        else
                        {
                            if (app.Status == ApplicationStatus.HIRED)
                            {
                                bestApp = app;
                            }
                            else if (app.Status == ApplicationStatus.INTERVIEW_SCHEDULED && bestApp.Status != ApplicationStatus.HIRED)
                            {
                                bestApp = app;
                            }
                            else if (app.Status == ApplicationStatus.SHORTLISTED && bestApp.Status != ApplicationStatus.HIRED && bestApp.Status != ApplicationStatus.INTERVIEW_SCHEDULED)
                            {
                                bestApp = app;
                            }
                            else if (app.Status == ApplicationStatus.PENDING && bestApp.Status != ApplicationStatus.HIRED && bestApp.Status != ApplicationStatus.INTERVIEW_SCHEDULED && bestApp.Status != ApplicationStatus.SHORTLISTED)
                            {
                                bestApp = app;
                            }
                        }
                    }

                    if (bestApp != null)
                    {
                        seeker.CandidateStatus = bestApp.Status.ToString();

                        int completedInterviews = 0;
                        foreach (var iv in interviews)
                        {
                            if (iv.ApplicationId == bestApp.ApplicationId && iv.Status == InterviewStatus.COMPLETED)
                            {
                                completedInterviews++;
                            }
                        }

                        if (bestApp.Status == ApplicationStatus.SHORTLISTED)
                        {
                            int shortlistRound = Math.Min(seeker.TotalRounds, completedInterviews + 1);
                            seeker.CurrentRound = Math.Max(seeker.CurrentRound, shortlistRound);
                            seeker.RejectedRound = null;
                        }
                        else if (bestApp.Status == ApplicationStatus.INTERVIEW_SCHEDULED)
                        {
                            int scheduleRound = Math.Min(seeker.TotalRounds, seeker.CurrentRound + 1);
                            seeker.CurrentRound = Math.Max(seeker.CurrentRound, scheduleRound);
                            seeker.RejectedRound = null;

                            // If rounds are maxed during interview scheduling, auto-promote to HIRED.
                            if (seeker.CurrentRound >= seeker.TotalRounds)
                                seeker.CandidateStatus = "HIRED";
                            else
                                seeker.CandidateStatus = "INTERVIEW_SCHEDULED";
                        }
                        else if (bestApp.Status == ApplicationStatus.HIRED)
                        {
                            seeker.CurrentRound = seeker.TotalRounds;
                            seeker.RejectedRound = null;
                        }
                        else if (bestApp.Status == ApplicationStatus.REJECTED)
                        {
                            seeker.CurrentRound = completedInterviews;
                            seeker.RejectedRound = completedInterviews;
                        }
                        else if (bestApp.Status == ApplicationStatus.PENDING)
                        {
                            seeker.CurrentRound = 0;
                            seeker.RejectedRound = null;
                        }
                    }
                }
                else
                {
                    seeker.CandidateStatus = seeker.IsActive ? "ACTIVE" : "BLOCKED";
                    seeker.CurrentRound = 0;
                    seeker.RejectedRound = null;
                }
            }

            // Apply loaded data to DataStore
            FileStorage.DataStore.JobSeekers.Clear();
            FileStorage.DataStore.JobSeekers.AddRange(seekers);
            
            FileStorage.DataStore.Admins.Clear();
            FileStorage.DataStore.Admins.AddRange(admins);
            
            FileStorage.DataStore.Jobs.Clear();
            FileStorage.DataStore.Jobs.AddRange(jobs);
            
            FileStorage.DataStore.Applications.Clear();
            FileStorage.DataStore.Applications.AddRange(apps);
            
            FileStorage.DataStore.Interviews.Clear();
            FileStorage.DataStore.Interviews.AddRange(interviews);
            
            FileStorage.DataStore.Notifications.Clear();
            FileStorage.DataStore.Notifications.AddRange(notifications);

            FileStorage.DataStore.Complaints.Clear();
            FileStorage.DataStore.Complaints.AddRange(complaints);
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  [DB] Hydration complete. Loaded all datasets directly from SQL Server.");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  [DB ERROR] LoadAllFromDb Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    private static async Task SyncProjectsAsync(List<Project> projects, int userId, SqlConnection conn)
    {
        await using (var deleteCmd = new SqlCommand("DELETE FROM JPNS.Projects WHERE UserId = @UserId", conn))
        {
            deleteCmd.Parameters.AddWithValue("@UserId", userId);
            await deleteCmd.ExecuteNonQueryAsync();
        }

        foreach (var p in projects)
        {
            string sql = @"
                INSERT INTO JPNS.Projects (UserId, Title, Description, Technologies, ProjectUrl, StartDate, EndDate)
                VALUES (@UserId, @Title, @Description, @Technologies, @ProjectUrl, @StartDate, @EndDate)";
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Title", p.Title);
            cmd.Parameters.AddWithValue("@Description", (object?)p.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Technologies", (object?)p.Technologies ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ProjectUrl", (object?)p.ProjectUrl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@StartDate", p.StartDate == default ? DateTime.Now : p.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", (object?)p.EndDate ?? DBNull.Value);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    private static async Task SyncCertificatesAsync(List<Certificate> certificates, int userId, SqlConnection conn)
    {
        await using (var deleteCmd = new SqlCommand("DELETE FROM JPNS.Certificates WHERE UserId = @UserId", conn))
        {
            deleteCmd.Parameters.AddWithValue("@UserId", userId);
            await deleteCmd.ExecuteNonQueryAsync();
        }

        foreach (var c in certificates)
        {
            string sql = @"
                INSERT INTO JPNS.Certificates (UserId, CertName, IssuingOrg, IssueDate, ExpiryDate, CertificateUrl, CredentialId)
                VALUES (@UserId, @CertName, @IssuingOrg, @IssueDate, @ExpiryDate, @CertificateUrl, @CredentialId)";
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@CertName", c.CertName);
            cmd.Parameters.AddWithValue("@IssuingOrg", (object?)c.IssuingOrg ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IssueDate", c.IssueDate == default ? DateTime.Now : c.IssueDate);
            cmd.Parameters.AddWithValue("@ExpiryDate", (object?)c.ExpiryDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CertificateUrl", (object?)c.CertificateUrl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CredentialId", (object?)c.CredentialId ?? DBNull.Value);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    private static async Task SyncEducationAsync(List<Education> educationList, int userId, SqlConnection conn)
    {
        await using (var deleteCmd = new SqlCommand("DELETE FROM JPNS.Education WHERE UserId = @UserId", conn))
        {
            deleteCmd.Parameters.AddWithValue("@UserId", userId);
            await deleteCmd.ExecuteNonQueryAsync();
        }

        foreach (var edu in educationList)
        {
            string sql = @"
                INSERT INTO JPNS.Education (UserId, Degree, Institution, FieldOfStudy, StartYear, EndYear, Grade, Description)
                VALUES (@UserId, @Degree, @Institution, @FieldOfStudy, @StartYear, @EndYear, @Grade, @Description)";
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Degree", edu.Degree);
            cmd.Parameters.AddWithValue("@Institution", edu.Institution);
            cmd.Parameters.AddWithValue("@FieldOfStudy", "Computer Science");
            cmd.Parameters.AddWithValue("@StartYear", DateTime.Now.Year - 4);
            cmd.Parameters.AddWithValue("@EndYear", DateTime.Now.Year);
            cmd.Parameters.AddWithValue("@Grade", (object?)edu.Grade ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Description", DBNull.Value);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    private static async Task SyncWorkExperienceAsync(List<WorkExperience> experienceList, int userId, SqlConnection conn)
    {
        await using (var deleteCmd = new SqlCommand("DELETE FROM JPNS.WorkExperience WHERE UserId = @UserId", conn))
        {
            deleteCmd.Parameters.AddWithValue("@UserId", userId);
            await deleteCmd.ExecuteNonQueryAsync();
        }

        foreach (var exp in experienceList)
        {
            string sql = @"
                INSERT INTO JPNS.WorkExperience (UserId, JobTitle, Company, Description, StartDate, EndDate, IsCurrent)
                VALUES (@UserId, @JobTitle, @Company, @Description, @StartDate, @EndDate, @IsCurrent)";
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@JobTitle", exp.JobTitle);
            cmd.Parameters.AddWithValue("@Company", exp.Company);
            cmd.Parameters.AddWithValue("@Description", DBNull.Value);
            cmd.Parameters.AddWithValue("@StartDate", exp.StartDate == default ? DateTime.Now.AddYears(-1) : exp.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", (object?)exp.EndDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsCurrent", exp.IsCurrent);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    private static async Task SyncCandidateSkillsAsync(List<CandidateSkill> skillsList, int userId, SqlConnection conn)
    {
        await using (var deleteCmd = new SqlCommand("DELETE FROM JPNS.CandidateSkills WHERE UserId = @UserId", conn))
        {
            deleteCmd.Parameters.AddWithValue("@UserId", userId);
            await deleteCmd.ExecuteNonQueryAsync();
        }

        foreach (var skill in skillsList)
        {
            int skillId = 0;
            using (var skillCmd = new SqlCommand(@"
                IF NOT EXISTS (SELECT 1 FROM JPNS.Skills WHERE SkillName = @SkillName)
                    INSERT INTO JPNS.Skills (SkillName) VALUES (@SkillName);
                SELECT SkillId FROM JPNS.Skills WHERE SkillName = @SkillName;", conn))
            {
                skillCmd.Parameters.AddWithValue("@SkillName", skill.SkillName.Trim());
                object? skillResult = await skillCmd.ExecuteScalarAsync();
                if (skillResult != null && skillResult != DBNull.Value)
                {
                    skillId = Convert.ToInt32(skillResult);
                }
            }

            if (skillId > 0)
            {
                string sql = @"
                    INSERT INTO JPNS.CandidateSkills (UserId, SkillId, ProficiencyLevel, YearsExperience)
                    VALUES (@UserId, @SkillId, @ProficiencyLevel, @YearsExperience)";
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@SkillId", skillId);
                cmd.Parameters.AddWithValue("@ProficiencyLevel", skill.ProficiencyLevel ?? "Intermediate");
                cmd.Parameters.AddWithValue("@YearsExperience", (object?)skill.YearsExperience ?? DBNull.Value);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    private static async Task SyncFeedbackAsync(List<Feedback> feedbacks, int userId, SqlConnection conn)
    {
        await using (var deleteCmd = new SqlCommand("DELETE FROM JPNS.Feedback WHERE UserId = @UserId", conn))
        {
            deleteCmd.Parameters.AddWithValue("@UserId", userId);
            await deleteCmd.ExecuteNonQueryAsync();
        }

        foreach (var fb in feedbacks)
        {
            int companyId = 0;
            using (var compCmd = new SqlCommand("SELECT CompanyId FROM JPNS.CompanyProfiles WHERE CompanyName = @CompanyName", conn))
            {
                compCmd.Parameters.AddWithValue("@CompanyName", fb.CompanyName.Trim());
                object? result = await compCmd.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                {
                    companyId = Convert.ToInt32(result);
                }
                else
                {
                    using (var adminCmd = new SqlCommand("SELECT TOP 1 UserId FROM JPNS.Users WHERE Role = 'admin'", conn))
                    {
                        object? adminObj = await adminCmd.ExecuteScalarAsync();
                        if (adminObj != null && adminObj != DBNull.Value)
                        {
                            int adminUserId = Convert.ToInt32(adminObj);
                            using (var insertCompCmd = new SqlCommand(@"
                                INSERT INTO JPNS.CompanyProfiles (UserId, CompanyName, IsVerified)
                                VALUES (@UserId, @CompanyName, 1);
                                SELECT SCOPE_IDENTITY();", conn))
                            {
                                insertCompCmd.Parameters.AddWithValue("@UserId", adminUserId);
                                insertCompCmd.Parameters.AddWithValue("@CompanyName", fb.CompanyName.Trim());
                                object? newCompId = await insertCompCmd.ExecuteScalarAsync();
                                if (newCompId != null && newCompId != DBNull.Value)
                                {
                                    companyId = Convert.ToInt32(newCompId);
                                }
                            }
                        }
                    }
                }
            }

            if (companyId > 0)
            {
                string sql = @"
                    INSERT INTO JPNS.Feedback (UserId, CompanyId, Rating, Review, IsAnonymous, CreatedAt)
                    VALUES (@UserId, @CompanyId, @Rating, @Review, @IsAnonymous, @CreatedAt)";
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@CompanyId", companyId);
                cmd.Parameters.AddWithValue("@Rating", fb.Rating);
                cmd.Parameters.AddWithValue("@Review", (object?)fb.Review ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsAnonymous", fb.IsAnonymous);
                cmd.Parameters.AddWithValue("@CreatedAt", fb.CreatedAt == default ? DateTime.Now : fb.CreatedAt);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    private static string MapJobSeekerIdToEmail(string oldId)
    {
        return oldId.ToUpper().Trim() switch
        {
            "JSK-2026-0001" => "arun.kumar@example.com",
            "JSK-2026-0002" => "priya.sharma@example.com",
            "JSK-2026-0003" => "vikram.rajan@example.com",
            "JSK-2026-0004" => "divya.menon@example.com",
            "JSK-2026-0005" => "suresh.babu@example.com",
            "JSK-2026-0006" => "meena.krishnan@example.com",
            "JSK-2026-0007" => "arjun.pillai@example.com",
            "JSK-2026-0008" => "kavitha.devi@example.com",
            "JSK-2026-0009" => "nikhil.reddy@example.com",
            "JSK-2026-0010" => "lakshmi.n@example.com",
            "JSK-2026-0011" => "harini@gmail.com",
            "JSK-2026-0012" => "sri@gmail.com",
            "JSK-2026-0013" => "harish@gmail.com",
            "JSK-2026-0014" => "maha@gmail.com",
            "JSK-2026-0015" => "kumar@gmail.com",
            "JSK-2026-0016" => "lakshmi@gmail.com",
            "JSK-2026-0017" => "harinisri@gmail.com",
            "JSK-2026-0019" => "gowtham@gmail.com",
            "JSK-2026-0020" => "kirthik@gmail.com",
            "JSK-2026-0021" => "kirthik12@gmail.com",
            "JSK-2026-0022" => "kirthik123@gmail.com",
            "JSK-2026-0023" => "kirthikkumar@gmail.com",
            _ => ""
        };
    }

    private static string GetEmailForSeekerId(string seekerId)
    {
        string mappedEmail = MapJobSeekerIdToEmail(seekerId);
        if (!string.IsNullOrEmpty(mappedEmail))
            return mappedEmail;

        var seeker = FileStorage.DataStore.FindJobSeekerById(seekerId);
        if (seeker != null)
            return seeker.Email;

        string[] parts = seekerId.Split('-');
        if (parts.Length == 3 && int.TryParse(parts[2], out int idNum))
        {
            if (idNum < 100)
            {
                string oldFormatId = $"JSK-2026-{idNum:D4}";
                mappedEmail = MapJobSeekerIdToEmail(oldFormatId);
                if (!string.IsNullOrEmpty(mappedEmail))
                    return mappedEmail;
            }
        }
        return "";
    }

    public static bool IsSeedingRequired()
    {
        if (!_enabled) return false;
        try
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            
            using var cmd1 = new SqlCommand("SELECT COUNT(*) FROM JPNS.Applications", conn);
            int count1 = (int)cmd1.ExecuteScalar()!;
            
            using var cmd2 = new SqlCommand("SELECT COUNT(*) FROM JPNS.Interviews", conn);
            int count2 = (int)cmd2.ExecuteScalar()!;

            using var cmd3 = new SqlCommand("SELECT COUNT(*) FROM JPNS.CandidateSkills", conn);
            int count3 = (int)cmd3.ExecuteScalar()!;

            using var cmd4 = new SqlCommand("SELECT COUNT(*) FROM JPNS.WorkExperience", conn);
            int count4 = (int)cmd4.ExecuteScalar()!;
            
            return count1 == 0 || count2 == 0 || count3 == 0 || count4 == 0;
        }
        catch
        {
            return false;
        }
    }

    public static void SeedDatabaseSynchronously(
        List<Admin1> admins,
        List<JobSeeker> seekers,
        List<JobListing> jobs,
        List<Application1> applications,
        List<Interview1> interviews,
        List<Notification> notifications)
    {
        if (!_enabled) return;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  [DB] Seeding DB tables from JSON starting...");
        Console.ResetColor();

        foreach (var admin in admins)
        {
            try
            {
                UpsertAdminAsync(admin).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [DB ERROR] Seeding Admin '{admin.Email}': {ex.Message}");
            }
        }

        foreach (var seeker in seekers)
        {
            try
            {
                UpsertJobSeekerAsync(seeker).GetAwaiter().GetResult();
                UpsertResumeAsync(seeker).GetAwaiter().GetResult();
                SyncSavedJobsForUser(seeker).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [DB ERROR] Seeding Seeker '{seeker.Email}': {ex.Message}");
            }
        }

        foreach (var job in jobs)
        {
            try
            {
                UpsertJobListingAsync(job).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [DB ERROR] Seeding Job '{job.Title}': {ex.Message}");
            }
        }

        foreach (var app in applications)
        {
            try
            {
                UpsertApplicationAsync(app).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [DB ERROR] Seeding Application '{app.ApplicationId}': {ex.Message}");
            }
        }

        foreach (var interview in interviews)
        {
            try
            {
                UpsertInterviewAsync(interview).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [DB ERROR] Seeding Interview '{interview.InterviewId}': {ex.Message}");
            }
        }

        foreach (var notif in notifications)
        {
            try
            {
                UpsertNotificationAsync(notif).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [DB ERROR] Seeding Notification: {ex.Message}");
            }
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("  [DB] Database seeding completed successfully.");
        Console.ResetColor();
    }
}
