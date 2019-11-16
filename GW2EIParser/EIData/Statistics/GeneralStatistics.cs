using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.EIData.Buff;

namespace GW2EIParser.EIData
{
    /// <summary>
    /// Passes statistical information
    /// </summary>
    public class GeneralStatistics
    {
        public GeneralStatistics(CombatData combatData, List<Player> players, BuffsContainer boons)
        {
            HashSet<long> skillIDs = combatData.GetSkills();
            // Main boons
            foreach (Buff boon in boons.BuffsByNature[BuffNature.Boon])
            {
                if (skillIDs.Contains(boon.ID))
                {
                    PresentBoons.Add(boon);
                }
            }
            // Main Conditions
            foreach (Buff boon in boons.BuffsByNature[BuffNature.Condition])
            {
                if (skillIDs.Contains(boon.ID))
                {
                    PresentConditions.Add(boon);
                }
            }

            // Important class specific boons
            foreach (Buff boon in boons.BuffsByNature[BuffNature.OffensiveBuffTable])
            {
                if (skillIDs.Contains(boon.ID))
                {
                    PresentOffbuffs.Add(boon);
                }
            }

            foreach (Buff boon in boons.BuffsByNature[BuffNature.DefensiveBuffTable])
            {
                if (skillIDs.Contains(boon.ID))
                {
                    PresentDefbuffs.Add(boon);
                }

            }

            // All class specific boons
            var remainingBuffsByIds = boons.BuffsByNature[BuffNature.GraphOnlyBuff].GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList().FirstOrDefault());
            foreach (Player player in players)
            {
                PresentPersonalBuffs[player.InstID] = new HashSet<Buff>();
                foreach (AbstractBuffEvent item in combatData.GetBuffDataByDst(player.AgentItem))
                {
                    if (item is BuffApplyEvent && item.To == player.AgentItem && remainingBuffsByIds.TryGetValue(item.BuffID, out Buff boon))
                    {
                        PresentPersonalBuffs[player.InstID].Add(boon);
                    }
                }
            }
        }


        // present buff
        public List<Buff> PresentBoons { get; } = new List<Buff>();//Used only for Boon tables
        public List<Buff> PresentConditions { get; } = new List<Buff>();//Used only for Condition tables
        public List<Buff> PresentOffbuffs { get; } = new List<Buff>();//Used only for Off Buff tables
        public List<Buff> PresentDefbuffs { get; } = new List<Buff>();//Used only for Def Buff tables
        public Dictionary<ushort, HashSet<Buff>> PresentPersonalBuffs { get; } = new Dictionary<ushort, HashSet<Buff>>();

        //Positions for group
        private List<Point3D> _stackCenterPositions = null;

        public List<Point3D> GetStackCenterPositions(ParsedLog log)
        {
            if (_stackCenterPositions == null)
            {
                SetStackCenterPositions(log);
            }
            return _stackCenterPositions;
        }

        private void SetStackCenterPositions(ParsedLog log)
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
                    _stackCenterPositions.Add(new Point3D(x, y, z, GeneralHelper.PollingRate * time));
                }
            }
        }
    }
}
