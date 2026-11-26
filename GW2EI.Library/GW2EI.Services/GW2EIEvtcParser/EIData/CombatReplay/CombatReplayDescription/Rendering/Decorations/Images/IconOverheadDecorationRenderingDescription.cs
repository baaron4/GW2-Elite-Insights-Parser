using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.IconOverheadDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class IconOverheadDecorationRenderingDescription : IconDecorationRenderingDescription
{

    internal IconOverheadDecorationRenderingDescription(ParsedEvtcLog log, IconOverheadDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature, StringToUlongHandler stringToUlongHandler) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature, stringToUlongHandler)
    {
        Type = Types.IconOverhead;
        if (decoration.IsSquadMarker)
        {
            Type = Types.SquadMarkerOverhead;
        }
    }
}
