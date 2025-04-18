using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.RectangleDecoration;

namespace GW2EIEvtcParser.EIData;

public class RectangleDecorationRenderingDescription : FormDecorationRenderingDescription
{

    internal RectangleDecorationRenderingDescription(ParsedEvtcLog log, RectangleDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
    {
        Type = Types.Rectangle;
    }
}
