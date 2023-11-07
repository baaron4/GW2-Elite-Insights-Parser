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
            var remainingBuffsByIds = buffs.BuffsByClassification[BuffClassification.Other].GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList().FirstOrDefault());
            foreach (Player player in players)
            {
                _presentRemainingBuffsPerPlayer[player] = new HashSet<Buff>();
                foreach (AbstractBuffEvent item in combatData.GetBuffDataByDst(player.AgentItem))
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
        public IReadOnlyList<Buff> PresentDebuffs => _presentDebuffs;//Used only for Debuff tables
        public IReadOnlyList<Buff> PresentGearbuffs => _presentGearbuffs;//Used only for Gear Buff tables
        public IReadOnlyList<Buff> PresentNourishements => _presentNourishments;
        public IReadOnlyList<Buff> PresentEnhancements => _presentEnhancements;
        public IReadOnlyList<Buff> PresentOtherConsumables => _presentOtherConsumables;

        public IReadOnlyCollection<Buff> GetPresentRemainingBuffsOnPlayer(AbstractSingleActor actor)
        {
            if (!(actor is Player p))
            {
                return new HashSet<Buff>();
            }
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
        private readonly List<Buff> _presentDebuffs = new List<Buff>();//Used only for Debuff tables
        private readonly List<Buff> _presentGearbuffs = new List<Buff>();//Used only for Gear Buff tables
        private readonly List<Buff> _presentNourishments = new List<Buff>();
        private readonly List<Buff> _presentEnhancements = new List<Buff>();
        private readonly List<Buff> _presentOtherConsumables = new List<Buff>();
        private readonly Dictionary<Player, HashSet<Buff>> _presentRemainingBuffsPerPlayer  = new Dictionary<Player, HashSet<Buff>>();


        //Positions for group
        private List<ParametricPoint3D> _stackCenterPositions = null;
        private List<ParametricPoint3D> _stackCommanderPositions = null;

        public IReadOnlyList<ParametricPoint3D> GetStackCenterPositions(ParsedEvtcLog log)
        {
            if (_stackCenterPositions == null)
            {
                SetStackCenterPositions(log);
            }
            return _stackCenterPositions;
        }

        public IReadOnlyList<ParametricPoint3D> GetStackCommanderPositions(ParsedEvtcLog log)
        {
            if (_stackCommanderPositions == null)
            {
                SetStackCommanderPositions(log);
            }
            return _stackCommanderPositions;
        }

        private void SetStackCenterPositions(ParsedEvtcLog log)
        {
            _stackCenterPositions = new List<ParametricPoint3D>();
            if (log.CombatData.HasMovementData)
            {
                var GroupsPosList = new List<IReadOnlyList<Point3D>>();
                foreach (Player player in log.PlayerList)
                {
                    GroupsPosList.Add(player.GetCombatReplayActivePositions(log));
                }
                for (int time = 0; time < GroupsPosList[0].Count; time++)
                {
                    float x = 0;
                    float y = 0;
                    float z = 0;
                    int activePlayers = GroupsPosList.Count;
                    foreach (IReadOnlyList<Point3D> points in GroupsPosList)
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
                    if (activePlayers == 0)
                    {
                        _stackCenterPositions.Add(new ParametricPoint3D(x, y, z, ParserHelper.CombatReplayPollingRate * time));
                    } 
                    else
                    {
                        x /= activePlayers;
                        y /= activePlayers;
                        z /= activePlayers;
                        _stackCenterPositions.Add(new ParametricPoint3D(x, y, z, ParserHelper.CombatReplayPollingRate * time));
                    }
                }
            }
        }
        private void SetStackCommanderPositions(ParsedEvtcLog log)
        {
            _stackCommanderPositions = new List<ParametricPoint3D>();
            Player commander = log.PlayerList.FirstOrDefault(x => x.IsCommander(log));
            if (log.CombatData.HasMovementData && commander != null)
            {
                _stackCommanderPositions = new List<ParametricPoint3D>(commander.GetCombatReplayPolledPositions(log));
            }
        }

    }
}
