using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffGiveCastFinder : BuffCastFinder<BuffApplyEvent>
    {
        public BuffGiveCastFinder(long skillID, long buffID) : base(skillID, buffID)
        {
        }

        public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var res = new List<InstantCastEvent>();
            var applies = combatData.GetBuffData(BuffID).OfType<BuffApplyEvent>().GroupBy(x => x.CreditedBy).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<AgentItem, List<BuffApplyEvent>> pair in applies)
            {
                long lastTime = int.MinValue;
                foreach (BuffApplyEvent bae in pair.Value)
                {
                    if (bae.Initial)
                    {
                        continue;
                    }
                    if (bae.Time - lastTime < ICD)
                    {
                        lastTime = bae.Time;
                        continue;
                    }
                    if (CheckCondition(bae, combatData, agentData, skillData))
                    {
                        lastTime = bae.Time;
                        res.Add(new InstantCastEvent(GetTime(bae, bae.CreditedBy, combatData), skillData.Get(SkillID), bae.CreditedBy));
                    }
                }
            }

            return res;
        }
    }
}
