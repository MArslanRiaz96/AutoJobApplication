using System.Collections.Generic;

namespace AutoJobApplication.Interfaces
{
    public interface ICvService
    {
        byte[] AddSkillsToDocx(byte[] docxData, List<string> skills);
    }
}
