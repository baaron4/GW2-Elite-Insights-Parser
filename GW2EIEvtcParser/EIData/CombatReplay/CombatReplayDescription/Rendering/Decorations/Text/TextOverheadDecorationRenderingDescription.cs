using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.TextOverheadDecoration;

namespace GW2EIEvtcParser.EIData;

public class TextOverheadDecorationRenderingDescription : TextDecorationRenderingDescription
{
    internal TextOverheadDecorationRenderingDescription(ParsedEvtcLog log, TextOverheadDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
    {
        Type = Types.TextOverhead;
    }
}
