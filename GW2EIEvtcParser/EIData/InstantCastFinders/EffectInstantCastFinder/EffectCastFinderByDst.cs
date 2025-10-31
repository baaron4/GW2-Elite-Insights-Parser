using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class EffectCastFinderByDst : EffectCastFinder
{
    protected override AgentItem GetKeyAgent(EffectEvent effectEvent)
    {
        return effectEvent.Dst;
    }
    protected override AgentItem GetOtherKeyAgent(EffectEvent effectEvent)
    {
        return effectEvent.Src;
    }
    protected override bool DebugEffectChecker(EffectEvent evt, CombatData combatData, AgentData agentData, SkillData skillData)
    {
        var test = combatData.GetEffectEventsBySrc(evt.Dst).Where(x => Math.Abs(x.Time - evt.Time) <= ParserHelper.ServerDelayConstant && x.EffectID != evt.EffectID);
        var testGUIDs = test.Select(x => x.GUIDEvent);
        var test2 = combatData.GetEffectEventsByDst(evt.Dst).Where(x => Math.Abs(x.Time - evt.Time) <= ParserHelper.ServerDelayConstant && x.EffectID != evt.EffectID);
        var test2GUIDs = test2.Select(x => x.GUIDEvent);
        return true;
    }

    public EffectCastFinderByDst(long skillID, GUID effectGUID) : base(skillID, effectGUID)
    {
    }
}
