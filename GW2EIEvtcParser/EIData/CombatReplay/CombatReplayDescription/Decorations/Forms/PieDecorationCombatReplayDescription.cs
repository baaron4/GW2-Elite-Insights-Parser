using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class PieDecorationCombatReplayDescription : CircleDecorationCombatReplayDescription
    {
        public float OpeningAngle { get; set; }

        internal PieDecorationCombatReplayDescription(ParsedEvtcLog log, PieDecoration decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs) : base(log, decoration, map, usedSkills, usedBuffs)
        {
            Type = "Pie";
            OpeningAngle = decoration.OpeningAngle;
        }

    }
}
