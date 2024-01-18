using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    public class Player : AbstractPlayer
    {
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

        /// <summary>
        /// Checks if the player has a Commander Tag GUID.
        /// </summary>
        /// <param name="log"></param>
        /// <returns><see langword="true"/> if a GUID was found, otherwise <see langword="false"/></returns>
        public bool IsCommander(ParsedEvtcLog log)
        {
            return IsCommander(log, out _);
        }

        public bool IsCommander(ParsedEvtcLog log, out string tagGUID)
        {
            IReadOnlyList<TagEvent> tagEvents = log.CombatData.GetTagEvents(AgentItem);

            if (tagEvents.Count > 0)
            {
                foreach (TagEvent tagEvent in tagEvents)
                {
                    MarkerGUIDEvent marker = log.CombatData.GetMarkerGUIDEvent(tagEvent.TagID);
                    if (marker != null)
                    {
                        if (MarkerGUIDs.CommanderTagMarkersHexGUIDs.Contains(marker.HexContentGUID))
                        {
                            tagGUID = marker.HexContentGUID;
                            return true;
                        }
                    }
                    else if (tagEvent.TagID != 0)
                    {
                        tagGUID = MarkerGUIDs.BlueCommanderTag;
                        return true;
                    }
                }
            }

            tagGUID = null;
            return false;
        }

        protected override void InitAdditionalCombatReplayData(ParsedEvtcLog log)
        {
            base.InitAdditionalCombatReplayData(log);
            if (IsCommander(log, out string tagGUID))
            {
                CombatReplay.AddRotatedOverheadIcon(new Segment(FirstAware, LastAware, 1), this, MarkerGUIDs.CommanderTagToIcon[tagGUID], 180f, 15);
            }
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
