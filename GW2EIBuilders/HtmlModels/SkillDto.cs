using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels
{
    internal class SkillDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool Aa { get; set; }
        public bool IsSwap { get; set; }
        public bool NotAccurate { get; set; }

        public static void AssembleSkills(ICollection<SkillItem> skills, Dictionary<string, SkillDto> dict, SkillData skillData)
        {
            foreach (SkillItem skill in skills)
            {
                dict["s" + skill.ID] = new SkillDto()
                {
                    Id = skill.ID,
                    Name = skill.Name,
                    Icon = skill.Icon,
                    Aa = skill.AA,
                    IsSwap = skill.IsSwap,
                    NotAccurate = skillData.IsNotAccurate(skill.ID)
                };
            }
        }

        private static object[] GetSkillData(AbstractCastEvent cl, long phaseStart)
        {
            object[] rotEntry = new object[5];
            double start = (cl.Time - phaseStart) / 1000.0;
            rotEntry[0] = start;
            rotEntry[1] = cl.SkillId;
            rotEntry[2] = cl.ActualDuration;
            rotEntry[3] = (int)cl.Status;
            rotEntry[4] = cl.Acceleration;
            return rotEntry;
        }

        public static List<object[]> BuildRotationData(ParsedEvtcLog log, AbstractActor p, PhaseData phase, Dictionary<long, SkillItem> usedSkills)
        {
            var list = new List<object[]>();
            IReadOnlyList<AbstractCastEvent> casting = p.GetIntersectingCastEvents(log, phase.Start, phase.End);
            foreach (AbstractCastEvent cl in casting)
            {
                if (!usedSkills.ContainsKey(cl.SkillId))
                {
                    usedSkills.Add(cl.SkillId, cl.Skill);
                }

                list.Add(GetSkillData(cl, phase.Start));
            }
            return list;
        }
    }
}
