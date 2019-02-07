using LuckParser.Models.ParseModels;
using System.Collections.Generic;

namespace LuckParser.Models.HtmlModels
{  
    public class SkillDto
    {
        public long Id;
        public string Name;
        public string Icon;
        public bool Aa;

        public static List<SkillDto> AssembleSkills(ICollection<SkillItem> skills)
        {
            List<SkillDto> dtos = new List<SkillDto>();
            foreach (SkillItem skill in skills)
            {
                SkillDto dto = new SkillDto()
                {
                    Id = skill.ID,
                    Name = skill.Name,
                    Icon = skill.Icon,
                    Aa = skill.AA
                };
                dtos.Add(dto);
            }
            return dtos;
        }
    }
}
