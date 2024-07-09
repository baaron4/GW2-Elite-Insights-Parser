using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class DoughnutDecorationCombatReplayDescription : FormDecorationCombatReplayDescription
    {
        public uint InnerRadius { get; }
        public uint OuterRadius { get; }

        internal DoughnutDecorationCombatReplayDescription(ParsedEvtcLog log, DoughnutDecoration decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs) : base(log, decoration, map, usedSkills, usedBuffs)
        {
            Type = "Doughnut";
            OuterRadius = decoration.OuterRadius;
            InnerRadius = decoration.InnerRadius;
        }

    }
}
