-- ============================================================
-- JPNS — Job Portal Navigation System
-- Script 01: Create Schema + All 18 Tables (Full DDL)
-- ============================================================
-- Run order: Database.sql → SeedData.sql → ConstraintsAndIndexes.sql
-- ============================================================

-- Create schema
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'JPNS')
    EXEC('CREATE SCHEMA JPNS');
GO

-- ── 1. Users ──────────────────────────────────────────────────────────────────
IF OBJECT_ID('JPNS.Users', 'U') IS NULL
BEGIN
    CREATE TABLE JPNS.Users (
        UserId               INT             IDENTITY(1,1) NOT NULL,
        FullName             NVARCHAR(150)   NOT NULL,
        Email                NVARCHAR(120)   NOT NULL,
        Username             NVARCHAR(80)    NOT NULL,
        PasswordHash         NVARCHAR(255)   NOT NULL,
        Salt                 NVARCHAR(64)    NOT NULL,
        MobileNumber         NVARCHAR(15)    NULL,
        Role                 NVARCHAR(20)    NOT NULL
                             CONSTRAINT DF_Users_Role    DEFAULT 'jobseeker'
                             CONSTRAINT CHK_Users_Role   CHECK (Role IN ('jobseeker', 'admin')),
        IsActive             BIT             NOT NULL CONSTRAINT DF_Users_IsActive      DEFAULT 1,
        IsEmailVerified      BIT             NOT NULL CONSTRAINT DF_Users_EmailVerified  DEFAULT 0,
        IsTwoFactorEnabled   BIT             NOT NULL CONSTRAINT DF_Users_2FA           DEFAULT 0,
        IsLocked             BIT             NOT NULL CONSTRAINT DF_Users_Locked        DEFAULT 0,
        LockedUntil          DATETIME2       NULL,
        FailedLoginAttempts  INT             NOT NULL CONSTRAINT DF_Users_FA            DEFAULT 0
                             CONSTRAINT CHK_Users_FA CHECK (FailedLoginAttempts >= 0),
        LastLoginAt          DATETIME2       NULL,
        CreatedAt            DATETIME2       NOT NULL CONSTRAINT DF_Users_CA            DEFAULT GETDATE(),
        UpdatedAt            DATETIME2       NOT NULL CONSTRAINT DF_Users_UA            DEFAULT GETDATE(),

        CONSTRAINT PK_Users        PRIMARY KEY (UserId),
        CONSTRAINT UQ_Users_Email  UNIQUE (Email),
        CONSTRAINT UQ_Users_Uname  UNIQUE (Username),
        CONSTRAINT CHK_Users_Email CHECK (Email LIKE '%@%.%')
    );

    CREATE INDEX IX_Users_Email ON JPNS.Users(Email);
    CREATE INDEX IX_Users_Role  ON JPNS.Users(Role);
    PRINT 'Table JPNS.Users created.';
END
GO

-- ── 2. JobSeekerProfiles ──────────────────────────────────────────────────────
IF OBJECT_ID('JPNS.JobSeekerProfiles', 'U') IS NULL
BEGIN
    CREATE TABLE JPNS.JobSeekerProfiles (
        ProfileId       INT             IDENTITY(1,1) NOT NULL,
        UserId          INT             NOT NULL,
        Headline        NVARCHAR(200)   NULL,
        About           NVARCHAR(MAX)   NULL,
        CurrentLocation NVARCHAR(150)   NULL,
        ExperienceYears INT             NOT NULL CONSTRAINT DF_JSP_ExpYrs DEFAULT 0
                        CONSTRAINT CHK_JSP_ExpYrs CHECK (ExperienceYears >= 0),
        LinkedInUrl     NVARCHAR(300)   NULL,
        PortfolioUrl    NVARCHAR(300)   NULL,
        UpdatedAt       DATETIME2       NOT NULL CONSTRAINT DF_JSP_UA DEFAULT GETDATE(),

        CONSTRAINT PK_JSP       PRIMARY KEY (ProfileId),
        CONSTRAINT FK_JSP_User  FOREIGN KEY (UserId) REFERENCES JPNS.Users(UserId) ON DELETE CASCADE,
        CONSTRAINT UQ_JSP_User  UNIQUE (UserId)
    );

    CREATE INDEX IX_JSP_UserId ON JPNS.JobSeekerProfiles(UserId);
    PRINT 'Table JPNS.JobSeekerProfiles created.';
