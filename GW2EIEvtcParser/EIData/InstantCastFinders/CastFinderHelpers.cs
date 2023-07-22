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
        internal static IEnumerable<T> FindRelatedEvents<T>(IEnumerable<T> events, long time, long epsilon = ServerDelayConstant) where T : AbstractTimeCombatEvent
        {
            return events.Where(evt => Math.Abs(evt.Time - time) < epsilon);
        }

        internal static bool HasRelatedHit(CombatData combatData, long skillID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(combatData.GetDamageData(skillID), time, epsilon)
                .Any(hit => hit.CreditedFrom == agent);
        }

        internal static bool HasPreviousCast(CombatData combatData, long skillID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(combatData.GetAnimatedCastData(skillID), time, epsilon)
                .Any(cast => cast.Caster == agent && cast.Time <= time);
        }

        internal static bool IsCasting(CombatData combatData, long skillID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return combatData.GetAnimatedCastData(skillID)
                .Any(cast => cast.Caster == agent && cast.Time - epsilon <= time && cast.EndTime + epsilon >= time);
        }

        internal static bool HasGainedBuff(CombatData combatData, long buffID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(combatData.GetBuffData(buffID).OfType<BuffApplyEvent>(), time, epsilon)
                .Any(apply => apply.To == agent);
        }

        internal static bool HasGainedBuff(CombatData combatData, long buffID, AgentItem agent, long time, AgentItem source, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(combatData.GetBuffData(buffID).OfType<BuffApplyEvent>(), time, epsilon)
                .Any(apply => apply.To == agent && apply.CreditedBy == source);
        }

        internal static bool HasGainedBuff(CombatData combatData, long buffID, AgentItem agent, long time, long appliedDuration, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(combatData.GetBuffData(buffID).OfType<BuffApplyEvent>(), time, epsilon)
                .Any(apply => apply.To == agent && Math.Abs(apply.AppliedDuration - appliedDuration) < epsilon);
        }

        internal static bool HasGainedBuff(CombatData combatData, long buffID, AgentItem agent, long time, long appliedDuration, AgentItem source, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(combatData.GetBuffData(buffID).OfType<BuffApplyEvent>(), time, epsilon)
                .Any(apply => apply.To == agent && apply.CreditedBy == source && Math.Abs(apply.AppliedDuration - appliedDuration) < epsilon);
        }

        internal static bool HasLostBuff(CombatData combatData, long buffID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(combatData.GetBuffData(buffID).OfType<BuffRemoveAllEvent>(), time, epsilon)
                .Any(remove => remove.To == agent);
        }

        internal static bool HasLostBuffStack(CombatData combatData, long buffID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(combatData.GetBuffData(buffID).OfType<AbstractBuffRemoveEvent>(), time, epsilon)
                .Any(remove => remove.To == agent);
        }

        internal static bool HasSpawnedMinion(AgentData agentData, MinionID minion, AgentItem master, long time, long epsilon = ServerDelayConstant)
        {
            return agentData.GetNPCsByID(minion)
                .Any(agent => agent.GetFinalMaster() == master && Math.Abs(agent.FirstAware - time) < epsilon);
        }

        internal static bool HasRelatedEffect(CombatData combatData, string effectGUID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            if (combatData.TryGetEffectEventsByGUID(effectGUID, out IReadOnlyList<EffectEvent> effectEvents))
            {
                return FindRelatedEvents(effectEvents, time, epsilon).Any(effect => effect.Src == agent);
            }
            return false;
        }

        internal static bool HasRelatedEffectDst(CombatData combatData, string effectGUID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            if (combatData.TryGetEffectEventsByGUID(effectGUID, out IReadOnlyList<EffectEvent> effectEvents))
            {
                return FindRelatedEvents(effectEvents, time, epsilon).Any(effect => effect.Dst == agent);
            }
            return false;
        }

        internal static bool HasExtendedBuff(CombatData combatData, long buffID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(combatData.GetBuffData(buffID).OfType<BuffExtensionEvent>(), time, epsilon)
                .Any(apply => apply.To == agent);
        }

        internal static bool HasExtendedBuff(CombatData combatData, long buffID, AgentItem agent, long time, AgentItem source, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(combatData.GetBuffData(buffID).OfType<BuffExtensionEvent>(), time, epsilon)
                .Any(apply => apply.To == agent && apply.CreditedBy == source);
        }

        internal static bool HasExtendedBuff(CombatData combatData, long buffID, AgentItem agent, long time, long extendedDuration, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(combatData.GetBuffData(buffID).OfType<BuffExtensionEvent>(), time, epsilon)
                .Any(apply => apply.To == agent && Math.Abs(apply.ExtendedDuration - extendedDuration) < epsilon);
        }

        internal static bool HasExtendedBuff(CombatData combatData, long buffID, AgentItem agent, long time, long extendedDuration, AgentItem source, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(combatData.GetBuffData(buffID).OfType<BuffExtensionEvent>(), time, epsilon)
                .Any(apply => apply.To == agent && apply.CreditedBy == source && Math.Abs(apply.ExtendedDuration - extendedDuration) < epsilon);
        }
    }
}
