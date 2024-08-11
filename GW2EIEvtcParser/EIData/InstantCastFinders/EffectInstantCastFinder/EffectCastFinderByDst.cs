using System;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class EffectCastFinderByDst : EffectCastFinder
    {
        protected override AgentItem GetKeyAgent(EffectEvent effectEvent)
        {
            return effectEvent.IsAroundDst ? effectEvent.Dst : ParserHelper._unknownAgent;
        }

        protected override bool DebugEffectChecker(EffectEvent evt, CombatData combatData, AgentData agentData, SkillData skillData)
        {
            var test = combatData.GetEffectEventsBySrc(evt.Dst).Where(x => Math.Abs(x.Time - evt.Time) <= ParserHelper.ServerDelayConstant && x.EffectID != evt.EffectID).ToList();
            var testGUIDs = test.Select(x => combatData.GetEffectGUIDEvent(x.EffectID)).Select(x => x.HexContentGUID).ToList();
            var test2 = combatData.GetEffectEventsByDst(evt.Dst).Where(x => Math.Abs(x.Time - evt.Time) <= ParserHelper.ServerDelayConstant && x.EffectID != evt.EffectID).ToList();
            var test2GUIDs = test2.Select(x => combatData.GetEffectGUIDEvent(evt.EffectID)).Select(x => x.HexContentGUID).ToList();
            return true;
        }

        public EffectCastFinderByDst(long skillID, string effectGUID) : base(skillID, effectGUID)
        {
        }
    }
}