END
GO

-- ── 3. Resumes ────────────────────────────────────────────────────────────────
IF OBJECT_ID('JPNS.Resumes', 'U') IS NULL
BEGIN
    CREATE TABLE JPNS.Resumes (
        ResumeId    INT             IDENTITY(1,1) NOT NULL,
        UserId      INT             NOT NULL,
        FileName    NVARCHAR(255)   NOT NULL,
        FileType    NVARCHAR(10)    NOT NULL
                    CONSTRAINT CHK_Res_Type CHECK (FileType IN ('PDF','DOC','MP4')),
        FileUrl     NVARCHAR(500)   NOT NULL,
        IsDefault   BIT             NOT NULL CONSTRAINT DF_Res_Default DEFAULT 0,
        UploadedAt  DATETIME2       NOT NULL CONSTRAINT DF_Res_UA DEFAULT GETDATE(),

        CONSTRAINT PK_Resumes   PRIMARY KEY (ResumeId),
        CONSTRAINT FK_Res_User  FOREIGN KEY (UserId) REFERENCES JPNS.Users(UserId) ON DELETE CASCADE
    );

    CREATE INDEX IX_Res_UserId ON JPNS.Resumes(UserId);
    PRINT 'Table JPNS.Resumes created.';
END
GO

-- ── 4. Skills ─────────────────────────────────────────────────────────────────
IF OBJECT_ID('JPNS.Skills', 'U') IS NULL
BEGIN
    CREATE TABLE JPNS.Skills (
        SkillId     INT             IDENTITY(1,1) NOT NULL,
        SkillName   NVARCHAR(100)   NOT NULL,
        Category    NVARCHAR(80)    NULL,

        CONSTRAINT PK_Skills      PRIMARY KEY (SkillId),
        CONSTRAINT UQ_Skills_Name UNIQUE (SkillName)
    );
    PRINT 'Table JPNS.Skills created.';
END
GO

-- ── 5. CandidateSkills ────────────────────────────────────────────────────────
IF OBJECT_ID('JPNS.CandidateSkills', 'U') IS NULL
BEGIN
    CREATE TABLE JPNS.CandidateSkills (
        CandidateSkillId INT          IDENTITY(1,1) NOT NULL,
        UserId           INT          NOT NULL,
        SkillId          INT          NOT NULL,
        ProficiencyLevel NVARCHAR(20) NOT NULL CONSTRAINT DF_CS_Prof DEFAULT 'Beginner'
                         CONSTRAINT CHK_CS_Prof CHECK (ProficiencyLevel IN ('Beginner','Intermediate','Expert')),
        YearsExperience  INT          NULL,

        CONSTRAINT PK_CandidateSkills  PRIMARY KEY (CandidateSkillId),
        CONSTRAINT FK_CS_User          FOREIGN KEY (UserId)  REFERENCES JPNS.Users(UserId)  ON DELETE CASCADE,
        CONSTRAINT FK_CS_Skill         FOREIGN KEY (SkillId) REFERENCES JPNS.Skills(SkillId),
        CONSTRAINT UQ_CS_UserSkill     UNIQUE (UserId, SkillId)
    );

    CREATE INDEX IX_CS_UserId  ON JPNS.CandidateSkills(UserId);
    CREATE INDEX IX_CS_SkillId ON JPNS.CandidateSkills(SkillId);
    PRINT 'Table JPNS.CandidateSkills created.';
END
GO

-- ── 6. Education ──────────────────────────────────────────────────────────────
IF OBJECT_ID('JPNS.Education', 'U') IS NULL
BEGIN
    CREATE TABLE JPNS.Education (
        EducationId   INT             IDENTITY(1,1) NOT NULL,
        UserId        INT             NOT NULL,
        Degree        NVARCHAR(150)   NOT NULL,
        Institution   NVARCHAR(200)   NOT NULL,
        FieldOfStudy  NVARCHAR(150)   NULL,
        StartYear     INT             NULL,
        EndYear       INT             NULL,
        Grade         NVARCHAR(20)    NULL,
        Description   NVARCHAR(MAX)   NULL,

        CONSTRAINT PK_Education PRIMARY KEY (EducationId),
        CONSTRAINT FK_Edu_User  FOREIGN KEY (UserId) REFERENCES JPNS.Users(UserId) ON DELETE CASCADE
    );

    CREATE INDEX IX_Edu_UserId ON JPNS.Education(UserId);
    PRINT 'Table JPNS.Education created.';
