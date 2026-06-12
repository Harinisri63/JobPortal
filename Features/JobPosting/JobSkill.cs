using System;

namespace JobPortal.Features.JobPosting
{
    public class JobSkill
    {
        public int JobSkillId { get; set; }

        public int JobId { get; set; }

        public int SkillId { get; set; }

        public string JobTitle { get; set; } = string.Empty;

        public string SkillName { get; set; } = string.Empty;

        public bool IsRequired { get; set; } = true;
    }
}