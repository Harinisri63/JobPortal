using JobPortal.Shared.Exceptions;
using JobPortal.FileStorage;

namespace JobPortal.Features.Interview;

internal class InterviewScheduler
{
    private readonly FileStorageService<Interview1> _storage;
    private List<Interview1> _interviews;

    public InterviewScheduler()
    {
        _storage = new FileStorageService<Interview1>("interviews.json");
        _interviews = _storage.LoadData();
    }

    public Interview1 Schedule(string applicationId, DateTime scheduledAt, string mode, string meetingLink = "", string venue = "")
    {
        Interview1? existing = null;
        foreach (Interview1 iv in _interviews)
        {
            if (iv.ApplicationId == applicationId &&
                iv.Status == Shared.Enums.InterviewStatus.SCHEDULED)
            {
                existing = iv;
                break;
            }
        }

        if (existing != null)
        {
            existing.UpdateDetails(scheduledAt, mode, meetingLink, venue);
            DataStore.SaveInterviews();
            return existing;
        }

        var interview = new Interview1(applicationId, scheduledAt, mode, meetingLink, venue);
        _interviews.Add(interview);
        DataStore.SaveInterviews();
        return interview;
    }

    public Interview1? GetByApplication(string applicationId)
    {
        foreach (Interview1 interview in _interviews)
        {
            if (interview.ApplicationId == applicationId)
            {
                return interview;
            }
        }
        return null;
    }

    public List<Interview1> GetByUser(string applicationId)
    {
        List<Interview1> result = new List<Interview1>();
        foreach (Interview1 interview in _interviews)
        {
            if (interview.ApplicationId == applicationId)
            {
                result.Add(interview);
            }
        }
        return result;
    }

    public List<Interview1> GetAll() => _interviews;

    public void Cancel(string interviewId)
    {
        Interview1? found = null;
        foreach (Interview1 interview in _interviews)
        {
            if (interview.InterviewId == interviewId)
            {
                found = interview;
                break;
            }
        }
        if (found == null)
            throw new JPNSException("Interview not found.");
        found.Cancel();
        DataStore.SaveInterviews();
    }

    public void MarkCompleted(string interviewId)
    {
        Interview1? found = null;
        foreach (Interview1 interview in _interviews)
        {
            if (interview.InterviewId == interviewId)
            {
                found = interview;
                break;
            }
        }
        if (found == null)
            throw new JPNSException("Interview not found.");
        found.MarkCompleted();
        DataStore.SaveInterviews();
    }

    public void Reload() => _interviews = DataStore.Interviews;
}