using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class DamageCastFinder : CheckedCastFinder<AbstractHealthDamageEvent>
    {
        private readonly long _damageSkillID;
        public DamageCastFinder(long skillID, long damageSkillID ) : base(skillID)
        {
            UsingNotAccurate(true);
            _damageSkillID = damageSkillID;
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
                    if (CheckCondition(de, combatData, agentData, skillData))
                    {
                        lastTime = de.Time;
                        res.Add(new InstantCastEvent(GetTime(de, de.From, combatData), skillData.Get(SkillID), de.From));
                    }
                }
            }
            return res;
        }
    }
}
