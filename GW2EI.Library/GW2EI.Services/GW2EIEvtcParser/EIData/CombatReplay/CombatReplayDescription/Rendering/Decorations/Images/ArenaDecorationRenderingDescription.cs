using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.ArenaDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class ArenaDecorationRenderingDescription : AttachedDecorationRenderingDescription
{

    internal ArenaDecorationRenderingDescription(ParsedEvtcLog log, ArenaDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature, StringToUlongHandler stringToUlongHandler) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature, stringToUlongHandler)
    {
        Type = Types.Arena;
        IsMechanicOrSkill = false;
    }
}
