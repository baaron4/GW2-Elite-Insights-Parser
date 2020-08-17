using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    internal class DamageCastFinder : InstantCastFinder
    {
        public delegate bool DamageCastChecker(AbstractDamageEvent evt, CombatData combatData);
        private readonly DamageCastChecker _triggerCondition;

        private readonly long _damageSkillID;
        public DamageCastFinder(long skillID, long damageSkillID, long icd, DamageCastChecker checker = null) : base(skillID, icd)
        {
            NotAccurate = true;
            _triggerCondition = checker;
            _damageSkillID = damageSkillID;
        }

        public DamageCastFinder(long skillID, long damageSkillID, long icd, ulong minBuild, ulong maxBuild, DamageCastChecker checker = null) : base(skillID, icd, minBuild, maxBuild)
        {
            NotAccurate = true;
            _triggerCondition = checker;
            _damageSkillID = damageSkillID;
        }

        public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var res = new List<InstantCastEvent>();
            var damages = combatData.GetDamageData(_damageSkillID).GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<AgentItem, List<AbstractDamageEvent>> pair in damages)
            {
                long lastTime = int.MinValue;
                foreach (AbstractDamageEvent de in pair.Value)
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
