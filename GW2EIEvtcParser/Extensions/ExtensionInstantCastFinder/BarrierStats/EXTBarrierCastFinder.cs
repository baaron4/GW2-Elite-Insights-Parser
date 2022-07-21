using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    internal class EXTBarrierCastFinder : InstantCastFinder
    {
        public delegate bool BarrierCastChecker(EXTAbstractBarrierEvent evt, CombatData combatData);
        private BarrierCastChecker _triggerCondition { get; set; }

        private readonly long _damageSkillID;
        public EXTBarrierCastFinder(long skillID, long damageSkillID) : base(skillID)
        {
            NotAccurate = true;
            _damageSkillID = damageSkillID;
        }
        internal EXTBarrierCastFinder UsingChecker(BarrierCastChecker checker)
        {
            _triggerCondition = checker;
            return this;
        }

        public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var res = new List<InstantCastEvent>();
            if (!combatData.HasEXTBarrier)
            {
                return res;
            }
            var heals = combatData.EXTBarrierCombatData.GetBarrierData(_damageSkillID).GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<AgentItem, List<EXTAbstractBarrierEvent>> pair in heals)
            {
                long lastTime = int.MinValue;
                if (!HealingStatsExtensionHandler.SanitizeForSrc(pair.Value))
                {
                    continue;
                }
                foreach (EXTAbstractBarrierEvent de in pair.Value)
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
                            res.Add(new InstantCastEvent(de.Time, skillData.Get(SkillID), de.From));
                        }
                    }
                    else
                    {
                        lastTime = de.Time;
                        res.Add(new InstantCastEvent(de.Time, skillData.Get(SkillID), de.From));
                    }
                }
            }
            return res;
        }
    }
}
