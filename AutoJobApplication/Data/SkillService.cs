using AutoJobApplication.Interfaces;
using System.Collections.Generic;

namespace AutoJobApplication.Data
{
    public class SkillService : ISkillService
    {
        private readonly List<string> _skills = new();

        public void AddSkills(List<string> skills)
        {
            if (skills != null)
            {
                _skills.AddRange(skills);
            }
        }

        public List<string> GetSkills()
        {
            return _skills;
        }
    }
}
