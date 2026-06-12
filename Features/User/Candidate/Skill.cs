namespace JobPortal.Features.User.Candidate
{
    public class Skill
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string? Category { get; set; }
    }

    public class CandidateSkill
    {
        public int CandidateSkillId { get; set; }
        public int UserId { get; set; }
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string ProficiencyLevel { get; set; } = "Beginner";
        public int? YearsExperience { get; set; }
    }
}