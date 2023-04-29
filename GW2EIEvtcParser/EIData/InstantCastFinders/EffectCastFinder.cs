using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal class EffectCastFinder : InstantCastFinder
    {
        public delegate bool EffectCastChecker(EffectEvent evt, CombatData combatData, AgentData agentData, SkillData skillData);
        private EffectCastChecker _triggerCondition { get; set; }

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
            UsingEnableInternal((combatData) => combatData.HasEffectData);
            _effectGUID = effectGUID;
        }

        internal EffectCastFinder UsingChecker(EffectCastChecker checker)
        {
            _triggerCondition = checker;
            return this;
        }

        internal EffectCastFinder UsingSrcBaseSpecChecker(Spec spec)
        {
            return UsingChecker((evt, combatData, agentData, skillData) => evt.Src.BaseSpec == spec);
        }

        internal EffectCastFinder UsingDstBaseSpecChecker(ParserHelper.Spec spec)
        {
            return UsingChecker((evt, combatData, agentData, skillData) => evt.Dst.BaseSpec == spec);
        }
        
        internal EffectCastFinder UsingSrcSpecChecker(Spec spec)
        {
            return UsingChecker((evt, combatData, agentData, skillData) => evt.Src.Spec == spec);
        }

        internal EffectCastFinder UsingDstSpecChecker(ParserHelper.Spec spec)
        {
            return UsingChecker((evt, combatData, agentData, skillData) => evt.Dst.Spec == spec);
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
                        if (_triggerCondition == null || _triggerCondition(effectEvent, combatData, agentData, skillData))
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
