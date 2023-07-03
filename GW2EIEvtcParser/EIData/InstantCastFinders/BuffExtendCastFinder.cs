using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffExtendCastFinder : BuffCastFinder<BuffExtensionEvent>
    {
        protected virtual AgentItem GetCasterAgent(AgentItem agent)
        {
            return agent;
        }

        public BuffExtendCastFinder(long skillID, long buffID) : base(skillID, buffID)
        {
        }

        internal BuffExtendCastFinder UsingDurationChecker(int duration, long epsilon = ServerDelayConstant)
        {  
            UsingChecker((evt, combatData, agentData, skillData) => Math.Abs(evt.ExtendedDuration - duration) < epsilon);
            return this;
        }

        public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var res = new List<InstantCastEvent>();
            var applies = combatData.GetBuffData(BuffID).OfType<BuffExtensionEvent>().GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<AgentItem, List<BuffExtensionEvent>> pair in applies)
            {
                long lastTime = int.MinValue;
                foreach (BuffExtensionEvent bae in pair.Value)
                {
                    if (bae.Time - lastTime < ICD)
                    {
                        lastTime = bae.Time;
                        continue;
                    }
                    if (CheckCondition(bae, combatData, agentData, skillData))
                    {
                        lastTime = bae.Time;
                        AgentItem caster = GetCasterAgent(bae.To);
                        res.Add(new InstantCastEvent(GetTime(bae, caster,combatData), skillData.Get(SkillID), caster));
                    }
                }
            }

            return res;
        }
    }
}
