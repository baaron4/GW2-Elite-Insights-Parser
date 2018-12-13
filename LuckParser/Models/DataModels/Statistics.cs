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
            SelfBuffs = new Dictionary<Player, Dictionary<long, FinalBuffs>[]>();
            GroupBuffs = new Dictionary<Player, Dictionary<long, FinalBuffs>[]>();
            OffGroupBuffs = new Dictionary<Player, Dictionary<long, FinalBuffs>[]>();
            SquadBuffs = new Dictionary<Player, Dictionary<long, FinalBuffs>[]>();
            TargetBuffs = new Dictionary<Target, Dictionary<long, FinalTargetBuffs>[]>();
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
            public int PowerDamage;
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
            public int DodgeCount;
            public int InvulnedCount;
            public int DamageInvulned;
            public int DamageBarrier;
            public int InterruptedCount;
            public int DownCount;
            public int DownDuration;
            public int DeadCount;
            public int DeadDuration;
            public int DcCount;
            public int DcDuration;
        }

        public readonly Dictionary<Player, FinalDefenses[]> Defenses;

        public class FinalSupport
        {
            //public long allHeal;
            public int Resurrects;
            public double ResurrectTime;
            public int CondiCleanse;
            public double CondiCleanseTime;
        }

        public readonly Dictionary<Player, FinalSupport[]> Support;

        public class FinalBuffs
        {
            public double Uptime;
            public double Generation;
            public double Overstack;
            public double Wasted;
            public double UnknownExtension;
            public double Presence;
        }

        public readonly Dictionary<Player, Dictionary<long, FinalBuffs>[]> SelfBuffs;
        public readonly Dictionary<Player, Dictionary<long, FinalBuffs>[]> GroupBuffs;
        public readonly Dictionary<Player, Dictionary<long, FinalBuffs>[]> OffGroupBuffs;
        public readonly Dictionary<Player, Dictionary<long, FinalBuffs>[]> SquadBuffs;

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
                foreach (Player p in plist)
                {
                    Generated.Add(p, 0);
                    Overstacked.Add(p, 0);
                    Wasted.Add(p, 0);
                }
            }

            public double Uptime;
            public double Presence;
            public readonly Dictionary<Player, double> Generated;
            public readonly Dictionary<Player, double> Overstacked;
            public readonly Dictionary<Player, double> Wasted;
            public readonly Dictionary<Player, double> UnknownExtension;
        }

        public readonly Dictionary<Target,Dictionary<long, FinalTargetBuffs>[]> TargetBuffs;

        public Dictionary<Target, double[]>[] TargetsHealth { get; set; }

        // present buff
        public readonly List<Boon> PresentBoons = new List<Boon>();//Used only for Boon tables
        public readonly List<Boon> PresentConditions = new List<Boon>();//Used only for Condition tables
        public readonly List<Boon> PresentOffbuffs = new List<Boon>();//Used only for Off Buff tables
        public readonly List<Boon> PresentDefbuffs = new List<Boon>();//Used only for Def Buff tables
        public readonly Dictionary<ushort, HashSet<Boon>> PresentPersonalBuffs = new Dictionary<ushort, HashSet<Boon>>();

        //Positions for group
        public List<Point3D> StackCenterPositions;
    }
}
