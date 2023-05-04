using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal static class CastFinderHelpers
    {
        internal static IEnumerable<T> FindRelatedEvents<T>(IEnumerable<T> events, long time, long epsilon = ServerDelayConstant) where T: AbstractTimeCombatEvent
        {
            return events.Where(evt => Math.Abs(evt.Time - time) < epsilon);
        }

        internal static bool HasRelatedHit(CombatData combatData, long skillID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(combatData.GetDamageData(skillID), time)
                .Any(hit => hit.CreditedFrom == agent);
        }

        internal static bool HasPreviousCast(CombatData combatData, long skillID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(combatData.GetAnimatedCastData(skillID), time)
                .Any(cast => cast.Caster == agent && cast.Time <= time);
        }

        internal static bool HasSelfAppliedBuff(CombatData combatData, long buffID, AgentItem agent, long time, long minDuration = 0, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(combatData.GetBuffData(buffID).OfType<BuffApplyEvent>(), time)
                .Any(apply => apply.By == agent && apply.To == agent && apply.AppliedDuration >= minDuration);      
        }

        internal static bool HasSelfAppliedStackingBuff(CombatData combatData, long buffID, long minStacks, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return minStacks <= FindRelatedEvents(combatData.GetBuffData(buffID).OfType<BuffApplyEvent>(), time)
                .Count(apply => apply.By == agent && apply.To == agent);      
        }

        internal static bool HasSpawnedMinion(AgentData agentData, MinionID minion, AgentItem master, long time, long epsilon = ServerDelayConstant)
        {
            return agentData.GetNPCsByID(minion)
                .Any(agent => agent.GetFinalMaster() == master && Math.Abs(agent.FirstAware - time) < epsilon);
        }
    }
}
