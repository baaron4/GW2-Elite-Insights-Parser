using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.EIData;

internal class BuffDictionary(int layer1InitialCapacity, int layer2InitialCapacityBuffs, int layer2InitialCapacityExts, int layer3InitialCapacityExts)
{
    readonly int _layer2InitialCapacityBuffs = layer2InitialCapacityBuffs;
    readonly int _layer2InitialCapacityExts = layer2InitialCapacityExts;
    readonly int _layer3InitialCapacityExts = layer3InitialCapacityExts;
    readonly Dictionary<long, List<BuffEvent>> _buffIDToEvents = new(layer1InitialCapacity);
    // Fast look up table for AddToList
    readonly Dictionary<long, Dictionary<uint, List<(BuffExtensionEvent bee, int index)>>> _buffIDToExtensions = new(layer1InitialCapacity);

    public bool TryGetValue(long buffID, [NotNullWhen(true)] out List<BuffEvent>? list)
    {
        return _buffIDToEvents.TryGetValue(buffID, out list);
    }

    static void AddToList(ParsedEvtcLog log, List<BuffEvent> list, Dictionary<uint, List<(BuffExtensionEvent bee, int index)>> dictExtension, BuffEvent buffEvent, int initialListCapacity)
    {
        bool insert = true;
        // Essence of speed issue for Soulbeast
        if (buffEvent is BuffExtensionEvent beeCurrent)
        {
            if (beeCurrent.ExtendedDuration < 1)
            {
                insert = false;
            }
            else if (beeCurrent.BuffInstance != 0)
            {
                if (dictExtension.TryGetValue(beeCurrent.BuffInstance, out var listExtension))
                {
                    var beeLast = listExtension.LastOrNull();
                    if (beeLast != null)
                    {
                        var bee = beeLast.Value.bee;
                        if (Math.Abs(buffEvent.Time - bee.Time) <= 1)
                        {
                            if (Math.Abs(beeCurrent.OldDuration - bee.OldDuration) <= 1)
                            {
                                insert = false;
                                list[beeLast.Value.index] = beeCurrent;
                                listExtension[^1] = (beeCurrent, beeLast.Value.index);
                            }
                            else if (Math.Abs(beeCurrent.NewDuration - bee.NewDuration) <= 1)
                            {
                                insert = false;
                            }
                        }
                    } 
                    if (insert)
                    {
                        listExtension.Add((beeCurrent, list.Count));
                    }
                }
                else
                {
                    dictExtension[beeCurrent.BuffInstance] = new(initialListCapacity){ (beeCurrent, list.Count) };
                }
            }
        }
        // handle duplicated application for buff initials
        else if (buffEvent is BuffApplyEvent bae && bae.BuffInstance != 0 && bae.Initial)
        {
            var duplicated = log.CombatData.GetBuffDataByInstanceID(bae.BuffID, bae.BuffInstance)
                .Where(x => x is BuffApplyEvent otherBae && x.To.Is(bae.To) && !otherBae.Initial && Math.Abs(otherBae.Time - bae.Time) <= 1)
                .Any();
            if (duplicated)
            {
                insert = false;
            }
        } 
        if (insert)
        {
            list.Add(buffEvent);
        }
    }

    public void Add(ParsedEvtcLog log, Buff buff, BuffEvent buffEvent)
    {
        if (!buffEvent.IsBuffSimulatorCompliant(log.CombatData.UseBuffInstanceSimulator))
        {
            return;
        }
        if (!_buffIDToEvents.TryGetValue(buff.ID, out var list))
        {
            list = new(_layer2InitialCapacityBuffs);
            _buffIDToEvents[buff.ID] = list;
            _buffIDToExtensions[buff.ID] = new(_layer2InitialCapacityExts);
        }

        AddToList(log, list, _buffIDToExtensions[buff.ID], buffEvent, _layer3InitialCapacityExts);
    }

    private BuffRemoveSingleEvent? _lastRemovedRegen = null;
    public void AddRegen(ParsedEvtcLog log, Buff buff, BuffEvent buffEvent)
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
        if (!_buffIDToEvents.TryGetValue(buff.ID, out var list))
        {
            list = new(_layer2InitialCapacityBuffs);
            _buffIDToEvents[buff.ID] = list;
            _buffIDToExtensions[buff.ID] = new(_layer2InitialCapacityExts);
        }
        
        AddToList(log, list, _buffIDToExtensions[buff.ID], buffEvent, _layer3InitialCapacityExts);
    }


    public void Finalize(ParsedEvtcLog log, AgentItem agentItem, out HashSet<Buff> trackedBuffs)
    {
        // add buff remove all for each despawn events
        long lastDespawn = agentItem.FirstAware;
        foreach (DespawnEvent dsp in log.CombatData.GetDespawnEvents(agentItem))
        {
            lastDespawn = dsp.Time;
            foreach (var pair in _buffIDToEvents)
            {
                pair.Value.Add(new BuffRemoveAllEvent(ParserHelper._unknownAgent, agentItem, dsp.Time + ParserHelper.ServerDelayConstant, int.MaxValue, log.SkillData.Get(pair.Key), IFF.Unknown, BuffRemoveAllEvent.FullRemoval, int.MaxValue));
            }
        }

        if (agentItem.LastAware < log.LogData.LogEnd - 2000 && agentItem.LastAware - lastDespawn > 2000)
        {
            foreach (var pair in _buffIDToEvents)
            {
                pair.Value.Add(new BuffRemoveAllEvent(ParserHelper._unknownAgent, agentItem, agentItem.LastAware + ParserHelper.ServerDelayConstant, int.MaxValue, log.SkillData.Get(pair.Key), IFF.Unknown, BuffRemoveAllEvent.FullRemoval, int.MaxValue));
            }
        }

        foreach (SpawnEvent sp in log.CombatData.GetSpawnEvents(agentItem))
        {
            foreach (var pair in _buffIDToEvents)
            {
                pair.Value.Add(new BuffRemoveAllEvent(ParserHelper._unknownAgent, agentItem, sp.Time - ParserHelper.ServerDelayConstant, int.MaxValue, log.SkillData.Get(pair.Key), IFF.Unknown, BuffRemoveAllEvent.FullRemoval, int.MaxValue));
            }
        }

        trackedBuffs = new HashSet<Buff>(_buffIDToEvents.Count);
        foreach (var (buffID, events) in _buffIDToEvents)
        {
            trackedBuffs.Add(log.Buffs.BuffsByIDs[buffID]);
            events.SortByTime();
        }
    }
}
