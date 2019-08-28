using System.Collections.Generic;
using LuckParser.Parser.ParsedData;

namespace LuckParser.Builders.HtmlModels
{
    public class SkillDto
    {
        public long Id;
        public string Name;
        public string Icon;
        public bool Aa;

        public static void AssembleSkills(ICollection<SkillItem> skills, Dictionary<string, SkillDto> dict)
        {
            foreach (SkillItem skill in skills)
            {
                dict["s" + skill.ID] = new SkillDto()
                {
                    Id = skill.ID,
                    Name = skill.Name,
                    Icon = skill.Icon,
                    Aa = skill.AA
                };
            }
        }
    }
}
