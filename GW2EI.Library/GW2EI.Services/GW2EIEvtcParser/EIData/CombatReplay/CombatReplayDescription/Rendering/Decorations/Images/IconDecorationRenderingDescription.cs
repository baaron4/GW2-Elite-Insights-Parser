using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.IconDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class IconDecorationRenderingDescription : ImageDecorationRenderingDescription
{
    public readonly string? Text;
    internal IconDecorationRenderingDescription(ParsedEvtcLog log, IconDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature, StringToUlongHandler stringToUlongHandler) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature, stringToUlongHandler)
    {
        Type = Types.Icon;
        if (decoration.IsSquadMarker)
        {
            Type = Types.SquadMarker;
        }
        if (decoration.Text != null)
        {
            Text = decoration.Text;
        }
    }
}
