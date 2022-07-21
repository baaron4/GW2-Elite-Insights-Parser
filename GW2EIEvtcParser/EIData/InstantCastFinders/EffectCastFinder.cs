using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class EffectCastFinder : InstantCastFinder
    {
        public delegate bool EffectCastChecker(EffectEvent evt, CombatData combatData);
        private readonly EffectCastChecker _triggerCondition;

        private readonly string _effectGUID;

        protected virtual Dictionary<AgentItem, List<EffectEvent>> GetEffectEventDict(EffectGUIDEvent effectGUIDEvent, CombatData combatData)
        {
            return combatData.GetEffectEvents(effectGUIDEvent.ContentID).GroupBy(x => x.Src).ToDictionary(x => x.Key, x => x.ToList());
        }

        protected virtual AgentItem GetAgent(EffectEvent effectEvent)
        {
            return effectEvent.Src;
        }

        public EffectCastFinder(long skillID, string effectGUID, long icd, EffectCastChecker checker = null) : base(skillID, icd)
        {
            NotAccurate = true; // TODO: confirm if culling is server side logic
            _triggerCondition = checker;
            _effectGUID = effectGUID;
        }

        public EffectCastFinder(long skillID, string effectGUID, long icd, ulong minBuild, ulong maxBuild, EffectCastChecker checker = null) : base(skillID, icd, minBuild, maxBuild)
        {
            NotAccurate = true; // TODO: confirm if culling is server side logic
            _triggerCondition = checker;
            _effectGUID = effectGUID;
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
                        if (_triggerCondition != null)
                        {
                            if (_triggerCondition(effectEvent, combatData))
                            {
                                lastTime = effectEvent.Time;
                                res.Add(new InstantCastEvent(effectEvent.Time, skillData.Get(SkillID), GetAgent(effectEvent)));
                            }
                        }
                        else
                        {
                            lastTime = effectEvent.Time;
                            res.Add(new InstantCastEvent(effectEvent.Time, skillData.Get(SkillID), GetAgent(effectEvent)));
                        }
                    }
                }
            }
            return res;
        }
    }
}
