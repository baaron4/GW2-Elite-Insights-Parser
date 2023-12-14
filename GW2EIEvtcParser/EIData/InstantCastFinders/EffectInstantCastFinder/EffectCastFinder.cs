using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal class EffectCastFinder : CheckedCastFinder<EffectEvent>
    {
        private readonly string _effectGUID;

        protected virtual Dictionary<AgentItem, List<EffectEvent>> GetEffectEventDict(EffectGUIDEvent effectGUIDEvent, CombatData combatData)
        {
            return combatData.GetEffectEventsByEffectID(effectGUIDEvent.ContentID).GroupBy(x => x.Src).ToDictionary(x => x.Key, x => x.ToList());
        }

        protected virtual AgentItem GetAgent(EffectEvent effectEvent)
        {
            return effectEvent.Src;
        }

        public EffectCastFinder(long skillID, string effectGUID) : base(skillID)
        {
            UsingNotAccurate(true); // TODO: confirm if culling is server side logic
            UsingEnable((combatData) => combatData.HasEffectData);
            _effectGUID = effectGUID;
        }

        internal EffectCastFinder UsingSrcBaseSpecChecker(Spec spec)
        {
            UsingChecker((evt, combatData, agentData, skillData) => evt.Src.BaseSpec == spec);
            return this;
        }

        internal EffectCastFinder UsingDstBaseSpecChecker(Spec spec)
        {
            UsingChecker((evt, combatData, agentData, skillData) => evt.Dst.BaseSpec == spec);
            return this;
        }
        
        internal EffectCastFinder UsingSrcSpecChecker(Spec spec)
        {
            UsingChecker((evt, combatData, agentData, skillData) => evt.Src.Spec == spec);
            return this;
        }

        internal EffectCastFinder UsingDstSpecChecker(Spec spec)
        {
            UsingChecker((evt, combatData, agentData, skillData) => evt.Dst.Spec == spec);
            return this;
        }

        internal EffectCastFinder UsingSecondaryEffectChecker(string effectGUID, long timeOffset = 0, long epsilon = ServerDelayConstant)
        {
            UsingChecker((evt, combatData, agentData, skillData) =>
            {
                if (combatData.TryGetEffectEventsByGUID(effectGUID, out IReadOnlyList<EffectEvent> effectEvents))
                {
                    return effectEvents.Any(other => GetAgent(other) == GetAgent(evt) && Math.Abs(other.Time - timeOffset - evt.Time) < epsilon);
                }
                return false;
            });
            return this;
        }

        public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var res = new List<InstantCastEvent>();
            EffectGUIDEvent effectGUIDEvent = combatData.GetEffectGUIDEvent(_effectGUID);
            if (effectGUIDEvent != null)
            {
                Dictionary<AgentItem, List<EffectEvent>> effects = GetEffectEventDict(effectGUIDEvent, combatData);
                foreach (KeyValuePair<AgentItem, List<EffectEvent>> pair in effects)
                {
                    long lastTime = int.MinValue;
                    foreach (EffectEvent effectEvent in pair.Value)
                    {
                        if (effectEvent.Time - lastTime < ICD)
                        {
                            lastTime = effectEvent.Time;
                            continue;
                        }
                        if (CheckCondition(effectEvent, combatData, agentData, skillData))
                        {
                            lastTime = effectEvent.Time;
                            AgentItem caster = GetAgent(effectEvent);
                            res.Add(new InstantCastEvent(GetTime(effectEvent, caster, combatData), skillData.Get(SkillID), caster));
                        }
                    }
                }
            }
            return res;
        }
    }
}
