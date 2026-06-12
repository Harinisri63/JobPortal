using JobPortal.Shared.Exceptions;
using JobPortal.Features.Candidate;
using JobPortal.Features.JobPosting;
using JobPortal.Features.Notifications;
using JobPortal.Features.Application;

namespace JobPortal.Features.Admin;

internal class Admin1 : BaseUser
{
    private string _adminId;
    private string _name;
    private string _company;
    private bool _isApproved;

    public string AdminId
    {
        get => _adminId;
        set => _adminId = value;
    }

    public string Name
    {
        get => _name;
        set => _name = value;
    }

    public string Company
    {
        get => _company;
        set => _company = value;
    }

    public bool IsApproved
    {
        get => _isApproved;
        set => _isApproved = value;
    }

    public Admin1() : base()
    {
        _adminId = "";
        _name = "";
        _company = "";
        _isApproved = false;
    }

    public Admin1(string name, string email, string phone, string company, string password) : base()
    {
        Auth.AuthService.ValidateName(name);
        Auth.AuthService.ValidateEmail(email);
        Auth.AuthService.ValidatePassword(password);

        _adminId = Guid.NewGuid().ToString();
        _id = _adminId;
        _name = name.Trim();
        _email = email.Trim().ToLowerInvariant();
        _company = company.Trim();
        _passwordHash = Auth.AuthService.HashPassword(password);
        _isActive = true;
        _isApproved = false;
        _createdAt = DateTime.Now;
        _lastLoginAt = DateTime.Now;
    }

    public Admin1(string adminId, string email, string password) : base(adminId, email, password)
    {
        _adminId = adminId;
        _name = "";
        _company = "";
        _isApproved = true;
    }

    public override string GetRole() => "Admin";

    internal static Admin1 CreateSeedAdmin()
    {
        var a = new Admin1();
        a._adminId = "ADM001";
        a._id = "ADM001";
        a._name = "Super Admin";
        a._email = "admin@jpns.com";
        a._company = "JPNS";
        a._passwordHash = Auth.AuthService.HashPassword("Admin@123");
        a._isActive = true;
        a._isApproved = true;
        a._createdAt = DateTime.Now;
        a._lastLoginAt = DateTime.Now;
        return a;
    }

    internal static Admin1 CreateSeed(string adminId, string name, string email,
        string company, string password, bool isApproved = true)
    {
        var a = new Admin1();
        a._adminId      = adminId;
        a._id           = adminId;
        a._name         = name;
        a._email        = email.ToLowerInvariant();
        a._company      = company;
        a._passwordHash = Auth.AuthService.HashPassword(password);
        a._isActive     = true;
        a._isApproved   = isApproved;
        a._createdAt    = DateTime.Now;
        a._lastLoginAt  = DateTime.Now;
        return a;
    }

    public void Approve()
    {
        _isApproved = true;
    }

    public void Reject()
    {
        _isApproved = false;
    }

    public void UpdateName(string name)
    {
        Auth.AuthService.ValidateName(name);
        _name = name.Trim();
    }

    public void UpdateCompany(string co)
    {
        _company = co.Trim();
    }

    public void UpdatePassword(string raw)
    {
        Auth.AuthService.ValidatePassword(raw);
        _passwordHash = Auth.AuthService.HashPassword(raw);
    }

    public void AddJobListing(JobListing job)
    {
        if (!_isApproved)
            throw new AccessDeniedException("Account pending admin approval. Cannot post jobs.");
        FileStorage.DataStore.Jobs.Add(job);
        FileStorage.DataStore.SaveJobs();
        FileStorage.DataStore.NotificationService.Send(
            _adminId, "Job Posted", $"Your job '{job.Title}' has been submitted for approval.", "ANNOUNCEMENT");
    }

    public void UpdateJobListing(string jobId, string title, string description)
    {
        JobListing? job = null;
        foreach (JobListing j in FileStorage.DataStore.Jobs)
        {
            if (j.JobId == jobId && j.AdminId == _adminId)
            {
                job = j;
                break;
            }
        }
        if (job == null)
            throw new JPNSException("Job listing not found or not owned by you.");
        job.UpdateTitle(title);
        job.UpdateDescription(description);
        FileStorage.DataStore.SaveJobs();
    }

