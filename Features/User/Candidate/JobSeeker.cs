using JobPortal.Shared.Exceptions;
using JobPortal.Shared.Interfaces;
using JobPortal.Features.Notifications;

namespace JobPortal.Features.Candidate;

using JobPortal.Shared.Interfaces;

internal class JobSeeker : BaseUser, IUserProfile, INotifiable
{
    private string       _seekerId;
    private string       _name;
    private string       _phone;
    private string       _location;
    private string       _resumeUrl;
    private bool         _isVerified;
    private int          _failedAttempts;
    private List<string> _savedJobs;
    private DateTime     _registeredAt;
    private string       _skills;
    private string       _education;
    private string       _experience;

    private int          _clearedRounds;
    private int          _totalRounds;

    private List<User.Candidate.Project> _projects = new();
    private List<User.Candidate.Certificate> _certificates = new();
    private List<User.Candidate.Education> _educationList = new();
    private List<User.Candidate.WorkExperience> _experienceList = new();
    private List<User.Candidate.CandidateSkill> _skillsList = new();
    private List<User.Candidate.Feedback> _feedbacks = new();

    private static int _counter = 1;

    public string SeekerId
    {
        get => _seekerId;
        set => _seekerId = value;
    }

    public string Name
    {
        get => _name;
        set => _name = value;
    }

    public string Phone
    {
        get => _phone;
        set => _phone = value;
    }

    public string Location
    {
        get => _location;
        set => _location = value;
    }

    public string ResumeUrl
    {
        get => _resumeUrl;
        set => _resumeUrl = value;
    }

    public bool IsVerified
    {
        get => _isVerified;
        set => _isVerified = value;
    }

    public int FailedAttempts
    {
        get => _failedAttempts;
        set => _failedAttempts = value;
    }

    public List<string> SavedJobs
    {
        get => _savedJobs;
        set => _savedJobs = value;
    }

    public DateTime RegisteredAt
    {
        get => _registeredAt;
        set => _registeredAt = value;
    }

    public string Skills
    {
        get => _skills;
        set => _skills = value;
    }

    public string Education
    {
        get => _education;
        set => _education = value;
    }

    public string Experience
    {
        get => _experience;
        set => _experience = value;
    }

    public int ClearedRounds
    {
        get => _clearedRounds;
        set => _clearedRounds = value;
    }

    public int TotalRounds
    {
        get => _totalRounds;
        set => _totalRounds = value;
    }

    public int CurrentRound
    {
        get => _clearedRounds;
        set => _clearedRounds = value;
    }

    public int? RejectedRound { get; set; }
    public string? CandidateStatus { get; set; }

    public List<User.Candidate.Project> Projects { get => _projects; set => _projects = value; }
    public List<User.Candidate.Certificate> Certificates { get => _certificates; set => _certificates = value; }
    public List<User.Candidate.Education> EducationList { get => _educationList; set => _educationList = value; }
    public List<User.Candidate.WorkExperience> ExperienceList { get => _experienceList; set => _experienceList = value; }
    public List<User.Candidate.CandidateSkill> SkillsList { get => _skillsList; set => _skillsList = value; }
    public List<User.Candidate.Feedback> Feedbacks { get => _feedbacks; set => _feedbacks = value; }

    public JobSeeker() : base()
    {
        _seekerId      = "";
        _name          = "";
        _phone         = "";
        _location      = "";
        _resumeUrl     = "";
        _isVerified    = false;
        _failedAttempts= 0;
        _savedJobs     = new List<string>();
        _registeredAt  = DateTime.Now;
        _skills        = "";
        _education     = "";
        _experience    = "";
        _clearedRounds = 0;
        _totalRounds   = 3;
        RejectedRound  = null;
        CandidateStatus = null;
    }

    public JobSeeker(string name, string email, string phone, string password) : base()
    {
        Auth.AuthService.ValidateName(name);
        Auth.AuthService.ValidateEmail(email);
        Auth.AuthService.ValidatePhone(phone);
        Auth.AuthService.ValidatePassword(password);

        _seekerId      = GenerateSeekerId();
        _id            = _seekerId;
        _name          = name.Trim();
        _email         = email.Trim().ToLowerInvariant();
        _phone         = phone.Trim();
        _passwordHash  = Auth.AuthService.HashPassword(password);
        _location      = "";
        _resumeUrl     = "";
        _isVerified    = false;
        _failedAttempts= 0;
        _savedJobs     = new List<string>();
        _registeredAt  = DateTime.Now;
        _lastLoginAt   = DateTime.Now;
        _skills        = "";
        _education     = "";
        _experience    = "";
        _clearedRounds = 0;
        _totalRounds   = 3;
        RejectedRound  = null;
        CandidateStatus = null;
    }

