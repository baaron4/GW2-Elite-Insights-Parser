using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    /// <summary>
    /// Passes statistical information
    /// </summary>
    public class GeneralStatistics
    {
        internal GeneralStatistics(CombatData combatData, IReadOnlyList<Player> players, BuffsContainer boons)
        {
            HashSet<long> skillIDs = combatData.GetSkills();
            // Main boons
            foreach (Buff boon in boons.BuffsByNature[BuffNature.Boon])
            {
                if (skillIDs.Contains(boon.ID))
                {
                    _presentBoons.Add(boon);
                }
            }
            // Main Conditions
            foreach (Buff boon in boons.BuffsByNature[BuffNature.Condition])
            {
                if (skillIDs.Contains(boon.ID))
                {
                    _presentConditions.Add(boon);
                }
            }

            // Important class specific boons
            foreach (Buff boon in boons.BuffsByNature[BuffNature.OffensiveBuffTable])
            {
                if (skillIDs.Contains(boon.ID))
                {
                    _presentOffbuffs.Add(boon);
                }
            }

            foreach (Buff boon in boons.BuffsByNature[BuffNature.SupportBuffTable])
            {
                if (skillIDs.Contains(boon.ID))
                {
                    _presentSupbuffs.Add(boon);
                }
            }

            foreach (Buff boon in boons.BuffsByNature[BuffNature.DefensiveBuffTable])
            {
                if (skillIDs.Contains(boon.ID))
                {
                    _presentDefbuffs.Add(boon);
                }

            }

            foreach (Buff boon in boons.BuffsBySource[ParserHelper.Source.FractalInstability])
            {
                if (skillIDs.Contains(boon.ID))
                {
                    _presentFractalInstabilities.Add(boon);
                }
            }

            // All class specific boons
            var remainingBuffsByIds = boons.BuffsByNature[BuffNature.GraphOnlyBuff].GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList().FirstOrDefault());
            foreach (Player player in players)
            {
                PresentPersonalBuffs[player] = new HashSet<Buff>();
                foreach (AbstractBuffEvent item in combatData.GetBuffData(player.AgentItem))
                {
                    if (item is BuffApplyEvent && item.To == player.AgentItem && remainingBuffsByIds.TryGetValue(item.BuffID, out Buff boon))
                    {
                        PresentPersonalBuffs[player].Add(boon);
                    }
                }
            }
        }
        public IReadOnlyList<Buff> PresentBoons => _presentBoons;
        public IReadOnlyList<Buff> PresentConditions => _presentConditions;
        public IReadOnlyList<Buff> PresentOffbuffs => _presentOffbuffs;
        public IReadOnlyList<Buff> PresentSupbuffs => _presentSupbuffs;
        public IReadOnlyList<Buff> PresentDefbuffs => _presentDefbuffs;
        public IReadOnlyList<Buff> PresentFractalInstabilities => _presentFractalInstabilities;

        // present buff
        private List<Buff> _presentBoons { get; } = new List<Buff>();
        private List<Buff> _presentConditions { get; } = new List<Buff>();
        private List<Buff> _presentOffbuffs { get; } = new List<Buff>();
        private List<Buff> _presentSupbuffs { get; } = new List<Buff>();
        private List<Buff> _presentDefbuffs { get; } = new List<Buff>();
        private List<Buff> _presentFractalInstabilities { get; } = new List<Buff>();

        public Dictionary<Player, HashSet<Buff>> PresentPersonalBuffs { get; } = new Dictionary<Player, HashSet<Buff>>();

        //Positions for group
        private List<Point3D> _stackCenterPositions = null;
        private List<Point3D> _stackCommanderPositions = null;

        public List<Point3D> GetStackCenterPositions(ParsedEvtcLog log)
        {
            if (_stackCenterPositions == null)
            {
                SetStackCenterPositions(log);
            }
            return _stackCenterPositions;
        }

        public List<Point3D> GetStackCommanderPositions(ParsedEvtcLog log)
        {
            if (_stackCommanderPositions == null)
            {
                SetStackCommanderPositions(log);
            }
            return _stackCommanderPositions;
        }

        private void SetStackCenterPositions(ParsedEvtcLog log)
        {
            _stackCenterPositions = new List<Point3D>();
            if (log.CombatData.HasMovementData)
            {
                var GroupsPosList = new List<IReadOnlyList<Point3D>>();
                foreach (Player player in log.PlayerList)
                {
                    if (player.IsFakeActor)
                    {
                        continue;
                    }
                    GroupsPosList.Add(player.GetCombatReplayActivePositions(log));
                }
                for (int time = 0; time < GroupsPosList[0].Count; time++)
                {
                    float x = 0;
                    float y = 0;
                    float z = 0;
                    int activePlayers = GroupsPosList.Count;
                    foreach (List<Point3D> points in GroupsPosList)
                    {
                        Point3D point = points[time];
                        if (point != null)
                        {
                            x += point.X;
                            y += point.Y;
                            z += point.Z;
                        }
                        else
                        {
                            activePlayers--;
                        }

                    }
                    x /= activePlayers;
                    y /= activePlayers;
                    z /= activePlayers;
                    _stackCenterPositions.Add(new Point3D(x, y, z, ParserHelper.PollingRate * time));
                }
            }
        }
        private void SetStackCommanderPositions(ParsedEvtcLog log)
        {
            _stackCommanderPositions = new List<Point3D>();
            Player commander = log.PlayerList.FirstOrDefault(x => x.HasCommanderTag);
            if (log.CombatData.HasMovementData && commander != null)
            {
                _stackCommanderPositions = new List<Point3D>(commander.GetCombatReplayPolledPositions(log));
            }
        }

    }
}
