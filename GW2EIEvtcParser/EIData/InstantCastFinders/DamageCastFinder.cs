using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class DamageCastFinder : InstantCastFinder
    {
        public delegate bool DamageCastChecker(AbstractHealthDamageEvent evt, CombatData combatData);
        private DamageCastChecker _triggerCondition { get; set; }

        private readonly long _damageSkillID;
        public DamageCastFinder(long skillID, long damageSkillID ) : base(skillID)
        {
            NotAccurate = true;
            _damageSkillID = damageSkillID;
        }

        internal DamageCastFinder UsingChecker(DamageCastChecker checker)
        {
            _triggerCondition = checker;
            return this;
        }

        public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var res = new List<InstantCastEvent>();
            var damages = combatData.GetDamageData(_damageSkillID).GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<AgentItem, List<AbstractHealthDamageEvent>> pair in damages)
            {
                long lastTime = int.MinValue;
                foreach (AbstractHealthDamageEvent de in pair.Value)
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
