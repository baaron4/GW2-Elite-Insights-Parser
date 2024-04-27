using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public class IconOverheadDecorationCombatReplayDescription : IconDecorationCombatReplayDescription
    {

        internal IconOverheadDecorationCombatReplayDescription(ParsedEvtcLog log, IconOverheadDecoration decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs) : base(log, decoration, map, usedSkills, usedBuffs)
        {
            Type = "IconOverheadDecoration";
            if (decoration.IsSquadMarker)
            {
                Type = "OverheadSquadMarkerDecoration";
            }
        }
    }

}
