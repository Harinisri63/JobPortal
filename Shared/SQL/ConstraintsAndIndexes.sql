-- ============================================================
-- JPNS — Job Portal Navigation System
-- Script 03: All 8 Stored Procedures
-- ============================================================
-- Run after Database.sql and SeedData.sql
-- ============================================================

-- ── usp_RegisterUser ─────────────────────────────────────────────────────────
GO
CREATE OR ALTER PROCEDURE JPNS.usp_RegisterUser
    @FullName       NVARCHAR(150),
    @Email          NVARCHAR(120),
    @Username       NVARCHAR(80),
    @PasswordHash   NVARCHAR(255),
    @Salt           NVARCHAR(64),
    @MobileNumber   NVARCHAR(15) = NULL,
    @Role           NVARCHAR(20) = 'jobseeker',
    @UserId         INT OUTPUT
AS BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM JPNS.Users WHERE Email = LOWER(@Email))
            RAISERROR('Email already registered.', 16, 1);

        IF EXISTS (SELECT 1 FROM JPNS.Users WHERE Username = LOWER(@Username))
            RAISERROR('Username already taken.', 16, 1);

        IF @Role NOT IN ('jobseeker', 'admin')
            RAISERROR('Invalid role. Must be jobseeker or admin.', 16, 1);

        INSERT INTO JPNS.Users (FullName, Email, Username, PasswordHash, Salt, MobileNumber, Role)
        VALUES (@FullName, LOWER(@Email), LOWER(@Username), @PasswordHash, @Salt, @MobileNumber, @Role);

        SET @UserId = SCOPE_IDENTITY();
        SELECT @UserId AS UserId;
    END TRY
    BEGIN CATCH THROW; END CATCH
END;
GO

-- ── usp_LoginUser ────────────────────────────────────────────────────────────
CREATE OR ALTER PROCEDURE JPNS.usp_LoginUser
    @EmailOrMobile  NVARCHAR(120),
    @PasswordHash   NVARCHAR(255)
AS BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @UserId INT, @IsLocked BIT, @LockedUntil DATETIME2,
                @StoredHash NVARCHAR(255), @IsActive BIT,
                @FailedAtt INT, @IsEmailVerified BIT;

        SELECT @UserId = UserId, @IsLocked = IsLocked, @LockedUntil = LockedUntil,
               @StoredHash = PasswordHash, @IsActive = IsActive,
               @FailedAtt = FailedLoginAttempts, @IsEmailVerified = IsEmailVerified
        FROM JPNS.Users
        WHERE Email = LOWER(@EmailOrMobile) OR MobileNumber = @EmailOrMobile;

        IF @UserId IS NULL
            RAISERROR('Invalid login credentials.', 16, 1);

        IF @IsActive = 0
            RAISERROR('Account deactivated. Contact admin.', 16, 1);

        IF @IsLocked = 1 AND (@LockedUntil IS NULL OR @LockedUntil > GETDATE())
            RAISERROR('Account temporarily locked. Try again later.', 16, 1);

        IF @StoredHash <> @PasswordHash
        BEGIN
            UPDATE JPNS.Users
            SET FailedLoginAttempts = FailedLoginAttempts + 1,
                IsLocked = CASE WHEN FailedLoginAttempts + 1 >= 5 THEN 1 ELSE 0 END,
                LockedUntil = CASE WHEN FailedLoginAttempts + 1 >= 5
                                   THEN DATEADD(MINUTE, 30, GETDATE()) ELSE NULL END
            WHERE UserId = @UserId;
            RAISERROR('Invalid login credentials.', 16, 1);
        END

        UPDATE JPNS.Users
        SET FailedLoginAttempts = 0, IsLocked = 0, LockedUntil = NULL, LastLoginAt = GETDATE()
        WHERE UserId = @UserId;

        SELECT UserId, FullName, Role, IsEmailVerified, IsActive
        FROM JPNS.Users WHERE UserId = @UserId;
    END TRY
    BEGIN CATCH THROW; END CATCH
END;
GO

