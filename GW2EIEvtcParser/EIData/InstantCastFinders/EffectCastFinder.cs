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
                var effects = combatData.GetEffectEvents(effectGUIDEvent.ContentID).GroupBy(x => x.Src).ToDictionary(x => x.Key, x => x.ToList());
                foreach (KeyValuePair<AgentItem, List<EffectEvent>> pair in effects)
                {
                    long lastTime = int.MinValue;
                    foreach (EffectEvent de in pair.Value)
                    {
                        if (de.Time - lastTime < ICD)
                        {
                            lastTime = de.Time;
                            continue;
                        }
                        if (_triggerCondition != null)
                        {
                            if (_triggerCondition(de, combatData))
                            {
                                lastTime = de.Time;
                                res.Add(new InstantCastEvent(de.Time, skillData.Get(SkillID), de.Src));
                            }
                        }
                        else
                        {
                            lastTime = de.Time;
                            res.Add(new InstantCastEvent(de.Time, skillData.Get(SkillID), de.Src));
                        }
                    }
                }
            }
            return res;
        }
    }
}