END
GO

-- ── 7. WorkExperience ─────────────────────────────────────────────────────────
IF OBJECT_ID('JPNS.WorkExperience', 'U') IS NULL
BEGIN
    CREATE TABLE JPNS.WorkExperience (
        ExperienceId  INT             IDENTITY(1,1) NOT NULL,
        UserId        INT             NOT NULL,
        JobTitle      NVARCHAR(150)   NOT NULL,
        Company       NVARCHAR(200)   NOT NULL,
        Location      NVARCHAR(150)   NULL,
        StartDate     DATE            NOT NULL,
        EndDate       DATE            NULL,
        IsCurrent     BIT             NOT NULL CONSTRAINT DF_WE_IsCurrent DEFAULT 0,
        Description   NVARCHAR(MAX)   NULL,

        CONSTRAINT PK_WorkExp  PRIMARY KEY (ExperienceId),
        CONSTRAINT FK_WE_User  FOREIGN KEY (UserId) REFERENCES JPNS.Users(UserId) ON DELETE CASCADE
    );

    CREATE INDEX IX_WE_UserId ON JPNS.WorkExperience(UserId);
    PRINT 'Table JPNS.WorkExperience created.';
END
GO

-- ── 8. Projects ───────────────────────────────────────────────────────────────
IF OBJECT_ID('JPNS.Projects', 'U') IS NULL
BEGIN
    CREATE TABLE JPNS.Projects (
        ProjectId     INT             IDENTITY(1,1) NOT NULL,
        UserId        INT             NOT NULL,
        Title         NVARCHAR(200)   NOT NULL,
        Description   NVARCHAR(MAX)   NULL,
        Technologies  NVARCHAR(500)   NULL,
        ProjectUrl    NVARCHAR(300)   NULL,
        StartDate     DATE            NULL,
        EndDate       DATE            NULL,

        CONSTRAINT PK_Projects   PRIMARY KEY (ProjectId),
        CONSTRAINT FK_Proj_User  FOREIGN KEY (UserId) REFERENCES JPNS.Users(UserId) ON DELETE CASCADE
    );

    CREATE INDEX IX_Proj_UserId ON JPNS.Projects(UserId);
    PRINT 'Table JPNS.Projects created.';
END
GO

-- ── 9. Certificates ───────────────────────────────────────────────────────────
IF OBJECT_ID('JPNS.Certificates', 'U') IS NULL
BEGIN
    CREATE TABLE JPNS.Certificates (
        CertificateId   INT             IDENTITY(1,1) NOT NULL,
        UserId          INT             NOT NULL,
        CertName        NVARCHAR(200)   NOT NULL,
        IssuingOrg      NVARCHAR(200)   NULL,
        IssueDate       DATE            NULL,
        ExpiryDate      DATE            NULL,
        CertificateUrl  NVARCHAR(500)   NULL,
        CredentialId    NVARCHAR(100)   NULL,

        CONSTRAINT PK_Certs      PRIMARY KEY (CertificateId),
        CONSTRAINT FK_Cert_User  FOREIGN KEY (UserId) REFERENCES JPNS.Users(UserId) ON DELETE CASCADE
    );

    CREATE INDEX IX_Cert_UserId ON JPNS.Certificates(UserId);
    PRINT 'Table JPNS.Certificates created.';
END
GO

-- ── 10. CompanyProfiles ───────────────────────────────────────────────────────
IF OBJECT_ID('JPNS.CompanyProfiles', 'U') IS NULL
BEGIN
    CREATE TABLE JPNS.CompanyProfiles (
        CompanyId       INT             IDENTITY(1,1) NOT NULL,
        UserId          INT             NOT NULL,
        CompanyName     NVARCHAR(200)   NOT NULL,
        Industry        NVARCHAR(100)   NULL,
        CompanySize     NVARCHAR(50)    NULL,
        Website         NVARCHAR(300)   NULL,
        Location        NVARCHAR(200)   NULL,
        Description     NVARCHAR(MAX)   NULL,
        LogoUrl         NVARCHAR(500)   NULL,
        IsVerified      BIT             NOT NULL CONSTRAINT DF_CP_Verified DEFAULT 0,
        CreatedAt       DATETIME2       NOT NULL CONSTRAINT DF_CP_CA DEFAULT GETDATE(),
        UpdatedAt       DATETIME2       NOT NULL CONSTRAINT DF_CP_UA DEFAULT GETDATE(),

        CONSTRAINT PK_CompanyProfiles PRIMARY KEY (CompanyId),
        CONSTRAINT FK_CP_User         FOREIGN KEY (UserId) REFERENCES JPNS.Users(UserId)
    );

    CREATE INDEX IX_CP_UserId   ON JPNS.CompanyProfiles(UserId);
    CREATE INDEX IX_CP_Verified ON JPNS.CompanyProfiles(IsVerified);
    PRINT 'Table JPNS.CompanyProfiles created.';
