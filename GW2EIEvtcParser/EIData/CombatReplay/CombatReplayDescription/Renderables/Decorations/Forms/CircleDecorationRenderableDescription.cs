using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class CircleDecorationRenderableDescription : FormDecorationRenderableDescription
    {
        public uint Radius { get; }
        public uint MinRadius { get; }

        internal CircleDecorationRenderableDescription(ParsedEvtcLog log, CircleDecoration decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs) : base(log, decoration, map, usedSkills, usedBuffs)
        {
            Type = "Circle";
            Radius = decoration.Radius;
            MinRadius = decoration.MinRadius;
        }
    }

}
