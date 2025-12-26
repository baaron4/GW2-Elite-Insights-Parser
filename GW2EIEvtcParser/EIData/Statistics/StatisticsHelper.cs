using System.Numerics;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData;

/// <summary>
/// Passes statistical information
/// </summary>
public class StatisticsHelper
{
    internal StatisticsHelper(CombatData combatData, IReadOnlyList<Player> players, BuffsContainer buffs)
    {
        IReadOnlyCollection<long> skillIDs = combatData.GetSkills();
        // Main boons
        foreach (Buff boon in buffs.BuffsByClassification[BuffClassification.Boon])
        {
            if (skillIDs.Contains(boon.ID))
            {
                _presentBoons.Add(boon);
            }
        }
        // Main Conditions
        foreach (Buff condition in buffs.BuffsByClassification[BuffClassification.Condition])
        {
            if (skillIDs.Contains(condition.ID))
            {
                _presentConditions.Add(condition);
            }
        }

        // Important class specific boons
        foreach (Buff offensiveBuff in buffs.BuffsByClassification[BuffClassification.Offensive])
        {
            if (skillIDs.Contains(offensiveBuff.ID))
            {
                _presentOffbuffs.Add(offensiveBuff);
            }
        }

        foreach (Buff supportBuff in buffs.BuffsByClassification[BuffClassification.Support])
        {
            if (skillIDs.Contains(supportBuff.ID))
            {
                _presentSupbuffs.Add(supportBuff);
            }
        }

        foreach (Buff defensiveBuff in buffs.BuffsByClassification[BuffClassification.Defensive])
        {
            if (skillIDs.Contains(defensiveBuff.ID))
            {
                _presentDefbuffs.Add(defensiveBuff);
            }

        }

        foreach (Buff gearBuff in buffs.BuffsByClassification[BuffClassification.Gear])
        {
            if (skillIDs.Contains(gearBuff.ID))
            {
                _presentGearbuffs.Add(gearBuff);
            }

        }

        foreach (Buff debuff in buffs.BuffsByClassification[BuffClassification.Debuff])
        {
            if (skillIDs.Contains(debuff.ID))
            {
                _presentDebuffs.Add(debuff);
            }

        }

        foreach (Buff nourishment in buffs.BuffsByClassification[BuffClassification.Nourishment])
        {
            if (skillIDs.Contains(nourishment.ID))
            {
                _presentNourishments.Add(nourishment);
            }

        }

        foreach (Buff enhancement in buffs.BuffsByClassification[BuffClassification.Enhancement])
        {
            if (skillIDs.Contains(enhancement.ID))
            {
                _presentEnhancements.Add(enhancement);
            }

        }

        foreach (Buff otherConsumable in buffs.BuffsByClassification[BuffClassification.OtherConsumable])
        {
            if (skillIDs.Contains(otherConsumable.ID))
            {
                _presentOtherConsumables.Add(otherConsumable);
            }

        }

        // All class specific boons
        var remainingBuffsByIDs = buffs.BuffsByClassification[BuffClassification.Other].GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.FirstOrDefault()!);
        foreach (Player player in players)
        {
            _presentRemainingBuffsPerPlayer[player] = [];
            foreach (BuffEvent item in combatData.GetBuffDataByDst(player.AgentItem))
            {
                if (item is BuffApplyEvent && item.To.Is(player.AgentItem) && remainingBuffsByIDs.TryGetValue(item.BuffID, out var boon))
                {
                    _presentRemainingBuffsPerPlayer[player].Add(boon);
                }
            }
        }
    }


    // present buff
    public IReadOnlyList<Buff> PresentBoons => _presentBoons;//Used only for Boon tables
    public IReadOnlyList<Buff> PresentConditions => _presentConditions;//Used only for Condition tables
    public IReadOnlyList<Buff> PresentOffbuffs => _presentOffbuffs;//Used only for Off Buff tables
    public IReadOnlyList<Buff> PresentSupbuffs => _presentSupbuffs;//Used only for Off Buff tables
    public IReadOnlyList<Buff> PresentDefbuffs => _presentDefbuffs;//Used only for Def Buff tables
    public IReadOnlyList<Buff> PresentDebuffs => _presentDebuffs;//Used only for Debuff tables
    public IReadOnlyList<Buff> PresentGearbuffs => _presentGearbuffs;//Used only for Gear Buff tables
    public IReadOnlyList<Buff> PresentNourishements => _presentNourishments;
    public IReadOnlyList<Buff> PresentEnhancements => _presentEnhancements;
    public IReadOnlyList<Buff> PresentOtherConsumables => _presentOtherConsumables;

    public IReadOnlyCollection<Buff> GetPresentRemainingBuffsOnPlayer(SingleActor actor)
    {
        if (actor is Player p && _presentRemainingBuffsPerPlayer.TryGetValue(p, out var buffs))
        {
            return buffs;
        }
        return [ ];
    }

    //

    private readonly List<Buff> _presentBoons = [];//Used only for Boon tables
    private readonly List<Buff> _presentConditions = [];//Used only for Condition tables
    private readonly List<Buff> _presentOffbuffs = [];//Used only for Off Buff tables
    private readonly List<Buff> _presentSupbuffs = [];//Used only for Off Buff tables
    private readonly List<Buff> _presentDefbuffs = [];//Used only for Def Buff tables
    private readonly List<Buff> _presentDebuffs = [];//Used only for Debuff tables
    private readonly List<Buff> _presentGearbuffs = [];//Used only for Gear Buff tables
    private readonly List<Buff> _presentNourishments = [];
    private readonly List<Buff> _presentEnhancements = [];
    private readonly List<Buff> _presentOtherConsumables = [];
    private readonly Dictionary<Player, HashSet<Buff>> _presentRemainingBuffsPerPlayer = [];


    //Positions for group
    private List<ParametricPoint3D?>? _stackCenterPositions = null;
    private List<ParametricPoint3D?>? _stackCommanderPositions = null;
    private List<(AgentItem p, GenericSegment<GUID> state)>? _commanderStates = null;

    /// <summary> Returns a list of center positions of the squad which are null in places where all players are dead or disconnected. One entry for each polling. </summary>
    public IReadOnlyList<ParametricPoint3D?> GetStackCenterPositions(ParsedEvtcLog log)
    {
        _stackCenterPositions ??= CalculateStackCenterPositions(log);
        return _stackCenterPositions;
    }

    /// <summary> Returns a list of commander positions for the squad which are null in places where there is no commander. One entry for each polling. </summary>
    public IReadOnlyList<ParametricPoint3D?> GetStackCommanderPositions(ParsedEvtcLog log)
    {
        _stackCommanderPositions ??= CalculateStackCommanderPositions(log);
        return _stackCommanderPositions;
    }

    /// <summary> Returns a list of commander states for the squad. Segment contains timeframe during which agent was commander and the marker GUID 
    /// List is ordered by state.Start. Segments should not overlap.
    /// </summary>
    public IReadOnlyList<(AgentItem p, GenericSegment<GUID> state)> GetCommanderStates(ParsedEvtcLog log)
    {
        _commanderStates ??= CalculateCommanderStates(log);
        return _commanderStates;
    }

    /// <summary> Calculates a list of center positions of the squad which are null in places where all players are dead or disconnected. </summary>
    private static List<ParametricPoint3D?> CalculateStackCenterPositions(ParsedEvtcLog log)
    {
        if (!log.CombatData.HasMovementData)
        {
            return [];
        }

        var positionsPerPlayer = new List<IReadOnlyList<ParametricPoint3D?>>(log.PlayerList.Count);
        foreach (Player player in log.PlayerList)
        {
            positionsPerPlayer.Add(player.GetCombatReplayActivePolledPositions(log));
        }

        var sampleCount = (int)(log.LogData.LogEnd / ParserHelper.CombatReplayPollingRate);

        var centerPositions = new List<ParametricPoint3D?>(sampleCount);
        var centerVector3Positions = new List<Vector3?>(sampleCount);
        var activePlayersPerPositions = new List<int>(sampleCount);
        foreach (var positions in positionsPerPlayer)
        {
            foreach (var position in positions)
            {
                if (position == null)
                {
                    continue;
                }
                int index = (int)(position.Value.Time / ParserHelper.CombatReplayPollingRate);
                while (centerVector3Positions.Count <= index)
                {
                    centerVector3Positions.Add(null);
                    activePlayersPerPositions.Add(0);
                }
                var current = centerVector3Positions[index];
                centerVector3Positions[index] = current != null ? current.Value + position.Value.XYZ : position.Value.XYZ;
                activePlayersPerPositions[index] += 1;
            }
        }
        while (centerVector3Positions.Count < sampleCount)
        {
            centerVector3Positions.Add(null);
            activePlayersPerPositions.Add(0);
        }
        for (int t = 0; t < sampleCount; t++)
        {
            var curVector3 = centerVector3Positions[t];
            if (curVector3 == null)
            {
                centerPositions.Add(null);
            }
            else
            {
                curVector3 /= activePlayersPerPositions[t];
                centerPositions.Add(new ParametricPoint3D(curVector3.Value, ParserHelper.CombatReplayPollingRate * t));
            }
        }

        return centerPositions;
    }

    /// <summary> Calculates a list of commander positions for the squad which are null in places where there is no commander. </summary>
    private static List<ParametricPoint3D?> CalculateStackCommanderPositions(ParsedEvtcLog log)
    {
        if (!log.CombatData.HasMovementData)
        {
            return [ ];
        }

        var commanders = new List<GenericSegment<Player>>(log.PlayerList.Count); //TODO_PERF(Rennorb): find average complexity
        foreach (Player player in log.PlayerList)
        {
            var newStates = player.GetCommanderStates(log);
            commanders.ReserveAdditional(newStates.Count);
            foreach (var state in newStates)
            {
                commanders.Add(state.WithOtherType(player));
            }
        }
        commanders.Sort((a, b) => a.Start.CompareTo(b.Start));

        var commanderPositions = new List<ParametricPoint3D?>((int)(log.LogData.LogDuration / ParserHelper.CombatReplayPollingRate));
        long start = long.MinValue;
        foreach (var commanderSegment in commanders) // don't deconstruct, guids are large
        {
            var polledPositions = commanderSegment.Value!.GetCombatReplayPolledPositions(log);
            foreach(var pos in polledPositions)
            {
                if(pos.Time < start) { continue; }
                if(pos.Time >= commanderSegment.End) { break; }

                if(pos.Time < commanderSegment.Start)
                {
                    //NOTE(Rennorb): This means we are between the end of the last, and teh beginning of the current segment,
                    // which in turn means there is no commander right now.
                    commanderPositions.Add(null);
                }
                else
                {
                    commanderPositions.Add(pos);
                }
            }

            start = commanderSegment.End;
        }

        return commanderPositions;
    }

    private static List<(AgentItem p, GenericSegment<GUID> state)> CalculateCommanderStates(ParsedEvtcLog log)
    {
        var useGUIDs = log.LogMetadata.EvtcBuild >= ArcDPSBuilds.FunctionalIDToGUIDEvents;
        var statesByPlayer = new Dictionary<AgentItem, IReadOnlyList<GenericSegment<GUID>>>(log.PlayerList.Count);
        var relevantPlayers = log.PlayerList.DistinctBy(x => x.EnglobingAgentItem).Select(x => x.EnglobingAgentItem);
        foreach (var player in relevantPlayers)
        {
            IReadOnlyList<MarkerEvent> markerEvents = log.CombatData.GetMarkerEvents(player);
            var commanderMarkerStates = new List<GenericSegment<GUID>>(markerEvents.Count);
            foreach (MarkerEvent markerEvent in markerEvents)
            {
                MarkerGUIDEvent marker = markerEvent.GUIDEvent!;
                if (useGUIDs)
                {
                    if (marker.IsCommanderTag)
                    {
                        commanderMarkerStates.Add(new(markerEvent.Time, Math.Min(markerEvent.EndTime, log.LogData.EvtcLogEnd), marker.ContentGUID));
                        if (markerEvent.EndNotSet)
                        {
                            break;
                        }
                    }
                }
                else if (markerEvent.MarkerID != 0)
                {
                    commanderMarkerStates.Clear();
                    commanderMarkerStates.Add(new(player.FirstAware, log.LogData.EvtcLogEnd, MarkerGUIDs.BlueCommanderTag));
                    break;
                }
            }
            if (commanderMarkerStates.Count > 0)
            {
                statesByPlayer[player] = commanderMarkerStates;
            }
        }
        var states = new List<(AgentItem p, GenericSegment<GUID> seg)>(statesByPlayer.Count * statesByPlayer.Values.FirstOrDefault()?.Count ?? 1);
        foreach (var (player, state) in statesByPlayer)
        {
            foreach (var segment in state)
            {
                states.Add((player, segment));
            }
        }
        states.Sort((a, b) => (int)(a.seg.Start - b.seg.Start));

        List<(AgentItem p, GenericSegment<GUID> state)> CommanderStates = new(states.Count);
        var (lastPlayer, lastSegment) = states[0];
        foreach (var (player, seg) in states.Skip(1))
        {
            if (lastPlayer.Is(player) && lastSegment.Value == seg.Value)
            {
                lastSegment.End = seg.End;
            }
            else
            {
                // Avoid overlaps, only one commander can be active at a given time
                if (seg.Start < lastSegment.End)
                {
                    lastSegment.End = seg.Start;
                }
                CommanderStates.Add((lastPlayer, lastSegment));
                lastPlayer = player;
                lastSegment = seg;
            }
        }
        CommanderStates.Add((lastPlayer, lastSegment));

        return CommanderStates;
    }

}