-- ── usp_PostJob ──────────────────────────────────────────────────────────────
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
                 SalaryMin, SalaryMax, ExperienceRequired, ExpiryDate)
            VALUES
                (@PostedByUserId, @CompanyId, @Title, @Description, @Location, @JobType,
                 @SalaryMin, @SalaryMax, @ExperienceRequired, @ExpiryDate);

            SET @JobId = SCOPE_IDENTITY();
        COMMIT TRANSACTION;

        SELECT @JobId AS JobId, 'pending' AS Status;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- ── usp_ApplyForJob ──────────────────────────────────────────────────────────
CREATE OR ALTER PROCEDURE JPNS.usp_ApplyForJob
    @UserId         INT,
    @JobId          INT,
    @ResumeId       INT,
    @CoverLetter    NVARCHAR(MAX) = NULL,
    @ApplicationId  INT OUTPUT
AS BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM JPNS.Applications WHERE UserId = @UserId AND JobId = @JobId)
            RAISERROR('You have already applied for this job.', 16, 1);

        DECLARE @ExpiryDate DATETIME2, @Status NVARCHAR(20);
        SELECT @ExpiryDate = ExpiryDate, @Status = Status FROM JPNS.JobListings WHERE JobId = @JobId;

        IF @ExpiryDate IS NULL RAISERROR('Job not found.', 16, 1);
        IF @ExpiryDate < GETDATE() RAISERROR('This job posting has expired.', 16, 1);
        IF @Status <> 'approved' RAISERROR('This job is not available for applications.', 16, 1);

        DECLARE @ResumeUrl NVARCHAR(500);
        SELECT @ResumeUrl = FileUrl FROM JPNS.Resumes WHERE ResumeId = @ResumeId AND UserId = @UserId;

        IF @ResumeUrl IS NULL
            RAISERROR('Resume not found. Please upload your resume first.', 16, 1);

        BEGIN TRANSACTION;
            INSERT INTO JPNS.Applications (UserId, JobId, ResumeId, ResumeSnapshot, CoverLetter)
            VALUES (@UserId, @JobId, @ResumeId, @ResumeUrl, @CoverLetter);

            SET @ApplicationId = SCOPE_IDENTITY();

            UPDATE JPNS.JobListings
            SET TotalApplications = TotalApplications + 1
            WHERE JobId = @JobId;
        COMMIT TRANSACTION;

        SELECT @ApplicationId AS ApplicationId, 'PENDING' AS Status;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- ── usp_UpdateApplicationStatus ──────────────────────────────────────────────
CREATE OR ALTER PROCEDURE JPNS.usp_UpdateApplicationStatus
    @ApplicationId   INT,
    @NewStatus       NVARCHAR(20),
    @UpdatedByUserId INT
AS BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF @NewStatus NOT IN ('PENDING','SHORTLISTED','REJECTED','HIRED')
            RAISERROR('Invalid application status.', 16, 1);

        DECLARE @CandidateUserId INT, @JobTitle NVARCHAR(200);
        SELECT @CandidateUserId = a.UserId, @JobTitle = j.Title
        FROM JPNS.Applications a
        INNER JOIN JPNS.JobListings j ON a.JobId = j.JobId
        WHERE a.ApplicationId = @ApplicationId;

        IF @CandidateUserId IS NULL RAISERROR('Application not found.', 16, 1);

        BEGIN TRANSACTION;
            UPDATE JPNS.Applications
            SET Status = @NewStatus, UpdatedAt = GETDATE()
            WHERE ApplicationId = @ApplicationId;

            INSERT INTO JPNS.Notifications (UserId, Message, NotificationType, ReferenceId)
            VALUES (@CandidateUserId,
                    'Your application for ' + @JobTitle + ' is now ' + @NewStatus + '.',
                    'APPLICATION_STATUS', @ApplicationId);
        COMMIT TRANSACTION;

        SELECT @ApplicationId AS ApplicationId, @NewStatus AS NewStatus;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- ── usp_ScheduleInterview ────────────────────────────────────────────────────
CREATE OR ALTER PROCEDURE JPNS.usp_ScheduleInterview
    @ApplicationId      INT,
    @ScheduledByUserId  INT,
    @ScheduledAt        DATETIME2,
    @Mode               NVARCHAR(20),
    @MeetingLink        NVARCHAR(500) = NULL,
    @Venue              NVARCHAR(300) = NULL,
    @InterviewId        INT OUTPUT
