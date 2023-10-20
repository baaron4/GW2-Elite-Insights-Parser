using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffDictionary
    {
        private readonly Dictionary<long, List<AbstractBuffEvent>> _dict = new Dictionary<long, List<AbstractBuffEvent>>();
        // Constructors
        public BuffDictionary()
        {
        }

        public bool TryGetValue(long buffID, out List<AbstractBuffEvent> list)
        {
            return _dict.TryGetValue(buffID, out list);
        }

        private static void AddToList(List<AbstractBuffEvent> list, AbstractBuffEvent buffEvent)
        {
            AbstractBuffEvent last = list.Last();
            // Essence of speed issue for Soulbeast
            if (last is BuffExtensionEvent beeLast && buffEvent is BuffExtensionEvent beeCurrent 
                && beeCurrent.BuffInstance == beeLast.BuffInstance && buffEvent.Time == last.Time
                && Math.Abs(beeCurrent.OldDuration - beeLast.OldDuration) <= 1)
            {
                list.Remove(last);
            }
            list.Add(buffEvent);
        }

            public void Add(ParsedEvtcLog log, Buff buff, AbstractBuffEvent buffEvent)
        {
            if (!buffEvent.IsBuffSimulatorCompliant(log.CombatData.UseBuffInstanceSimulator))
            {
                return;
            }
            buffEvent.TryFindSrc(log);
            if (_dict.TryGetValue(buff.ID, out List<AbstractBuffEvent> list))
            {
                AddToList(list, buffEvent);
                return;
            }
            _dict[buff.ID] = new List<AbstractBuffEvent>() { buffEvent };
        }

        private BuffRemoveSingleEvent _lastRemovedRegen = null;
        public void AddRegen(ParsedEvtcLog log, Buff buff, AbstractBuffEvent buffEvent)
        {
            if (!buffEvent.IsBuffSimulatorCompliant(log.CombatData.UseBuffInstanceSimulator))
            {
                if (buffEvent is BuffRemoveSingleEvent brse && log.CombatData.HasStackIDs && brse.RemovedDuration > ParserHelper.BuffSimulatorDelayConstant)
                {
                    _lastRemovedRegen = brse;
                }
                return;
            }
            if (_lastRemovedRegen != null && buffEvent is BuffApplyEvent bae)
            {
                if (bae.Time - _lastRemovedRegen.Time < ParserHelper.ServerDelayConstant)
                {
                    bae.OverridenDurationInternal = (uint)_lastRemovedRegen.RemovedDuration;
                    bae.OverridenInstance = _lastRemovedRegen.BuffInstance;
                }
                _lastRemovedRegen = null;
            }
            buffEvent.TryFindSrc(log);
            if (_dict.TryGetValue(buff.ID, out List<AbstractBuffEvent> list))
            {
                AddToList(list, buffEvent);
                return;
            }
            _dict[buff.ID] = new List<AbstractBuffEvent>() { buffEvent };
        }

        /*private static int CompareBuffEventType(AbstractBuffEvent x, AbstractBuffEvent y)
        {
            if (x.Time < y.Time)
            {
                return -1;
            }
            else if (x.Time > y.Time)
            {
                return 1;
            }
            else
            {
                return x.CompareTo(y);
            }
        }*/


        public void Finalize(ParsedEvtcLog log, AgentItem agentItem, out HashSet<Buff> trackedBuffs)
        {
            // add buff remove all for each despawn events
            long lastDespawn = agentItem.FirstAware;
            foreach (DespawnEvent dsp in log.CombatData.GetDespawnEvents(agentItem))
            {
                lastDespawn = dsp.Time;
                foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in _dict)
                {
                    pair.Value.Add(new BuffRemoveAllEvent(ParserHelper._unknownAgent, agentItem, dsp.Time + ParserHelper.ServerDelayConstant, int.MaxValue, log.SkillData.Get(pair.Key), ArcDPSEnums.IFF.Unknown, BuffRemoveAllEvent.FullRemoval, int.MaxValue));
                }
            }
            if (agentItem.LastAware < log.FightData.FightEnd - 2000 && agentItem.LastAware - lastDespawn > 2000)
            {
                foreach(KeyValuePair<long, List<AbstractBuffEvent>> pair in _dict)
                {
                    pair.Value.Add(new BuffRemoveAllEvent(ParserHelper._unknownAgent, agentItem, agentItem.LastAware + ParserHelper.ServerDelayConstant, int.MaxValue, log.SkillData.Get(pair.Key), ArcDPSEnums.IFF.Unknown, BuffRemoveAllEvent.FullRemoval, int.MaxValue));
                }
            }
            foreach (SpawnEvent sp in log.CombatData.GetSpawnEvents(agentItem))
            {
                foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in _dict)
                {
                    pair.Value.Add(new BuffRemoveAllEvent(ParserHelper._unknownAgent, agentItem, sp.Time - ParserHelper.ServerDelayConstant, int.MaxValue, log.SkillData.Get(pair.Key), ArcDPSEnums.IFF.Unknown, BuffRemoveAllEvent.FullRemoval, int.MaxValue));
                }
            }
            trackedBuffs = new HashSet<Buff>();
            foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in _dict)
            {
                trackedBuffs.Add(log.Buffs.BuffsByIds[pair.Key]);
                var auxValue = pair.Value.OrderBy(x => x.Time).ToList();
                pair.Value.Clear();
                pair.Value.AddRange(auxValue);
            }
        }

    }

}
