using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.Builders.HtmlModels
{
    public class SkillDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool Aa { get; set; }

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
