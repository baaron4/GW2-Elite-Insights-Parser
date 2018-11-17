using System.Collections.Generic;
using LuckParser.Models.ParseModels;

namespace LuckParser.Models.DataModels
{
    /// <summary>
    /// Passes statistical information about dps logs
    /// </summary>
    public class Statistics
    {
        public Statistics()
        {
            DpsTarget = new Dictionary<Target, Dictionary<Player, FinalDPS[]>>();
            DpsAll = new Dictionary<Player, FinalDPS[]>();
            Defenses = new Dictionary<Player, FinalDefenses[]>();
            StatsTarget = new Dictionary<Target, Dictionary<Player, FinalStats[]>>();
            StatsAll = new Dictionary<Player, FinalStatsAll[]>();
            Support = new Dictionary<Player, FinalSupport[]>();
            SelfBoons = new Dictionary<Player, Dictionary<long, FinalBoonUptime>[]>();
            GroupBoons = new Dictionary<Player, Dictionary<long, FinalBoonUptime>[]>();
            OffGroupBoons = new Dictionary<Player, Dictionary<long, FinalBoonUptime>[]>();
            SquadBoons = new Dictionary<Player, Dictionary<long, FinalBoonUptime>[]>();
            TargetConditions = new Dictionary<Target, Dictionary<long, FinalTargetBoon>[]>();
            TargetDps = new Dictionary<Target, FinalDPS[]>();
            AvgTargetConditions = new Dictionary<Target, double[]>();
            AvgTargetBoons = new Dictionary<Target, double[]>();
            Phases = new List<PhaseData>();
        }

        public List<PhaseData> Phases;

        public class FinalDPS
        {
            // Total
            public int Dps;
            public int Damage;
            public int CondiDps;
            public int CondiDamage;
            public int PowerDps;
            public int PowerDamage;
        }

        public readonly Dictionary<Target, Dictionary<Player, FinalDPS[]>> DpsTarget;
        public readonly Dictionary<Player, FinalDPS[]> DpsAll;
        public Dictionary<Target, FinalDPS[]> TargetDps;

        public class FinalStats
        {
            public int PowerLoopCount;
            public int CritablePowerLoopCount;
            public int CriticalRate;
            public int CriticalDmg;
            public int ScholarRate;
            public int ScholarDmg;
            public int EagleRate;
            public int EagleDmg;
            public int MovingRate;
            public int MovingDamage;
            public int FlankingDmg;
            public int FlankingRate;
            public int GlanceRate;
            public int Missed;
            public int Interrupts;
            public int Invulned;
            public int PlayerPowerDamage;
        }

        public class FinalStatsAll : FinalStats
        {
            // Rates
            public int Wasted;
            public double TimeWasted;
            public int Saved;
            public double TimeSaved;
            public double StackDist;

            // boons
            public double AvgBoons;
            public double AvgConditions;

            // Counts
            public int SwapCount;
            public int DownCount;
            public int DodgeCount;

            // Misc
            public double Died;
            public double Dcd;
        }

        public readonly Dictionary<Target, Dictionary<Player, FinalStats[]>> StatsTarget;
        public readonly Dictionary<Player, FinalStatsAll[]> StatsAll;
        public readonly Dictionary<Target, double[]> AvgTargetConditions;
        public readonly Dictionary<Target, double[]> AvgTargetBoons;

        public class FinalDefenses
        {
            //public long allHealReceived;
            public long DamageTaken;
            public int BlockedCount;
            public int EvadedCount;
            public int InvulnedCount;
            public int DamageInvulned;
            public int DamageBarrier;
        }

        public readonly Dictionary<Player, FinalDefenses[]> Defenses;

        public class FinalSupport
        {
            //public long allHeal;
            public int Resurrects;
            public float ResurrectTime;
            public int CondiCleanse;
            public float CondiCleanseTime;
        }

        public readonly Dictionary<Player, FinalSupport[]> Support;

        public class FinalBoonUptime
        {
            public double Uptime;
            public double Generation;
            public double Overstack;
            public double Presence;
        }

        public readonly Dictionary<Player, Dictionary<long, FinalBoonUptime>[]> SelfBoons;
        public readonly Dictionary<Player, Dictionary<long, FinalBoonUptime>[]> GroupBoons;
        public readonly Dictionary<Player, Dictionary<long, FinalBoonUptime>[]> OffGroupBoons;
        public readonly Dictionary<Player, Dictionary<long, FinalBoonUptime>[]> SquadBoons;

        public class FinalTargetBoon
        {
            public FinalTargetBoon(List<Player> plist)
            {
                Uptime = 0;
                Presence = 0;
                Generated = new Dictionary<Player, double>();
                Overstacked = new Dictionary<Player, double>();
                foreach (Player p in plist)
                {
                    Generated.Add(p, 0);
                    Overstacked.Add(p, 0);
                }
            }

            public double Uptime;
            public double Presence;
            public readonly Dictionary<Player, double> Generated;
            public readonly Dictionary<Player, double> Overstacked;
        }

        public readonly Dictionary<Target,Dictionary<long, FinalTargetBoon>[]> TargetConditions;

        public Dictionary<Target, double[]> TargetHealth { get; set; }

        // present buff
        public readonly List<Boon> PresentBoons = new List<Boon>();//Used only for Boon tables
        public readonly List<Boon> PresentConditions = new List<Boon>();//Used only for Condition tables
        public readonly List<Boon> PresentOffbuffs = new List<Boon>();//Used only for Off Buff tables
        public readonly List<Boon> PresentDefbuffs = new List<Boon>();//Used only for Def Buff tables
        public readonly Dictionary<ushort, List<Boon>> PresentPersonalBuffs = new Dictionary<ushort,List<Boon>>();

        //Positions for group
        public List<Point3D> StackCenterPositions;
    }
}
