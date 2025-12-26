using System.Globalization;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

public class Player : PlayerActor
{

    private List<GenericSegment<GUID>>? CommanderStates = null;
    private bool _squadless = false;
    // Constructors
    internal Player(AgentItem agent, bool noSquad) : base(agent)
    {
        if (agent.Type != AgentItem.AgentType.Player)
        {
            throw new InvalidDataException("Agent is not a Player");
        }
        string[] name = agent.Name.Split('\0');
        if (name.Length < 2)
        {
            throw new EvtcAgentException("Name problem on Player");
        }
        if (name[1].Length == 0 || name[2].Length == 0 || Character.Contains('-'))
        {
            throw new EvtcAgentException("Missing Group on Player");
        }
        Account = name[1].TrimStart(':');
        _squadless = noSquad;
        Group = noSquad ? 1 : int.Parse(name[2], NumberStyles.Integer, CultureInfo.InvariantCulture);
#if DEBUG
        if (AgentItem.IsEnglobedAgent)
        {
            Character = $"${Character} {EnglobingAgentItem.EnglobedAgentItems.IndexOf(AgentItem) + 1}";
        }
#endif
    }


    internal void MakeSquadless()
    {
        _squadless = true;
        Group = 1;
    }

    internal void OverrideGroup(int group)
    {
        if (_squadless)
        {
            return;
        }
        Group = group;
    }

    internal override (Dictionary<long, BuffStatistics> Buffs, Dictionary<long, BuffStatistics> ActiveBuffs) ComputeBuffs(ParsedEvtcLog log, long start, long end, BuffEnum type)
    {
        return (type) switch 
        {
            BuffEnum.Group =>
                BuffStatistics.GetBuffsForPlayers(log.PlayerList.Where(p => p.Group == Group && this != p), log, this, start, end),
            BuffEnum.OffGroup => 
                BuffStatistics.GetBuffsForPlayers(log.PlayerList.Where(p => p.Group != Group), log, this, start, end),
            BuffEnum.Squad =>
                BuffStatistics.GetBuffsForPlayers(log.PlayerList.Where(p => p != this), log, this, start, end),
            _ =>  BuffStatistics.GetBuffsForSelf(log, this, start, end),
        };
    }

    internal override (Dictionary<long, BuffVolumeStatistics> Volumes, Dictionary<long, BuffVolumeStatistics> ActiveVolumes) ComputeBuffVolumes(ParsedEvtcLog log, long start, long end, BuffEnum type)
    {
        return (type) switch
        {
            BuffEnum.Group =>
                BuffVolumeStatistics.GetBuffVolumesForPlayers(log.PlayerList.Where(p => p.Group == Group && this != p), log, this, start, end),
            BuffEnum.OffGroup =>
                BuffVolumeStatistics.GetBuffVolumesForPlayers(log.PlayerList.Where(p => p.Group != Group), log, this, start, end),
            BuffEnum.Squad =>
                BuffVolumeStatistics.GetBuffVolumesForPlayers(log.PlayerList.Where(p => p != this), log, this, start, end),
            _ => BuffVolumeStatistics.GetBuffVolumesForSelf(log, this, start, end),
        };
    }

    /// <summary>
    /// Checks if the player has a Commander Tag GUID.
    /// </summary>
    /// <returns><see langword="true"/> if a GUID was found, otherwise <see langword="false"/></returns>
    public bool IsCommander(ParsedEvtcLog log)
    {
        return GetCommanderStates(log).Count > 0;
    }

    /// <summary>
    /// Return commander status list, the value of the segment is currently active commander tag ID.
    /// Player had said tag between every segment.Start and segment.End.
    /// 
    /// The value of the segment is the GUID of the specific commander tag.
    /// </summary>
    public IReadOnlyList<GenericSegment<GUID>> GetCommanderStates(ParsedEvtcLog log)
    {
        if (CommanderStates == null)
        {
            CommanderStates = log.StatisticsHelper.GetCommanderStates(log).Where(x => x.p.Is(AgentItem)).Select(x => x.state).ToList();
            // Clamp to aware times
            for (var i = 0; i < CommanderStates.Count; i++)
            {
                var seg = CommanderStates[i];
                CommanderStates[i] = new(Math.Max(seg.Start, FirstAware), Math.Min(seg.End, LastAware), seg.Value);
            }
            CommanderStates.RemoveAll(x => x.IsEmpty());
        }
        return CommanderStates;
    }

    /// <summary>
    /// Return commander status list, with no consideration of tag type.
    /// Player had a commander tag between every segment.Start and segment.End.
    /// </summary>
    public IReadOnlyList<Segment> GetCommanderStatesNoTagValues(ParsedEvtcLog log)
    {
        var commanderStates = GetCommanderStates(log);
        if(commanderStates.Count == 0) { return [ ]; }

        var result = new List<Segment>();
        Segment last = commanderStates[0].WithOtherType<double>();
        foreach (var state in commanderStates.Skip(1))
        {
            if (state.Start != last.End)
            {
                result.Add(last);
                last = state.WithOtherType<double>();
            }
            else
            {
                last.End = state.End;
            }
        }
        result.Add(last);

        return result;
    }

    protected override void InitAdditionalCombatReplayData(ParsedEvtcLog log, CombatReplay replay)
    {
        foreach (var seg in GetCommanderStates(log))
        {
            if (ParserIcons.CommanderTagToIcon.TryGetValue(seg.Value, out var icon))
            {
                replay.Decorations.AddRotatedOverheadMarkerIcon(new Segment(seg.Start, seg.End, 1), this, icon, 180f, 15);
            }
        }
        base.InitAdditionalCombatReplayData(log, replay);
    }

}
