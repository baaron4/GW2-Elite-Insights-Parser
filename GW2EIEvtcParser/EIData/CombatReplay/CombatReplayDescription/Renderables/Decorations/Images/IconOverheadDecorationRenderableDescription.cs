using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class IconOverheadDecorationRenderableDescription : IconDecorationRenderableDescription
    {

        internal IconOverheadDecorationRenderableDescription(ParsedEvtcLog log, IconOverheadDecoration decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs) : base(log, decoration, map, usedSkills, usedBuffs)
        {
            Type = "IconOverheadDecoration";
            if (decoration.IsSquadMarker)
            {
                Type = "OverheadSquadMarkerDecoration";
            }
        }
    }

}
