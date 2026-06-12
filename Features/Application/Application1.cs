using JobPortal.Shared.Enums;
using JobPortal.Shared.Exceptions;
using JobPortal.Shared.Interfaces;
using JobPortal.Features.Notifications;

namespace JobPortal.Features.Application;

internal class Application1 : INotifiable
{
    private string _applicationId;
    private string _jobSeekerId;
    private string _jobId;
    private DateTime _appliedDate;
    private ApplicationStatus _status;
    private string _resumeSnapshot;
    private string _coverLetter;

    public event NotifyHandler? OnStatusChanged;

    public string ApplicationId
    {
        get => _applicationId;
        set => _applicationId = value;
    }

    public string JobSeekerId
    {
        get => _jobSeekerId;
        set => _jobSeekerId = value;
    }

    public string JobId
    {
        get => _jobId;
        set => _jobId = value;
    }

    public DateTime AppliedDate
    {
        get => _appliedDate;
        set => _appliedDate = value;
    }

    public ApplicationStatus Status
    {
        get => _status;
        set => _status = value;
    }

    public string ResumeSnapshot
    {
        get => _resumeSnapshot;
        set => _resumeSnapshot = value;
    }

    public string CoverLetter
    {
        get => _coverLetter;
        set => _coverLetter = value;
    }

    public Application1()
    {
        _applicationId  = Guid.NewGuid().ToString();
        _jobSeekerId    = "";
        _jobId          = "";
        _appliedDate    = DateTime.Now;
        _status         = ApplicationStatus.PENDING;
        _resumeSnapshot = "";
        _coverLetter    = "";
    }

    public Application1(string jobSeekerId, string jobId, string resumeSnapshot, string coverLetter = "")
    {
        if (string.IsNullOrWhiteSpace(jobSeekerId)) 
            throw new ValidationException("JobSeekerId", "Job seeker ID required.");
        if (string.IsNullOrWhiteSpace(jobId))       
            throw new ValidationException("JobId", "Job ID required.");
        if (string.IsNullOrWhiteSpace(resumeSnapshot)) 
            throw new ResumeNotUploadedException();
        if (coverLetter.Length > JobPosting.JobConfig.MaxCoverLetterLength)
            throw new ValidationException("CoverLetter", $"Cover letter must not exceed {JobPosting.JobConfig.MaxCoverLetterLength} chars.");

        _applicationId  = Guid.NewGuid().ToString();
        _jobSeekerId    = jobSeekerId;
        _jobId          = jobId;
        _appliedDate    = DateTime.Now;
        _status         = ApplicationStatus.PENDING;
        _resumeSnapshot = resumeSnapshot;
        _coverLetter    = coverLetter.Trim();
    }

    public Application1(Application1 existing)
    {
        _applicationId  = Guid.NewGuid().ToString();
        _jobSeekerId    = existing._jobSeekerId;
        _jobId          = existing._jobId;
        _appliedDate    = existing._appliedDate;
        _status         = existing._status;
        _resumeSnapshot = existing._resumeSnapshot;
        _coverLetter    = existing._coverLetter;
    }

