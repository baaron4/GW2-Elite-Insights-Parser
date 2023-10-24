using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class FinalSupport
    {
        private Dictionary<long, (int count, long time)> _friendlyRemovals { get; } = new Dictionary<long, (int count, long time)>();
        public IReadOnlyDictionary<long, (int count, long time)> FriendlyRemovals => _friendlyRemovals;
        private Dictionary<long, (int count, long time)> _foeRemovals { get; } = new Dictionary<long, (int count, long time)>();
        public IReadOnlyDictionary<long, (int count, long time)> FoeRemovals => _foeRemovals;
        private Dictionary<long, (int count, long time)> _unknownRemovals { get; } = new Dictionary<long, (int count, long time)>();
        public IReadOnlyDictionary<long, (int count, long time)> UnknownRemovals => _unknownRemovals;

        internal FinalSupport(ParsedEvtcLog log, long start, long end, AbstractSingleActor actor, AbstractSingleActor to)
        {
            foreach (long buffID in log.Buffs.BuffsByIds.Keys)
            {
                int foeCount = 0;
                long foeTime = 0;
                int friendlyCount = 0;
                long friendlyTime = 0;
                int unknownCount = 0;
                long unknownTime = 0;
                foreach (BuffRemoveAllEvent brae in log.CombatData.GetBuffRemoveAllData(buffID))
                {
                    if (brae.Time >= start && brae.Time <= end && brae.CreditedBy == actor.AgentItem)
                    {
                        if (to != null && brae.To != to.AgentItem)
                        {
                            continue;
                        }
                        if (brae.ToFriendly)
                        {
                            friendlyCount++;
                            friendlyTime = Math.Max(friendlyTime + brae.RemovedDuration, log.FightData.FightDuration);
                        } 
                        else if (brae.ToFoe)
                        {
                            foeCount++;
                            foeTime = Math.Max(foeTime + brae.RemovedDuration, log.FightData.FightDuration);
                        } 
                        else
                        {
                            unknownCount++;
                            unknownTime = Math.Max(unknownTime + brae.RemovedDuration, log.FightData.FightDuration);
                        }
                    }
                }
                if (foeCount > 0)
                {
                    _foeRemovals[buffID] = (foeCount, foeTime);
                }
                if (friendlyCount > 0)
                {
                    _friendlyRemovals[buffID] = (friendlyCount, friendlyTime);
                }
                if (unknownCount > 0)
                {
                    _unknownRemovals[buffID] = (unknownCount, unknownTime);
                }
            }
        }

    }
}
