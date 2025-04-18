using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.ActorOrientationDecoration;

namespace GW2EIEvtcParser.EIData;

public class ActorOrientationDecorationRenderingDescription : AttachedDecorationRenderingDescription
{

    internal ActorOrientationDecorationRenderingDescription(ParsedEvtcLog log, ActorOrientationDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
    {
        Type = Types.ActorOrientation;
        IsMechanicOrSkill = false;
    }

}
