namespace JobPortal.Features.JobPosting;

internal class SkillsFilter : BaseJobFilter
{
    private readonly List<string> _requiredSkills;

    public SkillsFilter(List<string> requiredSkills)
    {
        _requiredSkills = new List<string>();
        foreach (string skill in requiredSkills)
        {
            _requiredSkills.Add(
                skill.Trim().ToLowerInvariant());
        }
    }

    public override List<JobListing> FilterJobs(List<JobListing> jobs)
    {
        List<JobListing> matchedJobs = new List<JobListing>();
        foreach (JobListing job in jobs)
        {
            if (!job.IsActive || job.IsExpired())
            {
                continue;
            }
            bool skillMatched = false;
            foreach (string skill in _requiredSkills)
            {
                foreach (string rs in job.RequiredSkills)
                {
                    if (rs.ToLowerInvariant().Contains(skill))
                    {
                        skillMatched = true;
                        break;
                    }
                }
                if (skillMatched)
                {
                    break;
                }
            }
            if (skillMatched)
            {
                matchedJobs.Add(job);
            }
        }
        matchedJobs.Sort((a, b) => CalculateRelevance(b).CompareTo(CalculateRelevance(a)));

        return matchedJobs;
    }

    public override double CalculateRelevance(JobListing job)
    {
        int matches = 0;
        foreach (string skill in _requiredSkills)
        {
            bool found = false;
            foreach (string rs in job.RequiredSkills)
            {
                if (rs.ToLowerInvariant().Contains(skill))
                {
                    found = true;
                    break;
                }
            }
            if (found)
            {
                matches++;
            }
        }
        if (_requiredSkills.Count == 0)
        {
            return 0;
        }
        return (double)matches / _requiredSkills.Count;
    }
}
