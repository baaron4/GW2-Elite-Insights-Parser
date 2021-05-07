using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    /// <summary>
    /// Passes statistical information
    /// </summary>
    public class StatisticsHelper
    {
        internal StatisticsHelper(CombatData combatData, IReadOnlyList<Player> players, BuffsContainer buffs)
        {
            IReadOnlyCollection<long> skillIDs = combatData.GetSkills();
            // Main boons
            foreach (Buff boon in buffs.BuffsByNature[BuffNature.Boon])
            {
                if (skillIDs.Contains(boon.ID))
                {
                    _presentBoons.Add(boon);
                }
            }
            // Main Conditions
            foreach (Buff condition in buffs.BuffsByNature[BuffNature.Condition])
            {
                if (skillIDs.Contains(condition.ID))
                {
                    _presentConditions.Add(condition);
                }
            }

            // Important class specific boons
            foreach (Buff offensiveBuff in buffs.BuffsByNature[BuffNature.OffensiveBuffTable])
            {
                if (skillIDs.Contains(offensiveBuff.ID))
                {
                    _presentOffbuffs.Add(offensiveBuff);
                }
            }

            foreach (Buff supportBuff in buffs.BuffsByNature[BuffNature.SupportBuffTable])
            {
                if (skillIDs.Contains(supportBuff.ID))
                {
                    _presentSupbuffs.Add(supportBuff);
                }
            }

            foreach (Buff defensiveBuff in buffs.BuffsByNature[BuffNature.DefensiveBuffTable])
            {
                if (skillIDs.Contains(defensiveBuff.ID))
                {
                    _presentDefbuffs.Add(defensiveBuff);
                }

            }

            foreach (Buff gearBuff in buffs.BuffsByNature[BuffNature.GearBuffTable])
            {
                if (skillIDs.Contains(gearBuff.ID))
                {
                    _presentGearbuffs.Add(gearBuff);
                }

            }

            foreach (Buff fractalInstability in buffs.BuffsBySource[ParserHelper.Source.FractalInstability])
            {
                if (skillIDs.Contains(fractalInstability.ID))
                {
                    _presentFractalInstabilities.Add(fractalInstability);
                }
            }

            // All class specific boons
            var remainingBuffsByIds = buffs.BuffsByNature[BuffNature.GraphOnlyBuff].GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList().FirstOrDefault());
            foreach (Player player in players)
            {
                _presentRemainingBuffsPerPlayer[player] = new HashSet<Buff>();
                foreach (AbstractBuffEvent item in combatData.GetBuffData(player.AgentItem))
                {
                    if (item is BuffApplyEvent && item.To == player.AgentItem && remainingBuffsByIds.TryGetValue(item.BuffID, out Buff boon))
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
        public IReadOnlyList<Buff> PresentGearbuffs => _presentGearbuffs;//Used only for Gear Buff tables
        public IReadOnlyList<Buff> PresentFractalInstabilities => _presentFractalInstabilities;

        public IReadOnlyCollection<Buff> GetPresentRemainingBuffsOnPlayer(Player p)
        {
            if (_presentRemainingBuffsPerPlayer.TryGetValue(p, out HashSet<Buff> buffs))
            {
                return buffs;
            }
            return new HashSet<Buff>();
        }

        //

        private readonly List<Buff> _presentBoons = new List<Buff>();//Used only for Boon tables
        private readonly List<Buff> _presentConditions  = new List<Buff>();//Used only for Condition tables
        private readonly List<Buff> _presentOffbuffs  = new List<Buff>();//Used only for Off Buff tables
        private readonly List<Buff> _presentSupbuffs  = new List<Buff>();//Used only for Off Buff tables
        private readonly List<Buff> _presentDefbuffs = new List<Buff>();//Used only for Def Buff tables
        private readonly List<Buff> _presentGearbuffs = new List<Buff>();//Used only for Gear Buff tables
        private readonly List<Buff> _presentFractalInstabilities = new List<Buff>();
        private readonly Dictionary<Player, HashSet<Buff>> _presentRemainingBuffsPerPlayer  = new Dictionary<Player, HashSet<Buff>>();


        //Positions for group
        private List<Point3D> _stackCenterPositions = null;
        private List<Point3D> _stackCommanderPositions = null;

        public IReadOnlyList<Point3D> GetStackCenterPositions(ParsedEvtcLog log)
        {
            if (_stackCenterPositions == null)
            {
                SetStackCenterPositions(log);
            }
            return _stackCenterPositions;
        }

        public IReadOnlyList<Point3D> GetStackCommanderPositions(ParsedEvtcLog log)
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
                var GroupsPosList = new List<List<Point3D>>();
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
