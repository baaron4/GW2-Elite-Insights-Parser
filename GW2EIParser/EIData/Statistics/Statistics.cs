using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.EIData.Buff;

namespace GW2EIParser.Models
{
    /// <summary>
    /// Passes statistical information
    /// </summary>
    public class Statistics
    {
        public Statistics(CombatData combatData, List<Player> players, BuffsContainer boons)
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

        public class FinalSupport
        {
            //public long allHeal;
            public long Resurrects { get; set; }
            public double ResurrectTime { get; set; }
            public long CondiCleanse { get; set; }
            public double CondiCleanseTime { get; set; }
            public long CondiCleanseSelf { get; set; }
            public double CondiCleanseTimeSelf { get; set; }
            public long BoonStrips { get; set; }
            public double BoonStripsTime { get; set; }
        }

        public class FinalBuffs
        {
            public double Uptime { get; set; }
            public double Generation { get; set; }
            public double Overstack { get; set; }
            public double Wasted { get; set; }
            public double UnknownExtended { get; set; }
            public double ByExtension { get; set; }
            public double Extended { get; set; }
            public double Presence { get; set; }
        }

        public enum BuffEnum { Self, Group, OffGroup, Squad };

        public class FinalTargetBuffs
        {
            public FinalTargetBuffs(List<Player> plist)
            {
                Uptime = 0;
                Presence = 0;
                Generated = new Dictionary<Player, double>();
                Overstacked = new Dictionary<Player, double>();
                Wasted = new Dictionary<Player, double>();
                UnknownExtension = new Dictionary<Player, double>();
                Extension = new Dictionary<Player, double>();
                Extended = new Dictionary<Player, double>();
                foreach (Player p in plist)
                {
                    Generated.Add(p, 0);
                    Overstacked.Add(p, 0);
                    Wasted.Add(p, 0);
                    UnknownExtension.Add(p, 0);
                    Extension.Add(p, 0);
                    Extended.Add(p, 0);
                }
            }

            public double Uptime { get; set; }
            public double Presence { get; set; }
            public Dictionary<Player, double> Generated { get; }
            public Dictionary<Player, double> Overstacked { get; }
            public Dictionary<Player, double> Wasted { get; }
            public Dictionary<Player, double> UnknownExtension { get; }
            public Dictionary<Player, double> Extension { get; }
            public Dictionary<Player, double> Extended { get; }
        }

        public class DamageModifierData
        {
            public int HitCount { get; }
            public int TotalHitCount { get; }
            public double DamageGain { get; }
            public int TotalDamage { get; }

            public DamageModifierData(int hitCount, int totalHitCount, double damageGain, int totalDamage)
            {
                HitCount = hitCount;
                TotalHitCount = totalHitCount;
                DamageGain = damageGain;
                TotalDamage = totalDamage;
            }
        }


        public class Consumable
        {
            public Buff Buff { get; }
            public long Time { get; }
            public int Duration { get; }
            public int Stack { get; set; }

            public Consumable(Buff item, long time, int duration)
            {
                Buff = item;
                Time = time;
                Duration = duration;
                Stack = 1;
            }
        }

        public class DeathRecap
        {
            public class DeathRecapDamageItem
            {
                public long ID { get; set; }
                public bool IndirectDamage { get; set; }
                public string Src { get; set; }
                public int Damage { get; set; }
                public int Time { get; set; }
            }

            public int DeathTime { get; set; }
            public List<DeathRecapDamageItem> ToDown { get; set; }
            public List<DeathRecapDamageItem> ToKill { get; set; }
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