    public void DeleteJobListing(string jobId)
    {
        JobListing? job = null;
        foreach (JobListing j in FileStorage.DataStore.Jobs)
        {
            if (j.JobId == jobId && j.AdminId == _adminId)
            {
                job = j;
                break;
            }
        }
        if (job == null)
            throw new JPNSException("Job listing not found or not owned by you.");
        FileStorage.DataStore.Jobs.Remove(job);
        FileStorage.DataStore.SaveJobs();
    }

    public void CloseJobListing(string jobId)
    {
        JobListing? job = null;
        foreach (JobListing j in FileStorage.DataStore.Jobs)
        {
            if (j.JobId == jobId && j.AdminId == _adminId)
            {
                job = j;
                break;
            }
        }
        if (job == null)
            throw new JPNSException("Job listing not found.");
        job.MarkClosed();
        FileStorage.DataStore.SaveJobs();
    }

    public List<Application1> ViewApplications(string jobId)
    {
        List<Application1> result = new List<Application1>();
        foreach (Application1 a in FileStorage.DataStore.Applications)
        {
            if (a.JobId == jobId)
            {
                result.Add(a);
            }
        }
        return result;
    }

    public void ShortlistCandidate(string appId)
    {
        Application1? app = null;
        foreach (Application1 a in FileStorage.DataStore.Applications)
        {
            if (a.ApplicationId == appId)
            {
                app = a;
                break;
            }
        }
        if (app == null)
            throw new JPNSException("Application not found.");
        app.UpdateStatus("SHORTLISTED");
        FileStorage.DataStore.NotificationService.Send(app.JobSeekerId, "Application Update",
            "Congratulations! You have been shortlisted.", "APPLICATION_STATUS");
        FileStorage.DataStore.SaveApplications();
    }

    public void RejectCandidate(string appId)
    {
        Application1? app = null;
        foreach (Application1 a in FileStorage.DataStore.Applications)
        {
            if (a.ApplicationId == appId)
            {
                app = a;
                break;
            }
        }
        if (app == null)
            throw new JPNSException("Application not found.");
        app.UpdateStatus("REJECTED");
        FileStorage.DataStore.NotificationService.Send(app.JobSeekerId, "Application Update",
            "Unfortunately, your application has been rejected.", "APPLICATION_STATUS");
        FileStorage.DataStore.SaveApplications();
    }

    public void BlockAccount(string seekerId)
    {
        JobSeeker? seeker = FileStorage.DataStore.FindJobSeekerById(seekerId);
        if (seeker == null)
            throw new JPNSException("Job seeker not found.");
        seeker.Lock();
        FileStorage.DataStore.SaveJobSeekers();
    }

    public void ActivateAccount(string seekerId)
    {
        JobSeeker? seeker = FileStorage.DataStore.FindJobSeekerById(seekerId);
        if (seeker == null)
            throw new JPNSException("Job seeker not found.");
        seeker.Activate();
        FileStorage.DataStore.SaveJobSeekers();
    }

    public void SendAnnouncement(string message)
    {
        List<string> ids = new List<string>();
        foreach (JobSeeker s in FileStorage.DataStore.JobSeekers)
        {
            ids.Add(s.SeekerId);
        }
        FileStorage.DataStore.NotificationService.BroadcastToAll(ids, "Announcement", message);
    }

    public void Display()
    {
        Console.WriteLine($"  Admin ID  : {_adminId}");
        Console.WriteLine($"  Name      : {_name}");
        Console.WriteLine($"  Email     : {_email}");
        Console.WriteLine($"  Company   : {_company}");
        Console.WriteLine($"  Approved  : {(_isApproved ? "Yes" : "No")}");
        Console.WriteLine($"  Active    : {(_isActive ? "Yes" : "No")}");
        Console.WriteLine($"  Created   : {_createdAt:dd MMM yyyy}");
    }
}