    public override string GetRole() => "JobSeeker";

    public void Verify() => _isVerified = true;

    public void Lock()    { _isActive = false; }
    public void Activate(){ _isActive = true; _failedAttempts = 0; }

    public void IncrementFailedAttempts() => _failedAttempts++;
    public void ResetFailedAttempts()     { _failedAttempts = 0; _isActive = true; }

    public void SetResumeUrl(string url)  => _resumeUrl = url;

    public void SaveJob(string jobId)
    {
        if (_savedJobs.Contains(jobId))
            throw new JPNSException("Job is already saved.");
        if (_savedJobs.Count >= JobPosting.JobConfig.MaxSavedJobs)
            throw new JPNSException($"Saved jobs limit ({JobPosting.JobConfig.MaxSavedJobs}) reached.");
        _savedJobs.Add(jobId);
    }

    public void RemoveSavedJob(string jobId)
    {
        if (!_savedJobs.Remove(jobId))
            throw new JPNSException("Unable to remove job. It is not in the saved list.");
    }

    public void UpdateProfile()
    {
        Console.WriteLine("  (Use the My Profile menu to update individual fields.)");
    }

    public void UpdateName(string name)     { Auth.AuthService.ValidateName(name);  _name     = name.Trim(); }
    public void UpdatePhone(string phone)   { Auth.AuthService.ValidatePhone(phone); _phone   = phone.Trim(); }
    public void UpdateLocation(string loc)  { _location  = loc.Trim(); }
    public void UpdateSkills(string skills) { _skills    = skills.Trim(); }
    public void UpdateEducation(string edu) { _education = edu.Trim(); }
    public void UpdateExperience(string exp){ _experience= exp.Trim(); }
    public void UpdatePassword(string raw)  { Auth.AuthService.ValidatePassword(raw); _passwordHash = Auth.AuthService.HashPassword(raw); }

    public void Display()
    {
        Console.WriteLine($"  Seeker ID : {_seekerId}");
        Console.WriteLine($"  Name      : {_name}");
        Console.WriteLine($"  Email     : {_email}");
        Console.WriteLine($"  Phone     : {_phone}");
        Console.WriteLine($"  Location  : {_location}");
        Console.WriteLine($"  Verified  : {(_isVerified ? "Yes" : "No")}");
        Console.WriteLine($"  Active    : {(_isActive ? "Yes" : "No")}");
        Console.WriteLine($"  Resume    : {(_resumeUrl == "" ? "Not uploaded" : _resumeUrl)}");
        Console.WriteLine($"  Skills    : {(_skills == "" ? "Not set" : _skills)}");
        Console.WriteLine($"  Education : {(_education == "" ? "Not set" : _education)}");
        Console.WriteLine($"  Exp       : {(_experience == "" ? "Not set" : _experience)}");
        Console.WriteLine($"  Registered: {_registeredAt:dd MMM yyyy}");
        Console.WriteLine($"  Last Login: {_lastLoginAt:dd MMM yyyy HH:mm}");
    }

    public string GetProfileSummary() => $"{_name} | {_email} | {_location} | Skills: {_skills}";

    public void SendNotification(string message)
    {
        FileStorage.DataStore.NotificationService.Send(_seekerId, "Notification", message, "SYSTEM");
    }

    private static string GenerateSeekerId()
    {
        string id = $"JSK-{DateTime.Now.Year}-{_counter:D4}";
        _counter++;
        return id;
    }

    internal static JobSeeker CreateSeed(string seekerId, string name, string email, string phone,string password, string location, string skills, string education, string experience,bool isVerified = true, bool isActive = true)
    {
        var s = new JobSeeker();
        s._seekerId      = seekerId;
        s._id            = seekerId;
        s._name          = name;
        s._email         = email.ToLowerInvariant();
        s._phone         = phone;
        s._passwordHash  = Auth.AuthService.HashPassword(password);
        s._location      = location;
        s._skills        = skills;
        s._education     = education;
        s._experience    = experience;
        s._isVerified    = isVerified;
        s._isActive      = isActive;
        s._registeredAt  = DateTime.Now.AddMonths(-3);
        s._savedJobs     = new List<string>();
        s._clearedRounds = 0;
        s._totalRounds   = 3;
        s.RejectedRound  = null;
        s.CandidateStatus = isActive ? "ACTIVE" : "BLOCKED";
        return s;
    }

    internal static void SyncCounter(int next) => _counter = next;
}
