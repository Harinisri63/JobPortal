using JobPortal.Shared.Enums;
using JobPortal.Shared.Exceptions;
using JobPortal.Shared.Structs;

namespace JobPortal.Features.JobPosting;

public class JobListing
{
    private string     _jobId;
    private string     _adminId;
    private string     _title;
    private string     _description;
    private string     _company;
    private string     _location;
    private SalaryRange _salaryRange;
    private int        _experienceRequired;
    private List<string> _requiredSkills;
    private DateTime   _postedDate;
    private DateTime   _expiryDate;
    private bool       _isActive;
    private JobStatus  _status;

    public static int TotalPostings { get; private set; } = 0;

    public string JobId
    {
        get => _jobId;
        set => _jobId = value;
    }

    public string AdminId
    {
        get => _adminId;
        set => _adminId = value;
    }

    public string Title
    {
        get => _title;
        set => _title = value;
    }

    public string Description
    {
        get => _description;
        set => _description = value;
    }

    public string Company
    {
        get => _company;
        set => _company = value;
    }

    public string Location
    {
        get => _location;
        set => _location = value;
    }

    public SalaryRange SalaryRange
    {
        get => _salaryRange;
        set
        {
            _salaryRange = value;
            SalaryMin = value.Min;
            SalaryMax = value.Max;
        }
    }

    public int PostedByUserId { get; set; }
    public int CompanyId { get; set; }
    public string JobType { get; set; } = "Full-time";
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public int TotalApplications { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public int NumberOfRounds { get; set; } = 3;

    public int ExperienceRequired
    {
        get => _experienceRequired;
        set => _experienceRequired = value;
    }

    public List<string> RequiredSkills
    {
        get => _requiredSkills;
        set => _requiredSkills = value;
    }

    public DateTime PostedDate
    {
        get => _postedDate;
        set => _postedDate = value;
    }

    public DateTime ExpiryDate
    {
        get => _expiryDate;
        set => _expiryDate = value;
    }

    public bool IsActive
    {
        get => _isActive;
        set => _isActive = value;
    }

    public JobStatus Status
    {
        get => _status;
        set => _status = value;
    }

    public JobListing()
    {
        _jobId          = Guid.NewGuid().ToString();
        _adminId        = "";
        _title          = "";
        _description    = "";
        _company        = "";
        _location       = "";
        _salaryRange    = new SalaryRange(1, 1);
        _requiredSkills = new List<string>();
        _postedDate     = DateTime.Now;
        _expiryDate     = DateTime.Now.AddDays(30);
        _isActive       = false;
        _status         = JobStatus.PENDING_APPROVAL;
        NumberOfRounds  = 3;
    }

    public JobListing(string adminId, string title, string description, string company, string location, SalaryRange salaryRange, int experienceRequired,List<string> requiredSkills, DateTime expiryDate, int numberOfRounds = 3)
    {
        ValidateTitle(title);
        ValidateDescription(description);
        ValidateSkills(requiredSkills);
        ValidateExpiry(expiryDate);
        if (experienceRequired < 0) 
            throw new ValidationException("ExperienceRequired", "Experience cannot be negative.");

        _jobId              = Guid.NewGuid().ToString();
        _adminId            = adminId;
        _title              = title.Trim();
        _description        = description.Trim();
        _company            = company.Trim();
        _location           = location.Trim();
        _salaryRange        = salaryRange;
        _experienceRequired = experienceRequired;
        List<string> trimmedSkills = new List<string>();
        foreach (string s in requiredSkills)
        {
            trimmedSkills.Add(s.Trim());
        }
        _requiredSkills = trimmedSkills;
        _postedDate         = DateTime.Now;
        _expiryDate         = expiryDate;
        _isActive           = false;
        _status             = JobStatus.PENDING_APPROVAL;
        NumberOfRounds      = numberOfRounds;
        TotalPostings++;
    }

    public void SetExpiry(DateTime expiry)
    {
        ValidateExpiry(expiry);
        _expiryDate = expiry;
    }

    public void Approve()
    {
        if (IsExpired()) 
        {
            _status = JobStatus.EXPIRED; 
            _isActive = false; 
            return; 
        }
        _status   = JobStatus.APPROVED;
        _isActive = true;
    }

    public void Reject()
    {
        _status   = JobStatus.REJECTED;
        _isActive = false;
    }

    public void MarkClosed()
    {
        _status   = JobStatus.CLOSED;
        _isActive = false;
    }

    public void MarkExpired()
    {
        _status   = JobStatus.EXPIRED;
        _isActive = false;
    }

    public bool IsExpired() => DateTime.Now > _expiryDate;

    public void UpdateTitle(string title)       
    {
        ValidateTitle(title);      
        _title = title.Trim(); 
    }
    public void UpdateDescription(string desc)  
    {
        ValidateDescription(desc); 
        _description = desc.Trim(); 
    }
    public void UpdateLocation(string location) 
    { 
        _location = location.Trim(); 
    }
    public void UpdateSalaryRange(SalaryRange range) 
    {
        _salaryRange = range; 
    }
    public void UpdateSkills(List<string> skills)    
    {
        ValidateSkills(skills);
        List<string> result = new List<string>();
        foreach (string s in skills)
        {
            result.Add(s.Trim());
        }
        _requiredSkills = result;
    }

    public void Display()
    {
        Console.WriteLine($"  Job ID    : {_jobId}");
        Console.WriteLine($"  Title     : {_title}");
        Console.WriteLine($"  Company   : {_company}");
        Console.WriteLine($"  Location  : {_location}");
        Console.WriteLine($"  Salary    : {_salaryRange}");
        Console.WriteLine($"  Exp Req   : {_experienceRequired} yr(s)");
        Console.WriteLine($"  Skills    : {string.Join(", ", _requiredSkills)}");
        Console.WriteLine($"  Posted    : {_postedDate:dd MMM yyyy}");
        Console.WriteLine($"  Expires   : {_expiryDate:dd MMM yyyy}");
        Console.WriteLine("  Status     : "); JobPortal.UI.ConsoleHelper.PrintBadge(_status.ToString()); 
        Console.WriteLine($"  Desc      : {_description[..Math.Min(_description.Length, 100)]}...");
    }

    public override string ToString() => $"{_title} @ {_company} | {_location} | {_salaryRange}";

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ValidationException("Title", "Job title is required.");
        if (title.Trim().Length < JobConfig.MinTitleLength)
            throw new ValidationException("Title", $"Job title must be at least {JobConfig.MinTitleLength} characters.");
        if (title.Trim().Length > JobConfig.MaxTitleLength)
            throw new ValidationException("Title", $"Job title must not exceed {JobConfig.MaxTitleLength} characters.");
    }

