using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    internal class EXTHealingCastFinder : InstantCastFinder
    {
        public delegate bool HealingCastChecker(EXTAbstractHealingEvent evt, CombatData combatData, AgentData agentData, SkillData skillData);
        private HealingCastChecker _triggerCondition { get; set; }

        private readonly long _damageSkillID;
        public EXTHealingCastFinder(long skillID, long damageSkillID) : base(skillID)
        {
            UsingNotAccurate(true);
            UsingEnableInternal((combatData) => combatData.HasEXTHealing);
            _damageSkillID = damageSkillID;
        }
        internal EXTHealingCastFinder UsingChecker(HealingCastChecker checker)
        {
            _triggerCondition = checker;
            return this;
        }

        public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var res = new List<InstantCastEvent>();
            var heals = combatData.EXTHealingCombatData.GetHealData(_damageSkillID).GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<AgentItem, List<EXTAbstractHealingEvent>> pair in heals)
            {
                long lastTime = int.MinValue;
                if (!HealingStatsExtensionHandler.SanitizeForSrc(pair.Value))
                {
                    continue;
                }
                foreach (EXTAbstractHealingEvent he in pair.Value)
                {
                    if (he.Time - lastTime < ICD)
                    {
                        lastTime = he.Time;
                        continue;
                    }
                    if (_triggerCondition == null || _triggerCondition(he, combatData, agentData, skillData))
                    {
                        lastTime = he.Time;
                        res.Add(new InstantCastEvent(GetTime(he, he.From,combatData), skillData.Get(SkillID), he.From));
                    }
                }
            }
            return res;
        }
    }
}
