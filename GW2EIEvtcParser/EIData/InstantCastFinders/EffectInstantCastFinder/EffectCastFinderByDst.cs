using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class EffectCastFinderByDst : EffectCastFinder
{
    protected override AgentItem? GetKeyAgent(EffectEvent effectEvent)
    {
        return effectEvent.IsAroundDst ? effectEvent.Dst : ParserHelper._unknownAgent;
    }

    //TODO(Rennorb) @perf @cleanup: move to tests? 
    protected override bool DebugEffectChecker(EffectEvent evt, CombatData combatData, AgentData agentData, SkillData skillData)
    {
        var test = combatData.GetEffectEventsBySrc(evt.Dst).Where(x => Math.Abs(x.Time - evt.Time) <= ParserHelper.ServerDelayConstant && x.EffectID != evt.EffectID).ToList();
        var testGUIDs = test.Select(x => x.GUIDEvent.ContentGUID).ToList();
        var test2 = combatData.GetEffectEventsByDst(evt.Dst).Where(x => Math.Abs(x.Time - evt.Time) <= ParserHelper.ServerDelayConstant && x.EffectID != evt.EffectID).ToList();
        var test2GUIDs = test2.Select(x => x.GUIDEvent.ContentGUID).ToList();
        return true;
    }

    public EffectCastFinderByDst(long skillID, GUID effectGUID) : base(skillID, effectGUID)
    {
    }
}
