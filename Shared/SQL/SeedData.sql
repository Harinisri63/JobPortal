-- ============================================================
-- JPNS — Job Portal Navigation System
-- Script 02: Seed Data
-- ============================================================
-- Run after Database.sql
-- Seeds: Skills, Admin, Company, 2 Job Seekers, 2 Jobs
-- Default password for all seed accounts: Admin@123
-- NOTE: Replace PLACEHOLDER_HASH_* with real BCrypt hashes.
-- ============================================================

BEGIN TRANSACTION;

-- ── Skills ────────────────────────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM JPNS.Skills WHERE SkillName = 'C#')
    INSERT INTO JPNS.Skills (SkillName, Category) VALUES
        ('C#',          'Programming'),
        ('.NET',        'Framework'),
        ('SQL Server',  'Database'),
        ('React',       'Frontend'),
        ('Node.js',     'Backend'),
        ('Java',        'Programming'),
        ('Spring Boot', 'Framework'),
        ('Python',      'Programming'),
        ('Azure',       'Cloud'),
        ('Docker',      'DevOps'),
        ('TypeScript',  'Programming'),
        ('Git',         'DevOps'),
        ('REST API',    'Backend'),
        ('HTML/CSS',    'Frontend'),
        ('Angular',     'Frontend');

-- ── Admin User ────────────────────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM JPNS.Users WHERE Email = 'admin@jpns.com')
BEGIN
    INSERT INTO JPNS.Users
        (FullName, Email, Username, PasswordHash, Salt, Role, IsActive, IsEmailVerified)
    VALUES
        ('System Administrator', 'admin@jpns.com', 'sysadmin',
         '$2a$12$REPLACE_WITH_REAL_BCRYPT_HASH_ADMIN',
         'replace-with-real-salt-admin',
         'admin', 1, 1);
END

-- ── Company Profile ───────────────────────────────────────────────────────────
DECLARE @AdminUserId INT = (SELECT UserId FROM JPNS.Users WHERE Email = 'admin@jpns.com');

IF @AdminUserId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM JPNS.CompanyProfiles WHERE CompanyName = 'TechCorp Solutions')
BEGIN
    INSERT INTO JPNS.CompanyProfiles
        (UserId, CompanyName, Industry, CompanySize, Website, Location, IsVerified)
    VALUES
        (@AdminUserId, 'TechCorp Solutions', 'Information Technology',
         '201-500', 'https://techcorp.example.com', 'Bengaluru, India', 1);
END

-- ── Job Seekers ───────────────────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM JPNS.Users WHERE Email = 'alice@email.com')
    INSERT INTO JPNS.Users
        (FullName, Email, Username, PasswordHash, Salt, Role, IsActive, IsEmailVerified)
    VALUES
        ('Alice Johnson', 'alice@email.com', 'alice_j',
         '$2a$12$REPLACE_WITH_REAL_BCRYPT_HASH_ALICE',
         'replace-with-real-salt-alice',
         'jobseeker', 1, 1);

IF NOT EXISTS (SELECT 1 FROM JPNS.Users WHERE Email = 'bob@email.com')
    INSERT INTO JPNS.Users
        (FullName, Email, Username, PasswordHash, Salt, Role, IsActive, IsEmailVerified)
    VALUES
        ('Bob Smith', 'bob@email.com', 'bob_smith',
         '$2a$12$REPLACE_WITH_REAL_BCRYPT_HASH_BOB',
         'replace-with-real-salt-bob',
         'jobseeker', 1, 1);

-- ── JobSeeker Profiles ────────────────────────────────────────────────────────
DECLARE @AliceId INT = (SELECT UserId FROM JPNS.Users WHERE Email = 'alice@email.com');
DECLARE @BobId   INT = (SELECT UserId FROM JPNS.Users WHERE Email = 'bob@email.com');

IF @AliceId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM JPNS.JobSeekerProfiles WHERE UserId = @AliceId)
    INSERT INTO JPNS.JobSeekerProfiles (UserId, Headline, CurrentLocation, ExperienceYears)
    VALUES (@AliceId, 'Full-Stack .NET Developer', 'Bengaluru, India', 4);

IF @BobId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM JPNS.JobSeekerProfiles WHERE UserId = @BobId)
    INSERT INTO JPNS.JobSeekerProfiles (UserId, Headline, CurrentLocation, ExperienceYears)
    VALUES (@BobId, 'React & Node.js Developer', 'Mumbai, India', 2);

-- ── Job Listings ──────────────────────────────────────────────────────────────
DECLARE @CompanyId INT = (SELECT CompanyId FROM JPNS.CompanyProfiles WHERE CompanyName = 'TechCorp Solutions');

IF @CompanyId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM JPNS.JobListings WHERE Title = 'Senior C# Developer')
    INSERT INTO JPNS.JobListings
        (PostedByUserId, CompanyId, Title, Description, Location, JobType,
         SalaryMin, SalaryMax, ExperienceRequired, Status, ExpiryDate)
    VALUES
        (@AdminUserId, @CompanyId,
         'Senior C# Developer',
         'We are looking for an experienced C# / .NET developer to join our backend team. '
         + 'You will design and build microservices using .NET, SQL Server, and Azure.',
         'Bengaluru, India', 'Full-time',
         1200000, 2000000, 3, 'approved',
         DATEADD(MONTH, 3, GETDATE()));

IF @CompanyId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM JPNS.JobListings WHERE Title = 'React Frontend Developer')
    INSERT INTO JPNS.JobListings
        (PostedByUserId, CompanyId, Title, Description, Location, JobType,
         SalaryMin, SalaryMax, ExperienceRequired, Status, ExpiryDate)
    VALUES
        (@AdminUserId, @CompanyId,
         'React Frontend Developer',
         'Join our product team as a React developer. Build responsive, '
         + 'accessible UIs using React 18, TypeScript, and Tailwind CSS.',
         'Remote', 'Full-time',
         800000, 1500000, 1, 'approved',
         DATEADD(MONTH, 2, GETDATE()));

COMMIT TRANSACTION;

PRINT 'Seed data inserted successfully.';
