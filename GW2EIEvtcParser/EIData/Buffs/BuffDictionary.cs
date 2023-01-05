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
            if (_dict.TryGetValue(buffID, out list))
            {
                return true;
            }
            return false;
        }

        private uint _lastRemovedDuration = 0;
        private long _lastRemovedTime = int.MinValue;
        private uint _lastRemovedStackID = 0;
        public void AddRegen(ParsedEvtcLog log, Buff buff, AbstractBuffEvent buffEvent)
        {
            if (!buffEvent.IsBuffSimulatorCompliant(log.CombatData.HasStackIDs))
            {
                if (buffEvent is BuffRemoveSingleEvent brse)
                {
                    _lastRemovedDuration = (uint)brse.RemovedDuration;
                    _lastRemovedStackID = brse.BuffInstance;
                    _lastRemovedTime = brse.Time;
                }
                return;
            }
            if (buffEvent is BuffApplyEvent bae)
            {
                if (bae.Time - _lastRemovedTime < ParserHelper.ServerDelayConstant)
                {
                    bae.OverridenDurationInternal = _lastRemovedDuration;
                    bae.OverridenInstance = _lastRemovedStackID;
                }
                _lastRemovedDuration = 0;
                _lastRemovedTime = int.MinValue;
                _lastRemovedStackID = 0;
            }
            buffEvent.TryFindSrc(log);
            if (_dict.TryGetValue(buff.ID, out List<AbstractBuffEvent> list))
            {
                list.Add(buffEvent);
                return;
            }
            _dict[buff.ID] = new List<AbstractBuffEvent>() { buffEvent };
        }

        private BuffApplyEvent _lastAdded = null;
        public void Add(ParsedEvtcLog log, Buff buff, AbstractBuffEvent buffEvent) 
        {
            if (!buffEvent.IsBuffSimulatorCompliant(log.CombatData.HasStackIDs))
            {
                if (_lastAdded != null && buffEvent is BuffRemoveSingleEvent brse && brse.Time - _lastAdded.Time < ParserHelper.ServerDelayConstant)
                {
                    _lastAdded.OverridenInstance = brse.BuffInstance;
                    _lastAdded = null;
                }
                return;
            }
            buffEvent.TryFindSrc(log);
            if (buffEvent is BuffApplyEvent bae)
            {
                _lastAdded = bae;
            }
            if (_dict.TryGetValue(buff.ID, out List<AbstractBuffEvent> list))
            {
                list.Add(buffEvent);
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
            foreach (DespawnEvent dsp in log.CombatData.GetDespawnEvents(agentItem))
            {
                foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in _dict)
                {
                    pair.Value.Add(new BuffRemoveAllEvent(ParserHelper._unknownAgent, agentItem, dsp.Time + ParserHelper.ServerDelayConstant, int.MaxValue, log.SkillData.Get(pair.Key), BuffRemoveAllEvent.FullRemoval, int.MaxValue));
                }
            }
            foreach (SpawnEvent sp in log.CombatData.GetSpawnEvents(agentItem))
            {
                foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in _dict)
                {
                    pair.Value.Add(new BuffRemoveAllEvent(ParserHelper._unknownAgent, agentItem, sp.Time - ParserHelper.ServerDelayConstant, int.MaxValue, log.SkillData.Get(pair.Key), BuffRemoveAllEvent.FullRemoval, int.MaxValue));
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
