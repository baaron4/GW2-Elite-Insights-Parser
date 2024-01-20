using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class BuffCastFinder<Event> : CheckedCastFinder<Event> where Event: AbstractBuffEvent
    {
        protected bool Minions { get; set; } = false;
        protected long BuffID { get; }
        protected BuffCastFinder(long skillID, long buffID) : base(skillID)
        {
            BuffID = buffID;
        }
        protected AgentItem GetCasterAgent(Event evt)
        {
            return Minions ? GetKeyAgent(evt).GetFinalMaster() : GetKeyAgent(evt);
        }

        protected abstract AgentItem GetKeyAgent(Event evt);

        public virtual BuffCastFinder<Event> WithMinions(bool minions)
        {
            Minions = minions;
            return this;
        }

        public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var res = new List<InstantCastEvent>();
            var applies = combatData.GetBuffData(BuffID).OfType<Event>().GroupBy(x => GetCasterAgent(x)).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<AgentItem, List<Event>> pair in applies)
            {
                long lastTime = int.MinValue;
                foreach (Event evt in pair.Value)
                {
                    if (CheckCondition(evt, combatData, agentData, skillData))
                    {
                        if (evt.Time - lastTime < ICD)
                        {
                            lastTime = evt.Time;
                            continue;
                        }
                        lastTime = evt.Time;
                        res.Add(new InstantCastEvent(GetTime(evt, pair.Key, combatData), skillData.Get(SkillID), pair.Key));
                    }
                }
            }

            return res;
        }
    }
}