END
GO

-- ── 11. JobListings ───────────────────────────────────────────────────────────
IF OBJECT_ID('JPNS.JobListings', 'U') IS NULL
BEGIN
    CREATE TABLE JPNS.JobListings (
        JobId               INT             IDENTITY(1,1) NOT NULL,
        PostedByUserId      INT             NOT NULL,
        CompanyId           INT             NOT NULL,
        Title               NVARCHAR(200)   NOT NULL,
        Description         NVARCHAR(MAX)   NOT NULL,
        Location            NVARCHAR(150)   NOT NULL,
        JobType             NVARCHAR(30)    NOT NULL
                            CONSTRAINT CHK_JL_Type CHECK (JobType IN ('Full-time','Part-time','Contract','Internship')),
        SalaryMin           DECIMAL(12,2)   NULL,
        SalaryMax           DECIMAL(12,2)   NULL,
        ExperienceRequired  INT             NOT NULL CONSTRAINT DF_JL_Exp DEFAULT 0
                            CONSTRAINT CHK_JL_Exp CHECK (ExperienceRequired >= 0),
        Status              NVARCHAR(20)    NOT NULL CONSTRAINT DF_JL_Status DEFAULT 'pending'
                            CONSTRAINT CHK_JL_Status CHECK (Status IN ('pending','approved','rejected','closed')),
        TotalApplications   INT             NOT NULL CONSTRAINT DF_JL_TotalApps DEFAULT 0,
        ExpiryDate          DATETIME2       NOT NULL,
        PostedDate          DATETIME2       NOT NULL CONSTRAINT DF_JL_PostedDate DEFAULT GETDATE(),
        UpdatedAt           DATETIME2       NOT NULL CONSTRAINT DF_JL_UpdatedAt  DEFAULT GETDATE(),

        CONSTRAINT PK_JobListings   PRIMARY KEY (JobId),
        CONSTRAINT FK_JL_PostedBy   FOREIGN KEY (PostedByUserId) REFERENCES JPNS.Users(UserId),
        CONSTRAINT FK_JL_Company    FOREIGN KEY (CompanyId)      REFERENCES JPNS.CompanyProfiles(CompanyId),
        CONSTRAINT CHK_JL_Salary    CHECK (SalaryMax IS NULL OR SalaryMax >= SalaryMin)
    );

    CREATE INDEX IX_JL_Status     ON JPNS.JobListings(Status);
    CREATE INDEX IX_JL_CompanyId  ON JPNS.JobListings(CompanyId);
    CREATE INDEX IX_JL_PostedBy   ON JPNS.JobListings(PostedByUserId);
    CREATE INDEX IX_JL_ExpiryDate ON JPNS.JobListings(ExpiryDate);
    PRINT 'Table JPNS.JobListings created.';
END
GO

-- ── 12. JobSkills ─────────────────────────────────────────────────────────────
IF OBJECT_ID('JPNS.JobSkills', 'U') IS NULL
BEGIN
    CREATE TABLE JPNS.JobSkills (
        JobSkillId  INT  IDENTITY(1,1) NOT NULL,
        JobId       INT  NOT NULL,
        SkillId     INT  NOT NULL,
        IsRequired  BIT  NOT NULL CONSTRAINT DF_JS_Required DEFAULT 1,

        CONSTRAINT PK_JobSkills    PRIMARY KEY (JobSkillId),
        CONSTRAINT FK_JS_Job       FOREIGN KEY (JobId)   REFERENCES JPNS.JobListings(JobId) ON DELETE CASCADE,
        CONSTRAINT FK_JS_Skill     FOREIGN KEY (SkillId) REFERENCES JPNS.Skills(SkillId),
        CONSTRAINT UQ_JS_JobSkill  UNIQUE (JobId, SkillId)
    );

    CREATE INDEX IX_JS_JobId   ON JPNS.JobSkills(JobId);
    CREATE INDEX IX_JS_SkillId ON JPNS.JobSkills(SkillId);
    PRINT 'Table JPNS.JobSkills created.';