    private static void ValidateDescription(string desc)
    {
        if (string.IsNullOrWhiteSpace(desc))
            throw new ValidationException("Description", "Job description is required.");
        if (desc.Trim().Length > JobConfig.MaxDescriptionLength)
            throw new ValidationException("Description", $"Description must not exceed {JobConfig.MaxDescriptionLength} characters.");
    }

    private static void ValidateSkills(List<string> skills)
    {
        if (skills == null || skills.Count < JobConfig.MinSkillCount)
            throw new ValidationException("Skills", $"At least {JobConfig.MinSkillCount} skill is required.");
    }

    private static void ValidateExpiry(DateTime expiry)
    {
        if (expiry <= DateTime.Now)
            throw new ValidationException("ExpiryDate", "Expiry date must be in the future.");
    }

    public sealed override int GetHashCode() => _jobId.GetHashCode();

    public virtual void ExportToFile(string path)
    {
        File.WriteAllText(path, ToString());
    }

    internal static JobListing CreateSeed(string jobId, string adminId, string title, string description,
        string company, string location, SalaryRange salary, int expRequired,
        List<string> skills, bool isActive, JobStatus status, int postedDaysAgo = 10, int expiryDays = 60)
    {
        var j = new JobListing();
        j._jobId              = jobId;
        j._adminId            = adminId;
        j._title              = title;
        j._description        = description;
        j._company            = company;
        j._location           = location;
        j._salaryRange        = salary;
        j._experienceRequired = expRequired;
        j._requiredSkills     = skills;
        j._postedDate         = DateTime.Now.AddDays(-postedDaysAgo);
        j._expiryDate         = DateTime.Now.AddDays(expiryDays);
        j._isActive           = isActive;
        j._status             = status;
        return j;
    }

    ~JobListing() { }
}
