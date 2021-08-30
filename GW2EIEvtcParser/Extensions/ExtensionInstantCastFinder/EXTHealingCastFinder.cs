using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    internal class EXTHealingCastFinder : InstantCastFinder
    {
        public delegate bool HealingCastChecker(EXTAbstractHealingEvent evt, CombatData combatData);
        private readonly HealingCastChecker _triggerCondition;

        private readonly long _damageSkillID;
        public EXTHealingCastFinder(long skillID, long damageSkillID, long icd, HealingCastChecker checker = null) : base(skillID, icd)
        {
            NotAccurate = true;
            _triggerCondition = checker;
            _damageSkillID = damageSkillID;
        }

        public EXTHealingCastFinder(long skillID, long damageSkillID, long icd, ulong minBuild, ulong maxBuild, HealingCastChecker checker = null) : base(skillID, icd, minBuild, maxBuild)
        {
            NotAccurate = true;
            _triggerCondition = checker;
            _damageSkillID = damageSkillID;
        }

        public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var res = new List<InstantCastEvent>();
            if (!combatData.HasEXTHealing)
            {
                return res;
            }
            var heals = combatData.EXTHealingCombatData.GetHealData(_damageSkillID).GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<AgentItem, List<EXTAbstractHealingEvent>> pair in heals)
            {
                long lastTime = int.MinValue;
                if (!HealingStatsExtensionHandler.SanitizeForSrc(pair.Value))
                {
                    continue;
                }
                foreach (EXTAbstractHealingEvent de in pair.Value)
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