END
GO

-- ── 13. Applications ──────────────────────────────────────────────────────────
IF OBJECT_ID('JPNS.Applications', 'U') IS NULL
BEGIN
    CREATE TABLE JPNS.Applications (
        ApplicationId   INT             IDENTITY(1,1) NOT NULL,
        UserId          INT             NOT NULL,
        JobId           INT             NOT NULL,
        ResumeId        INT             NOT NULL,
        ResumeSnapshot  NVARCHAR(500)   NULL,
        CoverLetter     NVARCHAR(MAX)   NULL,
        Status          NVARCHAR(20)    NOT NULL CONSTRAINT DF_App_Status DEFAULT 'PENDING'
                        CONSTRAINT CHK_App_Status CHECK (Status IN ('PENDING','SHORTLISTED','REJECTED','HIRED')),
        AppliedDate     DATETIME2       NOT NULL CONSTRAINT DF_App_Applied DEFAULT GETDATE(),
        UpdatedAt       DATETIME2       NOT NULL CONSTRAINT DF_App_UA      DEFAULT GETDATE(),

        CONSTRAINT PK_Applications  PRIMARY KEY (ApplicationId),
        CONSTRAINT FK_App_User      FOREIGN KEY (UserId)   REFERENCES JPNS.Users(UserId),
        CONSTRAINT FK_App_Job       FOREIGN KEY (JobId)    REFERENCES JPNS.JobListings(JobId),
        CONSTRAINT FK_App_Resume    FOREIGN KEY (ResumeId) REFERENCES JPNS.Resumes(ResumeId),
        CONSTRAINT UQ_App_UserJob   UNIQUE (UserId, JobId)
    );

    CREATE INDEX IX_App_UserId ON JPNS.Applications(UserId);
    CREATE INDEX IX_App_JobId  ON JPNS.Applications(JobId);
    CREATE INDEX IX_App_Status ON JPNS.Applications(Status);
    PRINT 'Table JPNS.Applications created.';
END
GO

-- ── 14. SavedJobs ─────────────────────────────────────────────────────────────
IF OBJECT_ID('JPNS.SavedJobs', 'U') IS NULL
BEGIN
    CREATE TABLE JPNS.SavedJobs (
        SavedJobId  INT       IDENTITY(1,1) NOT NULL,
        UserId      INT       NOT NULL,
        JobId       INT       NOT NULL,
        SavedAt     DATETIME2 NOT NULL CONSTRAINT DF_SJ_SavedAt DEFAULT GETDATE(),

        CONSTRAINT PK_SavedJobs    PRIMARY KEY (SavedJobId),
        CONSTRAINT FK_SJ_User      FOREIGN KEY (UserId) REFERENCES JPNS.Users(UserId)       ON DELETE CASCADE,
        CONSTRAINT FK_SJ_Job       FOREIGN KEY (JobId)  REFERENCES JPNS.JobListings(JobId)  ON DELETE CASCADE,
        CONSTRAINT UQ_SJ_UserJob   UNIQUE (UserId, JobId)
    );

    CREATE INDEX IX_SJ_UserId ON JPNS.SavedJobs(UserId);
    PRINT 'Table JPNS.SavedJobs created.';
END
GO