AS BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF @ScheduledAt <= GETDATE()
            RAISERROR('Interview must be scheduled for a future date.', 16, 1);

        IF @Mode NOT IN ('Online', 'Offline')
            RAISERROR('Interview mode must be Online or Offline.', 16, 1);

        IF @Mode = 'Online' AND (@MeetingLink IS NULL OR LEN(@MeetingLink) = 0)
            RAISERROR('Meeting link is required for online interviews.', 16, 1);

        DECLARE @CandidateUserId INT;
        SELECT @CandidateUserId = UserId FROM JPNS.Applications WHERE ApplicationId = @ApplicationId;

        IF @CandidateUserId IS NULL RAISERROR('Application not found.', 16, 1);

        BEGIN TRANSACTION;
            INSERT INTO JPNS.Interviews
                (ApplicationId, ScheduledByUserId, ScheduledAt, Mode, MeetingLink, Venue)
            VALUES
                (@ApplicationId, @ScheduledByUserId, @ScheduledAt, @Mode, @MeetingLink, @Venue);

            SET @InterviewId = SCOPE_IDENTITY();

            INSERT INTO JPNS.Notifications (UserId, Message, NotificationType, ReferenceId)
            VALUES (@CandidateUserId,
                    'Interview scheduled for ' + CONVERT(NVARCHAR, @ScheduledAt, 120) + ' (' + @Mode + ').',
                    'INTERVIEW_INVITE', @InterviewId);
        COMMIT TRANSACTION;

        SELECT @InterviewId AS InterviewId, 'SCHEDULED' AS Status;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- ── usp_AdminApproveJob ──────────────────────────────────────────────────────
CREATE OR ALTER PROCEDURE JPNS.usp_AdminApproveJob
    @JobId       INT,
    @AdminUserId INT,
    @Decision    NVARCHAR(10)
AS BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF @Decision NOT IN ('approved', 'rejected')
            RAISERROR('Decision must be ''approved'' or ''rejected''.', 16, 1);

        DECLARE @PostedByUserId INT, @JobTitle NVARCHAR(200);
        SELECT @PostedByUserId = PostedByUserId, @JobTitle = Title
        FROM JPNS.JobListings WHERE JobId = @JobId;

        IF @PostedByUserId IS NULL RAISERROR('Job listing not found.', 16, 1);

        BEGIN TRANSACTION;
            UPDATE JPNS.JobListings
            SET Status = @Decision, UpdatedAt = GETDATE()
            WHERE JobId = @JobId;

            INSERT INTO JPNS.Notifications (UserId, Message, NotificationType, ReferenceId)
            VALUES (@PostedByUserId,
                    'Your job posting [' + @JobTitle + '] has been ' + @Decision + ' by admin.',
                    'SYSTEM', @JobId);
        COMMIT TRANSACTION;

        SELECT @JobId AS JobId, @Decision AS Status;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- ── usp_SendNotification ─────────────────────────────────────────────────────
CREATE OR ALTER PROCEDURE JPNS.usp_SendNotification
    @UserId             INT,
    @Message            NVARCHAR(MAX),
    @NotificationType   NVARCHAR(50) = 'SYSTEM',
    @ReferenceId        INT = NULL
AS BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM JPNS.Users WHERE UserId = @UserId AND IsActive = 1)
            RAISERROR('User not found or inactive.', 16, 1);

        INSERT INTO JPNS.Notifications (UserId, Message, NotificationType, ReferenceId)
        VALUES (@UserId, @Message, @NotificationType, @ReferenceId);

        SELECT SCOPE_IDENTITY() AS NotificationId;
    END TRY
    BEGIN CATCH THROW; END CATCH
END;
GO

-- ── Supplementary stored procedures ──────────────────────────────────────────
CREATE OR ALTER PROCEDURE JPNS.usp_GetUserById @UserId INT AS BEGIN
    SET NOCOUNT ON;
    SELECT UserId, FullName, Email, Username, Role, IsActive, IsEmailVerified,
           IsLocked, LockedUntil, FailedLoginAttempts, LastLoginAt, CreatedAt, UpdatedAt
    FROM JPNS.Users WHERE UserId = @UserId;
END;
GO

CREATE OR ALTER PROCEDURE JPNS.usp_SetUserActive @UserId INT, @IsActive BIT AS BEGIN
    SET NOCOUNT ON;
    UPDATE JPNS.Users SET IsActive = @IsActive, UpdatedAt = GETDATE() WHERE UserId = @UserId;
