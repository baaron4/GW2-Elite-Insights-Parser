using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public abstract class FormDecorationCombatReplayDescription : GenericAttachedDecorationCombatReplayDescription
    {
        public bool Fill { get; }
        public int GrowingEnd { get; }
        public bool GrowingReverse { get; }
        public string Color { get; }

        internal FormDecorationCombatReplayDescription(ParsedEvtcLog log, FormDecoration decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs) : base(log, decoration, map, usedSkills, usedBuffs)
        {
            Fill = decoration.Filled;
            Color = decoration.Color;
            GrowingEnd = decoration.GrowingEnd;
            GrowingReverse = decoration.GrowingReverse;
        }

    }

}