-- ── 15. Interviews ────────────────────────────────────────────────────────────
IF OBJECT_ID('JPNS.Interviews', 'U') IS NULL
BEGIN
    CREATE TABLE JPNS.Interviews (
        InterviewId       INT             IDENTITY(1,1) NOT NULL,
        ApplicationId     INT             NOT NULL,
        ScheduledByUserId INT             NOT NULL,
        ScheduledAt       DATETIME2       NOT NULL,
        Mode              NVARCHAR(20)    NOT NULL
                          CONSTRAINT CHK_Iv_Mode CHECK (Mode IN ('Online','Offline')),
        MeetingLink       NVARCHAR(500)   NULL,
        Venue             NVARCHAR(300)   NULL,
        Status            NVARCHAR(20)    NOT NULL CONSTRAINT DF_Iv_Status DEFAULT 'SCHEDULED'
                          CONSTRAINT CHK_Iv_Status CHECK (Status IN ('SCHEDULED','COMPLETED','CANCELLED','NO_SHOW')),
        Feedback          NVARCHAR(MAX)   NULL,
        CreatedAt         DATETIME2       NOT NULL CONSTRAINT DF_Iv_CA DEFAULT GETDATE(),
        UpdatedAt         DATETIME2       NOT NULL CONSTRAINT DF_Iv_UA DEFAULT GETDATE(),

        CONSTRAINT PK_Interviews PRIMARY KEY (InterviewId),
        CONSTRAINT FK_Iv_App     FOREIGN KEY (ApplicationId)     REFERENCES JPNS.Applications(ApplicationId),
        CONSTRAINT FK_Iv_Sched   FOREIGN KEY (ScheduledByUserId) REFERENCES JPNS.Users(UserId)
    );

    CREATE INDEX IX_Iv_AppId   ON JPNS.Interviews(ApplicationId);
    CREATE INDEX IX_Iv_SchedAt ON JPNS.Interviews(ScheduledAt);
    PRINT 'Table JPNS.Interviews created.';
END
GO

-- ── 16. Notifications ─────────────────────────────────────────────────────────
IF OBJECT_ID('JPNS.Notifications', 'U') IS NULL
BEGIN
    CREATE TABLE JPNS.Notifications (
        NotificationId   INT             IDENTITY(1,1) NOT NULL,
        UserId           INT             NOT NULL,
        Message          NVARCHAR(MAX)   NOT NULL,
        NotificationType NVARCHAR(50)    NOT NULL CONSTRAINT DF_Notif_Type DEFAULT 'SYSTEM',
        ReferenceId      INT             NULL,
        IsRead           BIT             NOT NULL CONSTRAINT DF_Notif_IsRead DEFAULT 0,
        CreatedAt        DATETIME2       NOT NULL CONSTRAINT DF_Notif_CA DEFAULT GETDATE(),

        CONSTRAINT PK_Notifications PRIMARY KEY (NotificationId),
        CONSTRAINT FK_Notif_User    FOREIGN KEY (UserId) REFERENCES JPNS.Users(UserId) ON DELETE CASCADE
    );

    CREATE INDEX IX_Notif_UserId ON JPNS.Notifications(UserId);
    CREATE INDEX IX_Notif_IsRead ON JPNS.Notifications(UserId, IsRead);
    PRINT 'Table JPNS.Notifications created.';
END
GO

-- ── 17. Feedback ──────────────────────────────────────────────────────────────
IF OBJECT_ID('JPNS.Feedback', 'U') IS NULL
BEGIN
    CREATE TABLE JPNS.Feedback (
        FeedbackId   INT             IDENTITY(1,1) NOT NULL,
        UserId       INT             NOT NULL,
        CompanyId    INT             NOT NULL,
        Rating       TINYINT         NOT NULL CONSTRAINT CHK_FB_Rating CHECK (Rating BETWEEN 1 AND 5),
        Review       NVARCHAR(MAX)   NULL,
        IsAnonymous  BIT             NOT NULL CONSTRAINT DF_FB_Anon DEFAULT 0,
        CreatedAt    DATETIME2       NOT NULL CONSTRAINT DF_FB_CA DEFAULT GETDATE(),

        CONSTRAINT PK_Feedback   PRIMARY KEY (FeedbackId),
        CONSTRAINT FK_FB_User    FOREIGN KEY (UserId)    REFERENCES JPNS.Users(UserId),
        CONSTRAINT FK_FB_Company FOREIGN KEY (CompanyId) REFERENCES JPNS.CompanyProfiles(CompanyId)
    );

    CREATE INDEX IX_FB_CompanyId ON JPNS.Feedback(CompanyId);
    CREATE INDEX IX_FB_Rating    ON JPNS.Feedback(CompanyId, Rating);
    PRINT 'Table JPNS.Feedback created.';
END
GO

-- ── 18. Complaints ────────────────────────────────────────────────────────────
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

    CREATE INDEX IX_Comp_Status ON JPNS.Complaints(Status);
    CREATE INDEX IX_Comp_SubmBy ON JPNS.Complaints(SubmittedByUserId);
    PRINT 'Table JPNS.Complaints created.';
END
GO

PRINT 'All 18 JPNS tables created successfully.';
