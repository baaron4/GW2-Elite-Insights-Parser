using System;
using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public class FinalSupport
    {
        public Dictionary<long, (long count, double time)> Removals { get; } = new Dictionary<long, (long count, double time)>();


        public FinalSupport(ParsedLog log, PhaseData phase, AbstractSingleActor actor, AbstractSingleActor to)
        {
            foreach (long buffID in log.Buffs.BuffsByIds.Keys)
            {
                if (!Removals.TryGetValue(buffID, out (long count, double time) item))
                {
                    item = (0, 0);
                    Removals[buffID] = item;
                }
                foreach (BuffRemoveAllEvent brae in log.CombatData.GetBuffRemoveAllData(buffID))
                {
                    if (phase.InInterval(brae.Time) && brae.By == actor.AgentItem && brae.To == to.AgentItem)
                    {
                        item.count++;
                        item.time += Math.Min(brae.RemovedDuration, log.FightData.FightEnd);
                    }
                }
            }
        }

        protected FinalSupport(ParsedLog log, PhaseData phase, AbstractSingleActor actor)
        {
            foreach (long buffID in log.Buffs.BuffsByIds.Keys)
            {
                if (!Removals.TryGetValue(buffID, out (long count, double time) item))
                {
                    item = (0, 0);
                    Removals[buffID] = item;
                }
                foreach (BuffRemoveAllEvent brae in log.CombatData.GetBuffRemoveAllData(buffID))
                {
                    if (phase.InInterval(brae.Time) && brae.By == actor.AgentItem)
                    {
                        item.count++;
                        item.time = Math.Max(item.time + brae.RemovedDuration, log.FightData.FightEnd);
                    }
                }
            }
        }

    }
}
