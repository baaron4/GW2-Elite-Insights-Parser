using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.ArenaDecoration;

namespace GW2EIEvtcParser.EIData;

public class ArenaDecorationRenderingDescription : AttachedDecorationRenderingDescription
{

    internal ArenaDecorationRenderingDescription(ParsedEvtcLog log, ArenaDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
    {
        Type = Types.Arena;
        IsMechanicOrSkill = false;
    }
}
