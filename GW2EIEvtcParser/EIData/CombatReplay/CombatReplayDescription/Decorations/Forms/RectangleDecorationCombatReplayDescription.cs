using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class RectangleDecorationCombatReplayDescription : FormDecorationCombatReplayDescription
    {
        public uint Height { get; }
        public uint Width { get; }

        internal RectangleDecorationCombatReplayDescription(ParsedEvtcLog log, RectangleDecoration decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs) : base(log, decoration, map, usedSkills, usedBuffs)
        {
            Type = "Rectangle";
            Width = decoration.Width;
            Height = decoration.Height;
        }
    }
}