END;
GO

CREATE OR ALTER PROCEDURE JPNS.usp_SearchJobs
    @Keyword   NVARCHAR(200) = NULL,
    @SalaryMin DECIMAL(12,2) = NULL,
    @SalaryMax DECIMAL(12,2) = NULL
AS BEGIN
    SET NOCOUNT ON;
    SELECT j.JobId, j.Title, j.Location, j.JobType, j.SalaryMin, j.SalaryMax,
           j.ExperienceRequired, j.ExpiryDate, j.TotalApplications, j.Status,
           c.CompanyName
    FROM JPNS.JobListings j
    JOIN JPNS.CompanyProfiles c ON j.CompanyId = c.CompanyId
    WHERE j.Status = 'approved' AND j.ExpiryDate > GETDATE()
      AND (@Keyword IS NULL OR j.Title LIKE '%' + @Keyword + '%'
                            OR j.Description LIKE '%' + @Keyword + '%')
      AND (@SalaryMin IS NULL OR j.SalaryMax >= @SalaryMin)
      AND (@SalaryMax IS NULL OR j.SalaryMin <= @SalaryMax)
    ORDER BY j.PostedDate DESC;
END;
GO

CREATE OR ALTER PROCEDURE JPNS.usp_GetPendingJobs AS BEGIN
    SET NOCOUNT ON;
    SELECT j.JobId, j.Title, j.Location, j.JobType, j.SalaryMin, j.SalaryMax,
           j.ExpiryDate, j.PostedDate, c.CompanyName
    FROM JPNS.JobListings j
    JOIN JPNS.CompanyProfiles c ON j.CompanyId = c.CompanyId
    WHERE j.Status = 'pending'
    ORDER BY j.PostedDate ASC;
END;
GO

CREATE OR ALTER PROCEDURE JPNS.usp_GetApplicationsByUser @UserId INT AS BEGIN
    SET NOCOUNT ON;
    SELECT a.ApplicationId, a.JobId, a.Status, a.AppliedDate, a.UpdatedAt,
           j.Title AS JobTitle, c.CompanyName
    FROM JPNS.Applications a
    JOIN JPNS.JobListings j     ON a.JobId    = j.JobId
    JOIN JPNS.CompanyProfiles c ON j.CompanyId = c.CompanyId
    WHERE a.UserId = @UserId
    ORDER BY a.AppliedDate DESC;
END;
GO

CREATE OR ALTER PROCEDURE JPNS.usp_GetApplicationsByJob @JobId INT AS BEGIN
    SET NOCOUNT ON;
    SELECT a.ApplicationId, a.UserId, a.Status, a.AppliedDate, a.CoverLetter,
           u.FullName, u.Email
    FROM JPNS.Applications a
    JOIN JPNS.Users u ON a.UserId = u.UserId
    WHERE a.JobId = @JobId
    ORDER BY a.AppliedDate ASC;
END;
GO

CREATE OR ALTER PROCEDURE JPNS.usp_GetInterviewsByApplication @ApplicationId INT AS BEGIN
    SET NOCOUNT ON;
    SELECT InterviewId, ScheduledAt, Mode, MeetingLink, Venue, Status, Feedback, CreatedAt
    FROM JPNS.Interviews
    WHERE ApplicationId = @ApplicationId
    ORDER BY ScheduledAt ASC;
END;
GO

CREATE OR ALTER PROCEDURE JPNS.usp_GetNotifications
    @UserId     INT,
    @UnreadOnly BIT = 0
AS BEGIN
    SET NOCOUNT ON;
    SELECT NotificationId, Message, NotificationType, ReferenceId, IsRead, CreatedAt
    FROM JPNS.Notifications
    WHERE UserId = @UserId AND (@UnreadOnly = 0 OR IsRead = 0)
    ORDER BY CreatedAt DESC;
END;
GO

CREATE OR ALTER PROCEDURE JPNS.usp_MarkNotificationsRead @UserId INT AS BEGIN
    SET NOCOUNT ON;
    UPDATE JPNS.Notifications SET IsRead = 1 WHERE UserId = @UserId AND IsRead = 0;
END;
GO

PRINT 'All stored procedures created/updated successfully.';
