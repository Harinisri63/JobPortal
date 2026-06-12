using JobPortal.Shared.Enums;
using JobPortal.Shared.Exceptions;

namespace JobPortal.Features.Interview;

internal class Interview1
{
    private string          _interviewId;
    private string          _applicationId;
    private DateTime        _scheduledAt;
    private string          _mode;
    private string          _meetingLink;
    private string          _venue;
    private InterviewStatus _status;
    private DateTime        _createdAt;

    public string InterviewId
    {
        get => _interviewId;
        set => _interviewId = value;
    }

    public string ApplicationId
    {
        get => _applicationId;
        set => _applicationId = value;
    }

    public DateTime ScheduledAt
    {
        get => _scheduledAt;
        set => _scheduledAt = value;
    }

    public string Mode
    {
        get => _mode;
        set => _mode = value;
    }

    public string MeetingLink
    {
        get => _meetingLink;
        set => _meetingLink = value;
    }

    public string Venue
    {
        get => _venue;
        set => _venue = value;
    }

    public InterviewStatus Status
    {
        get => _status;
        set => _status = value;
    }

    public DateTime CreatedAt
    {
        get => _createdAt;
        set => _createdAt = value;
    }

    public Interview1()
    {
        _interviewId   = Guid.NewGuid().ToString();
        _applicationId = "";
        _scheduledAt   = DateTime.Now.AddDays(1);
        _mode          = "Online";
        _meetingLink   = "";
        _venue         = "";
        _status        = InterviewStatus.SCHEDULED;
        _createdAt     = DateTime.Now;
    }

    public Interview1(string applicationId, DateTime scheduledAt, string mode,string meetingLink = "", string venue = "")
    {
        if (string.IsNullOrWhiteSpace(applicationId))
            throw new InterviewSchedulingException("Application ID is required.");
        ValidateFutureDate(scheduledAt);
        ValidateMode(mode, meetingLink, venue);

        _interviewId   = Guid.NewGuid().ToString();
        _applicationId = applicationId;
        _scheduledAt   = scheduledAt;
        _mode          = mode;
        _meetingLink   = meetingLink.Trim();
        _venue         = venue.Trim();
        _status        = InterviewStatus.SCHEDULED;
        _createdAt     = DateTime.Now;
    }

    public void Reschedule(DateTime newSlot)
    {
        ValidateFutureDate(newSlot);
        if (_status == InterviewStatus.CANCELLED)
            throw new InterviewSchedulingException("Cannot reschedule a cancelled interview.");
        _scheduledAt = newSlot;
    }

    public void UpdateDetails(DateTime scheduledAt, string mode, string meetingLink, string venue)
    {
        if (_status == InterviewStatus.CANCELLED)
            throw new InterviewSchedulingException("Cannot reschedule a cancelled interview.");
        if (_status == InterviewStatus.COMPLETED)
            throw new InterviewSchedulingException("Cannot reschedule a completed interview.");

        ValidateFutureDate(scheduledAt);
        ValidateMode(mode, meetingLink, venue);

        _scheduledAt = scheduledAt;
        _mode = mode;
        _meetingLink = meetingLink.Trim();
        _venue = venue.Trim();
    }

    public void Cancel()
    {
        if (_status == InterviewStatus.COMPLETED)
            throw new InterviewSchedulingException("Cannot cancel a completed interview.");
        _status = InterviewStatus.CANCELLED;
    }

    public void MarkCompleted()
    {
        if (_status == InterviewStatus.CANCELLED)
            throw new InterviewSchedulingException("Cannot complete a cancelled interview.");
        _status = InterviewStatus.COMPLETED;
    }

    public void Display()
    {
        Console.WriteLine($"  Interview ID : {_interviewId}");
        Console.WriteLine($"  Scheduled At : {_scheduledAt:dd MMM yyyy HH:mm}");
        Console.WriteLine($"  Mode         : {_mode}");
        if (_mode.Equals("Online",  StringComparison.OrdinalIgnoreCase))
            Console.WriteLine($"  Meeting Link : {_meetingLink}");
        if (_mode.Equals("Offline", StringComparison.OrdinalIgnoreCase))
            Console.WriteLine($"  Venue        : {_venue}");
        Console.Write("  Status       : ");
        JobPortal.UI.ConsoleHelper.PrintBadge(_status.ToString());
        Console.WriteLine();
    }

    private static void ValidateFutureDate(DateTime dt)
    {
        if (dt <= DateTime.Now)
            throw new InterviewSchedulingException("Interview must be scheduled for a future date and time.");
    }

    private static void ValidateMode(string mode, string meetingLink, string venue)
    {
        if (!mode.Equals("Online", StringComparison.OrdinalIgnoreCase) &&
            !mode.Equals("Offline", StringComparison.OrdinalIgnoreCase))
            throw new InterviewSchedulingException("Mode must be Online or Offline.");
        if (mode.Equals("Online",  StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(meetingLink))
            throw new InterviewSchedulingException("Meeting link is required for Online mode.");
        if (mode.Equals("Offline", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(venue))
            throw new InterviewSchedulingException("Venue is required for Offline mode.");
    }
}
