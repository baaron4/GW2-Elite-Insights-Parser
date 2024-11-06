using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.GenericIconDecoration;

namespace GW2EIEvtcParser.EIData;

public abstract class GenericIconDecorationRenderingDescription : GenericAttachedDecorationRenderingDescription
{

    internal GenericIconDecorationRenderingDescription(ParsedEvtcLog log, GenericIconDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
    {
    }
}
