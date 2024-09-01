using System.Collections.Generic;

namespace AutoJobApplication.Interfaces
{
    public interface ISkillService
    {
        void AddSkills(List<string> skills);
        List<string> GetSkills();
    }
}