    public void UpdateStatus(string newStatus)
    {
        if (!Enum.TryParse<ApplicationStatus>(newStatus, true, out var parsed))
            throw new ValidationException("Status", $"Invalid status: {newStatus}");

        if (_status == parsed) return;

        bool valid = (_status, parsed) switch
        {
            (ApplicationStatus.PENDING,            ApplicationStatus.SHORTLISTED)        => true,
            (ApplicationStatus.PENDING,            ApplicationStatus.REJECTED)           => true,
            (ApplicationStatus.SHORTLISTED,        ApplicationStatus.INTERVIEW_SCHEDULED)=> true,
            (ApplicationStatus.SHORTLISTED,        ApplicationStatus.REJECTED)           => true,
            (ApplicationStatus.SHORTLISTED,        ApplicationStatus.HIRED)              => true,
            (ApplicationStatus.INTERVIEW_SCHEDULED,ApplicationStatus.SHORTLISTED)        => true,
            (ApplicationStatus.INTERVIEW_SCHEDULED,ApplicationStatus.HIRED)              => true,
            (ApplicationStatus.INTERVIEW_SCHEDULED,ApplicationStatus.REJECTED)           => true,
            _ => false
        };

        if (!valid)
            throw new JPNSException($"Cannot transition status from {_status} to {parsed}.");

        
        var previousStatus = _status;
        _status = parsed;

        var seeker = JobPortal.FileStorage.DataStore.FindJobSeekerById(_jobSeekerId);
        if (seeker != null)
        {
            int completedInterviews = 0;
            foreach (var iv in JobPortal.FileStorage.DataStore.Interviews)
            {
                if (iv.ApplicationId == _applicationId && iv.Status == InterviewStatus.COMPLETED)
                {
                    completedInterviews++;
                }
            }

            if (parsed == ApplicationStatus.SHORTLISTED)
            {
                if (previousStatus == ApplicationStatus.INTERVIEW_SCHEDULED)
                {
                    // The candidate just cleared an interview round — advance by 1.
                    seeker.CurrentRound = Math.Min(seeker.TotalRounds, seeker.CurrentRound + 1);
                }
                else
                {
                    // Fresh shortlist from PENDING — never let the count go backward.
                    int shortlistRound = Math.Min(seeker.TotalRounds, completedInterviews + 1);
                    seeker.CurrentRound = Math.Max(seeker.CurrentRound, shortlistRound);
                }
                seeker.RejectedRound = null;

                // Auto-promote to HIRED when all rounds are cleared.
                if (seeker.CurrentRound >= seeker.TotalRounds)
                {
                    _status = ApplicationStatus.HIRED;
                    seeker.CandidateStatus = "HIRED";
                }
                else
                {
                    seeker.CandidateStatus = "SHORTLISTED";
                }
            }
            else if (parsed == ApplicationStatus.INTERVIEW_SCHEDULED)
            {
                // Scheduling always advances one step beyond the current milestone.
                seeker.CurrentRound = Math.Min(seeker.TotalRounds, seeker.CurrentRound + 1);
                seeker.RejectedRound = null;

                // Auto-promote to HIRED if this scheduling fills the last round.
                if (seeker.CurrentRound >= seeker.TotalRounds)
                {
                    _status = ApplicationStatus.HIRED;
                    seeker.CandidateStatus = "HIRED";
                }
                else
                {
                    seeker.CandidateStatus = "INTERVIEW_SCHEDULED";
                }
            }
            else if (parsed == ApplicationStatus.HIRED)
            {
                seeker.CurrentRound = seeker.TotalRounds;
                seeker.RejectedRound = null;
                seeker.CandidateStatus = "HIRED";
            }
            else if (parsed == ApplicationStatus.REJECTED)
            {
                seeker.RejectedRound = seeker.CurrentRound;
                seeker.CandidateStatus = "REJECTED";
            }
            else
            {
                seeker.CandidateStatus = parsed.ToString();
            }

            JobPortal.FileStorage.DataStore.SaveJobSeekers();
        }

        string msg = $"Your application status has changed to: {_status}";
        OnStatusChanged?.Invoke(msg);
        SendNotification(msg);

    }

    public string GetStatusBadge() => _status switch
    {
        ApplicationStatus.PENDING             => "[PENDING]",
        ApplicationStatus.SHORTLISTED         => "[SHORTLISTED]",
        ApplicationStatus.INTERVIEW_SCHEDULED => "[INTERVIEW SCHEDULED]",
        ApplicationStatus.REJECTED            => "[REJECTED]",
        ApplicationStatus.HIRED               => "[HIRED]",
        _                                     => $"[{_status}]"
    };

    public void SendNotification(string message) { }

    public void Display(string jobTitle = "", string company = "")
    {
        Console.WriteLine($"  App ID    : {_applicationId}");
        if (!string.IsNullOrEmpty(jobTitle))   
            Console.WriteLine($"  Job       : {jobTitle}");
        if (!string.IsNullOrEmpty(company))    
            Console.WriteLine($"  Company   : {company}");
        Console.WriteLine($"  Applied   : {_appliedDate:dd MMM yyyy HH:mm}");
        Console.Write("  Status    : ");
        JobPortal.UI.ConsoleHelper.PrintBadge(_status.ToString());
        Console.WriteLine();
        if (!string.IsNullOrEmpty(_coverLetter))
            Console.WriteLine($"  Cover     : {_coverLetter[..Math.Min(_coverLetter.Length, 80)]}...");
    }

    internal static Application1 CreateSeed(string appId, string seekerId, string jobId,
        ApplicationStatus status, string resume, string coverLetter = "", int daysAgo = 10)
    {
        var a = new Application1();
        a._applicationId  = appId;
        a._jobSeekerId    = seekerId;
        a._jobId          = jobId;
        a._appliedDate    = DateTime.Now.AddDays(-daysAgo);
        a._status         = status;
        a._resumeSnapshot = resume;
        a._coverLetter    = coverLetter;
        return a;
    }
}
