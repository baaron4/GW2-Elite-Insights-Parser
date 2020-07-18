using System.Collections.Generic;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

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

        public static List<object[]> BuildRotationData(ParsedLog log, AbstractActor p, int phaseIndex, Dictionary<long, SkillItem> usedSkills)
        {
            var list = new List<object[]>();

            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];
            List<AbstractCastEvent> casting = p.GetIntersectingCastLogs(log, phase.Start, phase.End);
            foreach (AbstractCastEvent cl in casting)
            {
                if (!usedSkills.ContainsKey(cl.SkillId))
                {
                    usedSkills.Add(cl.SkillId, cl.Skill);
                }

                list.Add(ActorDetailsDto.GetSkillData(cl, phase.Start));
            }
            return list;
        }
    }
}
