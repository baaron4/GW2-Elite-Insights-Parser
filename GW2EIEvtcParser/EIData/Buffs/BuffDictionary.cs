using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffDictionary
    {
        //TODO(Rennorb) @perf @mem: find average complexity
        private readonly Dictionary<long, List<AbstractBuffEvent>> _dict = new(1024);
        // Fast look up table for AddToList
         //TODO(Rennorb) @perf
        private readonly Dictionary<long, Dictionary<uint, List<BuffExtensionEvent>>> _buffIdToExtensions = new();

        public BuffDictionary()
        {
        }

        public bool TryGetValue(long buffID, [NotNullWhen(true)] out List<AbstractBuffEvent>? list)
        {
            return _dict.TryGetValue(buffID, out list);
        }

        private static void AddToList(ParsedEvtcLog log, List<AbstractBuffEvent> list, Dictionary<uint, List<BuffExtensionEvent>> dictExtension, AbstractBuffEvent buffEvent)
        {
            // Essence of speed issue for Soulbeast
            if (buffEvent is BuffExtensionEvent beeCurrent)
            {
                if (beeCurrent.BuffInstance != 0)
                {
                    if (dictExtension.TryGetValue(beeCurrent.BuffInstance, out var listExtension))
                    {
                        var beeLast = listExtension.LastOrDefault();
                        if (beeLast != null && Math.Abs(buffEvent.Time - beeLast.Time) <= 1)
                        {
                            if (Math.Abs(beeCurrent.OldDuration - beeLast.OldDuration) <= 1)
                            {
                                list.Remove(beeLast);
                                listExtension.Remove(beeLast);
                            }
                            else if (Math.Abs(beeCurrent.NewDuration - beeLast.NewDuration) <= 1)
                            {
                                return;
                            }
                        }
                        listExtension.Add(beeCurrent);
                    }
                    else
                    {
                        dictExtension[beeCurrent.BuffInstance] = [ beeCurrent ];
                    }
                }
            }
            // handle duplicated application for buff initials
            else if (buffEvent is BuffApplyEvent bae && bae.BuffInstance != 0 && bae.Initial)
            {
                var duplicated = log.CombatData.GetBuffDataByInstanceID(bae.BuffID, bae.BuffInstance)
                    .Where(x => x is BuffApplyEvent otherBae && x.To == bae.To && !otherBae.Initial && Math.Abs(otherBae.Time - bae.Time) <= 1)
                    .Any();
                if (duplicated)
                {
                    return;
                }
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
            if (!_dict.TryGetValue(buff.ID, out var list))
            {
                //TODO(Rennorb) @perf
                list = new List<AbstractBuffEvent>();
                _dict[buff.ID] = list;
                _buffIdToExtensions[buff.ID] = new Dictionary<uint, List<BuffExtensionEvent>>();
            }
            AddToList(log, list, _buffIdToExtensions[buff.ID], buffEvent);
        }

        private BuffRemoveSingleEvent? _lastRemovedRegen = null;
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
            if (!_dict.TryGetValue(buff.ID, out var list))
            {
                //TODO(Rennorb) @perf
                list = new List<AbstractBuffEvent>();
                _dict[buff.ID] = list;
                _buffIdToExtensions[buff.ID] = new Dictionary<uint, List<BuffExtensionEvent>>();
            }
            AddToList(log, list, _buffIdToExtensions[buff.ID], buffEvent);
        }


        public void Finalize(ParsedEvtcLog log, AgentItem agentItem, out HashSet<Buff> trackedBuffs)
        {
            // add buff remove all for each despawn events
            long lastDespawn = agentItem.FirstAware;
            foreach (DespawnEvent dsp in log.CombatData.GetDespawnEvents(agentItem))
            {
                lastDespawn = dsp.Time;
                foreach (var pair in _dict)
                {
                    pair.Value.Add(new BuffRemoveAllEvent(ParserHelper._unknownAgent, agentItem, dsp.Time + ParserHelper.ServerDelayConstant, int.MaxValue, log.SkillData.Get(pair.Key), ArcDPSEnums.IFF.Unknown, BuffRemoveAllEvent.FullRemoval, int.MaxValue));
                }
            }
            if (agentItem.LastAware < log.FightData.FightEnd - 2000 && agentItem.LastAware - lastDespawn > 2000)
            {
                foreach (var pair in _dict)
                {
                    pair.Value.Add(new BuffRemoveAllEvent(ParserHelper._unknownAgent, agentItem, agentItem.LastAware + ParserHelper.ServerDelayConstant, int.MaxValue, log.SkillData.Get(pair.Key), ArcDPSEnums.IFF.Unknown, BuffRemoveAllEvent.FullRemoval, int.MaxValue));
                }
            }
            foreach (SpawnEvent sp in log.CombatData.GetSpawnEvents(agentItem))
            {
                foreach (var pair in _dict)
                {
                    pair.Value.Add(new BuffRemoveAllEvent(ParserHelper._unknownAgent, agentItem, sp.Time - ParserHelper.ServerDelayConstant, int.MaxValue, log.SkillData.Get(pair.Key), ArcDPSEnums.IFF.Unknown, BuffRemoveAllEvent.FullRemoval, int.MaxValue));
                }
            }
            trackedBuffs = new HashSet<Buff>(_dict.Count);
            foreach (var pair in _dict)
            {
                trackedBuffs.Add(log.Buffs.BuffsByIds[pair.Key]);
                pair.Value.SortByTime();
            }
        }
    }
}
