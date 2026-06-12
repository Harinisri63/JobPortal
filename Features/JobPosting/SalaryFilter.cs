using JobPortal.Shared.Structs;

namespace JobPortal.Features.JobPosting;

internal class SalaryFilter : BaseJobFilter
{
    private readonly SalaryRange _range;
    public SalaryFilter(SalaryRange range) => _range = range;

    public override List<JobListing> FilterJobs(List<JobListing> jobs)
    {
        List<JobListing> result = new List<JobListing>();
        foreach (JobListing j in jobs)
        {
            if (j.IsActive && !j.IsExpired() && j.SalaryRange.Min >= _range.Min && j.SalaryRange.Max <= _range.Max)
            {
                result.Add(j);
            }
        }
        for (int i = 0; i < result.Count - 1; i++)
        {
            for (int k = i + 1; k < result.Count; k++)
            {
                if (CalculateRelevance(result[k]) > CalculateRelevance(result[i]))
                {
                    var temp = result[i];
                    result[i] = result[k];
                    result[k] = temp;
                }
            }
        }

        return result;
    }

    public override double CalculateRelevance(JobListing job)
    {
        decimal midpoint = (_range.Min + _range.Max) / 2;
        decimal jobMid   = (job.SalaryRange.Min + job.SalaryRange.Max) / 2;
        decimal range    = _range.Max - _range.Min == 0 ? 1 : _range.Max - _range.Min;
        return (double)(1 - Math.Abs(jobMid - midpoint) / range);
    }

    public new string GetFilterSummary() => $"Salary Filter: {_range.Min:F0} - {_range.Max:F0} LPA";
}
