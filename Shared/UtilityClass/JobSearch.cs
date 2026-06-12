using JobPortal.Shared.Structs;

namespace JobPortal.Features.JobPosting;

internal class JobSearch
{
    public static List<JobListing> SearchJobs(List<JobListing> jobs)
    {
        List<JobListing> result = new List<JobListing>();
        foreach (JobListing j in jobs)
        {
            if (j.IsActive && !j.IsExpired())
            {
                result.Add(j);
            }
        }
        return result;
    }

    public static List<JobListing> SearchJobs(List<JobListing> jobs, string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword)) 
            return SearchJobs(jobs);
        string kw = keyword.Trim().ToLowerInvariant();
        List<JobListing> result = new List<JobListing>();
        foreach (JobListing j in jobs)
        {
            if (!j.IsActive || j.IsExpired())
                continue;
            string title = j.Title.ToLowerInvariant();
            string company = j.Company.ToLowerInvariant();
            string location = j.Location.ToLowerInvariant();
            string description = j.Description.ToLowerInvariant();
            bool match = false;
           if (title.Contains(kw) || company.Contains(kw) || location.Contains(kw) || description.Contains(kw))
            {
                match = true;
            }
            else
            {
                foreach (string s in j.RequiredSkills)
                {
                    if (s.ToLowerInvariant().Contains(kw))
                    {
                        match = true;
                        break;
                    }
                }
            }
            if (match)
            {
                result.Add(j);
            }
        }

        return result;
    }

    public static List<JobListing> SearchJobs(List<JobListing> jobs, string keyword, SalaryRange range)
    {
        List<JobListing> filtered = new List<JobListing>();
        List<JobListing> searchedJobs = SearchJobs(jobs, keyword);
        foreach (JobListing j in searchedJobs)
        {
            if (j.SalaryRange.Min >= range.Min && j.SalaryRange.Max <= range.Max)
            {
                filtered.Add(j);
            }
        }
        return filtered;
    }

    public static List<JobListing> FilterByLocation(List<JobListing> jobs, string location)
    {
        string loc = location.Trim().ToLowerInvariant();
        List<JobListing> result = new List<JobListing>();
        foreach (JobListing j in jobs)
        {
            if (j.IsActive && !j.IsExpired() && j.Location.ToLowerInvariant().Contains(loc))
            {
                result.Add(j);
            }
        }

        return result;
    }

    public static List<JobListing> FilterByExperience(List<JobListing> jobs, int maxYears)
    {
        List<JobListing> result = new List<JobListing>();

        foreach (JobListing j in jobs)
        {
            if (j.IsActive && !j.IsExpired() && j.ExperienceRequired <= maxYears)
            {
                result.Add(j);
            }
        }

        return result;
    }

    public static List<JobListing> FilterByCompany(List<JobListing> jobs, string company)
    {
        string co = company.Trim().ToLowerInvariant();
        List<JobListing> result = new List<JobListing>();
        foreach (JobListing j in jobs)
        {
            if (j.IsActive && !j.IsExpired() && j.Company.ToLowerInvariant().Contains(co))
            {
                result.Add(j);
            }
        }
        return result;
    }
}
