using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    public class Player : AbstractPlayer
    {

        private List<GenericSegment<string>> CommanderStates { get; set; } = null;
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
            if (name[1].Length == 0 || name[2].Length == 0 || Character.Contains("-"))
            {
                throw new EvtcAgentException("Missing Group on Player");
            }
            Account = name[1].TrimStart(':');
            Group = noSquad ? 1 : int.Parse(name[2], NumberStyles.Integer, CultureInfo.InvariantCulture);
        }


        internal void MakeSquadless()
        {
            Group = 1;
        }

        internal void OverrideGroup(int group)
        {
            Group = group;
        }

        internal void Anonymize(int index)
        {
            Character = "Player " + index;
            Account = "Account " + index;
            AgentItem.OverrideName(Character + "\0:" + Account + "\0" + Group);
        }

        internal override Dictionary<long, FinalActorBuffs>[] ComputeBuffs(ParsedEvtcLog log, long start, long end, BuffEnum type)
        {
            switch (type)
            {
                case BuffEnum.Group:
                    var otherPlayersInGroup = log.PlayerList
                        .Where(p => p.Group == Group && this != p)
                        .ToList();
                    return FinalActorBuffs.GetBuffsForPlayers(otherPlayersInGroup, log, AgentItem, start, end);
                case BuffEnum.OffGroup:
                    var offGroupPlayers = log.PlayerList.Where(p => p.Group != Group).ToList();
                    return FinalActorBuffs.GetBuffsForPlayers(offGroupPlayers, log, AgentItem, start, end);
                case BuffEnum.Squad:
                    var otherPlayers = log.PlayerList.Where(p => p != this).ToList();
                    return FinalActorBuffs.GetBuffsForPlayers(otherPlayers, log, AgentItem, start, end);
                case BuffEnum.Self:
                default:
                    return FinalActorBuffs.GetBuffsForSelf(log, this, start, end);
            }
        }

        internal override Dictionary<long, FinalActorBuffVolumes>[] ComputeBuffVolumes(ParsedEvtcLog log, long start, long end, BuffEnum type)
        {
            switch (type)
            {
                case BuffEnum.Group:
                    var otherPlayersInGroup = log.PlayerList
                        .Where(p => p.Group == Group && this != p)
                        .ToList();
                    return FinalActorBuffVolumes.GetBuffVolumesForPlayers(otherPlayersInGroup, log, AgentItem, start, end);
                case BuffEnum.OffGroup:
                    var offGroupPlayers = log.PlayerList.Where(p => p.Group != Group).ToList();
                    return FinalActorBuffVolumes.GetBuffVolumesForPlayers(offGroupPlayers, log, AgentItem, start, end);
                case BuffEnum.Squad:
                    var otherPlayers = log.PlayerList.Where(p => p != this).ToList();
                    return FinalActorBuffVolumes.GetBuffVolumesForPlayers(otherPlayers, log, AgentItem, start, end);
                case BuffEnum.Self:
                default:
                    return FinalActorBuffVolumes.GetBuffVolumesForSelf(log, this, start, end);
            }
        }

        /// <summary>
        /// Checks if the player has a Commander Tag GUID.
        /// </summary>
        /// <param name="log"></param>
        /// <returns><see langword="true"/> if a GUID was found, otherwise <see langword="false"/></returns>
        public bool IsCommander(ParsedEvtcLog log)
        {
            return GetCommanderStates(log).Count > 0;
        }

        /// <summary>
        /// Return commander status list, the value of the segment is currently active commander tag ID.
        /// Player had said tag between every segment.Start and segment.End.
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public IReadOnlyList<GenericSegment<string>> GetCommanderStates(ParsedEvtcLog log)
        {
            if (CommanderStates == null)
            {
                var statesByPlayer = new Dictionary<Player, IReadOnlyList<GenericSegment<string>>>(log.PlayerList.Count);
                foreach (Player player in log.PlayerList)
                {
                    IReadOnlyList<MarkerEvent> markerEvents = log.CombatData.GetMarkerEvents(player.AgentItem);
                    //TODO(Rennorb) @perf: find average complexity
                    var commanderMarkerStates = new List<GenericSegment<string>>(markerEvents.Count);
                    foreach (MarkerEvent markerEvent in markerEvents)
                    {
                        MarkerGUIDEvent marker = markerEvent.GUIDEvent;
                        if (marker.ContentID >= 0)
                        {
                            if (marker.IsCommanderTag)
                            {
                                commanderMarkerStates.Add(new(markerEvent.Time, Math.Min(markerEvent.EndTime, log.FightData.LogEnd), marker.HexContentGUID));
                                if (markerEvent.EndNotSet)
                                {
                                    break;
                                }
                            }
                        }
                        else if (markerEvent.MarkerID != 0)
                        {
                            commanderMarkerStates.Clear();
                            commanderMarkerStates.Add(new(player.FirstAware, log.FightData.LogEnd, MarkerGUIDs.BlueCommanderTag));
                            break;
                        }
                    }
                    if (commanderMarkerStates.Count > 0)
                    {
                        statesByPlayer[player] = commanderMarkerStates;
                    }
                }

                if (!statesByPlayer.ContainsKey(this))
                {
                    CommanderStates = new();
                    return CommanderStates;
                }

                //TODO(Rennorb) @perf: find average complexity
                var states = new List<(Player p, GenericSegment<string> seg)>(statesByPlayer.Count * statesByPlayer.Values.FirstOrDefault()?.Count ?? 1);
                foreach (var (player, state) in statesByPlayer)
                {
                    foreach (var segment in state)
                    {
                        states.Add((player, segment));
                    }
                }
                states.Sort((a, b) => (int)(a.seg.Start - b.seg.Start));

                CommanderStates = new List<GenericSegment<string>>(states.Count);

                GenericSegment<string>? lastAdded = null;
                Player? lastPlayer = null;
                foreach ((Player p, GenericSegment<string> seg) in states)
                {
                    if (lastPlayer == p && lastAdded.Value == seg.Value)
                    {
                        lastAdded.End = seg.End;
                    }
                    else
                    {
                        lastAdded = seg;
                        lastPlayer = p;
                        if(p == this) { CommanderStates.Add(seg); }
                    }
                }

                //TODO(Rennorb) @perf @mem: find average complexity, maybe this is not required
                if(CommanderStates.Count < (int)(CommanderStates.Capacity * 0.25f)) { CommanderStates.TrimExcess(); }
            }
            return CommanderStates;
        }

        /// <summary>
        /// Return commander status list, with no consideration of tag type.
        /// Player had a commander tag between every segment.Start and segment.End.
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public IReadOnlyList<Segment> GetCommanderStatesNoTagValues(ParsedEvtcLog log)
        {
            IReadOnlyList<GenericSegment<string>> commanderStates = GetCommanderStates(log);
            var result = new List<Segment>();
            Segment prev = null;
            foreach (GenericSegment<string> state in commanderStates)
            {
                if (prev == null || state.Start != prev.End)
                {
                    prev = new Segment(state.Start, state.End);
                    result.Add(prev);
                }
                else
                {
                    prev.End = state.End;
                }
            }
            return result;
        }

        protected override void InitAdditionalCombatReplayData(ParsedEvtcLog log)
        {
            foreach (GenericSegment<string> seg in GetCommanderStates(log))
            {
                if (ParserIcons.CommanderTagToIcon.TryGetValue(seg.Value, out string icon))
                {
                    CombatReplay.AddRotatedOverheadMarkerIcon(new Segment(seg.Start, seg.End, 1), this, icon, 180f, 15);
                }
            }
            base.InitAdditionalCombatReplayData(log);
        }

        public IReadOnlyList<Point3D> GetCombatReplayActivePositions(ParsedEvtcLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            (IReadOnlyList<Segment> deads, _, IReadOnlyList<Segment> dcs) = GetStatus(log);
            var activePositions = new List<ParametricPoint3D>(GetCombatReplayPolledPositions(log));
            for (int i = 0; i < activePositions.Count; i++)
            {
                ParametricPoint3D cur = activePositions[i];
                foreach (Segment seg in deads)
                {
                    if (seg.ContainsPoint(cur.Time))
                    {
                        activePositions[i] = null;
                    }
                }
                foreach (Segment seg in dcs)
                {
                    if (seg.ContainsPoint(cur.Time))
                    {
                        activePositions[i] = null;
                    }
                }
            }
            return activePositions;
        }

    }
}
