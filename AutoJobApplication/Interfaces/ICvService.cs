using System.Collections.Generic;

namespace AutoJobApplication.Interfaces
{
    public interface ICvService
    {
        byte[] AddSkillsToCv(byte[] fileData, List<string> skills);
    }
}
