using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffGainCastFinder : BuffCastFinder
    {

        public delegate bool BuffGainCastChecker(BuffApplyEvent evt, CombatData combatData);
        protected readonly BuffGainCastChecker TriggerCondition;

        protected virtual AgentItem GetCasterAgent(AgentItem agent)
        {
            return agent;
        }

        public BuffGainCastFinder(long skillID, long buffID, long icd, BuffGainCastChecker checker = null) : base(skillID, buffID, icd)
        {
            TriggerCondition = checker;
        }

        public BuffGainCastFinder(long skillID, long buffID, long icd, ulong minBuild, ulong maxBuild, BuffGainCastChecker checker = null) : base(skillID, buffID, icd, minBuild, maxBuild)
        {
            TriggerCondition = checker;
        }

        public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var res = new List<InstantCastEvent>();
            var applies = combatData.GetBuffData(BuffID).OfType<BuffApplyEvent>().GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
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
                    if (TriggerCondition != null)
                    {
                        if (TriggerCondition(bae, combatData))
                        {
                            lastTime = bae.Time;
                            res.Add(new InstantCastEvent(bae.Time, skillData.Get(SkillID), GetCasterAgent(bae.To)));
                        }
                    }
                    else
                    {
                        lastTime = bae.Time;
                        res.Add(new InstantCastEvent(bae.Time, skillData.Get(SkillID), GetCasterAgent(bae.To)));
                    }
                }
            }

            return res;
        }
    }
}
