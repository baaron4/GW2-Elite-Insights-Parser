using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffLossCastFinder : BuffCastFinder<BuffRemoveAllEvent>
    {
        public BuffLossCastFinder(long skillID, long buffID) : base(skillID, buffID)
        {
        }

        public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var res = new List<InstantCastEvent>();
            var removals = combatData.GetBuffData(BuffID).OfType<BuffRemoveAllEvent>().GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<AgentItem, List<BuffRemoveAllEvent>> pair in removals)
            {
                long lastTime = int.MinValue;
                foreach (BuffRemoveAllEvent brae in pair.Value)
                {
                    if (brae.Time - lastTime < ICD)
                    {
                        lastTime = brae.Time;
                        continue;
                    }
                    if (CheckCondition(brae, combatData, agentData, skillData))
                    {
                        lastTime = brae.Time;
                        res.Add(new InstantCastEvent(GetTime(brae, brae.To, combatData), skillData.Get(SkillID), brae.To));
                    }
                }
            }
            return res;
        }
    }
}
