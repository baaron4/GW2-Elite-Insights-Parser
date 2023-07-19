using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels.HTMLMetaData
{
    internal class SkillDto : AbstractSkillDto
    {
        public bool Aa { get; set; }
        public bool IsSwap { get; set; }
        public bool NotAccurate { get; set; }
        public bool TraitProc { get; set; }
        public bool GearProc { get; set; }

        public SkillDto(SkillItem skill, ParsedEvtcLog log) : base(skill, log)
        {
            Aa = skill.AA;
            IsSwap = skill.IsSwap;
            NotAccurate = log.SkillData.IsNotAccurate(skill.ID);
            GearProc = log.SkillData.IsGearProc(skill.ID);
            TraitProc = log.SkillData.IsTraitProc(skill.ID);
        }

        public static void AssembleSkills(ICollection<SkillItem> skills, Dictionary<string, SkillDto> dict, ParsedEvtcLog log)
        {
            foreach (SkillItem skill in skills)
            {
                dict["s" + skill.ID] = new SkillDto(skill, log);
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

        public static List<object[]> BuildRotationData(ParsedEvtcLog log, AbstractSingleActor p, PhaseData phase, Dictionary<long, SkillItem> usedSkills)
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
