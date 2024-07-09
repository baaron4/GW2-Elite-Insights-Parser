using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class DoughnutDecorationRenderableDescription : FormDecorationRenderableDescription
    {
        public uint InnerRadius { get; }
        public uint OuterRadius { get; }

        internal DoughnutDecorationRenderableDescription(ParsedEvtcLog log, DoughnutDecoration decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs) : base(log, decoration, map, usedSkills, usedBuffs)
        {
            Type = "Doughnut";
            OuterRadius = decoration.OuterRadius;
            InnerRadius = decoration.InnerRadius;
        }

    }
}
