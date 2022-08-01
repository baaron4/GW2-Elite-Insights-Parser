using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    public class Player : AbstractPlayer
    {
        // Fields

        private int _isCommander = -1;

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

        internal void Anonymize(int index)
        {
            Character = "Player " + index;
            Account = "Account " + index;
            AgentItem.OverrideName(Character+"\0:" + Account+ "\01");
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

        public bool IsCommander(ParsedEvtcLog log)
        {
            if (_isCommander == -1)
            {
                IReadOnlyList<TagEvent> tagEvents = log.CombatData.GetTagEvents(AgentItem);
                foreach (TagEvent tagEvent in tagEvents)
                {
                    MarkerGUIDEvent marker = log.CombatData.GetMarkerGUIDEvent(tagEvent.TagID);
                    if (marker != null)
                    {
                        switch (marker.ContentGUID)
                        {
                            case MarkerGUIDs.BlueCommanderTag:
                            case MarkerGUIDs.CyanCommanderTag:
                            case MarkerGUIDs.GreenCommanderTag:
                            case MarkerGUIDs.OrangeCommanderTag:
                            case MarkerGUIDs.PinkCommanderTag:
                            case MarkerGUIDs.PurpleCommanderTag:
                            case MarkerGUIDs.RedCommanderTag:
                            case MarkerGUIDs.WhiteCommanderTag:
                            case MarkerGUIDs.YellowCommanderTag:
                            case MarkerGUIDs.BlueCatmanderTag:
                            case MarkerGUIDs.CyanCatmanderTag:
                            case MarkerGUIDs.GreenCatmanderTag:
                            case MarkerGUIDs.OrangeCatmanderTag:
                            case MarkerGUIDs.PinkCatmanderTag:
                            case MarkerGUIDs.PurpleCatmanderTag:
                            case MarkerGUIDs.RedCatmanderTag:
                            case MarkerGUIDs.WhiteCatmanderTag:
                            case MarkerGUIDs.YellowCatmanderTag:
                                _isCommander = 1;
                                return true;
                            default:
                                _isCommander = 0;
                                break;
                        }
                    }
                    else if (tagEvent.TagID != 0)
                    {
                        _isCommander = 1;
                        return true;
                    }
                }
                _isCommander = 0;
            }            
            return _isCommander == 1;
        }

        public IReadOnlyList<Point3D> GetCombatReplayActivePositions(ParsedEvtcLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            (IReadOnlyList<(long start, long end)> deads, _, IReadOnlyList<(long start, long end)> dcs) = GetStatus(log);
            var activePositions = new List<Point3D>(GetCombatReplayPolledPositions(log));
            for (int i = 0; i < activePositions.Count; i++)
            {
                Point3D cur = activePositions[i];
                foreach ((long start, long end) in deads)
                {
                    if (cur.Time >= start && cur.Time <= end)
                    {
                        activePositions[i] = null;
                    }
                }
                foreach ((long start, long end) in dcs)
                {
                    if (cur.Time >= start && cur.Time <= end)
                    {
                        activePositions[i] = null;
                    }
                }
            }
            return activePositions;
        }

    }
}
