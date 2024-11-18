using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.ImageDecoration;

namespace GW2EIEvtcParser.EIData;

public abstract class ImageDecorationRenderingDescription : AttachedDecorationRenderingDescription
{

    internal ImageDecorationRenderingDescription(ParsedEvtcLog log, ImageDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
    {
    }
}
