using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.PolygonDecoration;

namespace GW2EIEvtcParser.EIData;

public class PolygonDecorationRenderingDescription : FormDecorationRenderingDescription
{

    internal PolygonDecorationRenderingDescription(ParsedEvtcLog log, PolygonDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
    {
        Type = Types.Polygon;
    }
}
