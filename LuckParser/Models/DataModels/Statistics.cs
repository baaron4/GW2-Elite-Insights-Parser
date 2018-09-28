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
            DpsBoss = new Dictionary<Boss, Dictionary<Player, FinalDPS[]>>();
            DpsAll = new Dictionary<Player, FinalDPS[]>();
            Defenses = new Dictionary<Player, FinalDefenses[]>();
            StatsBoss = new Dictionary<Boss, Dictionary<Player, FinalBossStats[]>>();
            StatsAll = new Dictionary<Player, FinalStats[]>();
            Support = new Dictionary<Player, FinalSupport[]>();
            SelfBoons = new Dictionary<Player, Dictionary<long, FinalBoonUptime>[]>();
            GroupBoons = new Dictionary<Player, Dictionary<long, FinalBoonUptime>[]>();
            OffGroupBoons = new Dictionary<Player, Dictionary<long, FinalBoonUptime>[]>();
            SquadBoons = new Dictionary<Player, Dictionary<long, FinalBoonUptime>[]>();
            BossConditions = new Dictionary<Boss, Dictionary<long, FinalBossBoon>[]>();
            BossDps = new Dictionary<Boss, FinalDPS[]>();
            AvgBossConditions = new Dictionary<Boss, double[]>();
            AvgBossBoons = new Dictionary<Boss, double[]>();
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
            // Player only
            public int PlayerPowerDamage;
        }

        public readonly Dictionary<Boss, Dictionary<Player, FinalDPS[]>> DpsBoss;
        public readonly Dictionary<Player, FinalDPS[]> DpsAll;
        public Dictionary<Boss, FinalDPS[]> BossDps;

        public class FinalBossStats
        {
            public int PowerLoopCount;
            public int CritablePowerLoopCount;
            public int CriticalRate;
            public int CriticalDmg;
            public int ScholarRate;
            public int ScholarDmg;
            public int MovingRate;
            public int MovingDamage;
            public int FlankingRate;
            public int GlanceRate;
            public int Missed;
            public int Interrupts;
            public int Invulned;
        }

        public class FinalStats
        {
            // Rates
            public int PowerLoopCount;
            public int CritablePowerLoopCount;
            public int CriticalRate;
            public int CriticalDmg;
            public int ScholarRate;
            public int ScholarDmg;
            public int MovingRate;
            public int MovingDamage;
            public int FlankingRate;
            public int GlanceRate;
            public int Missed;
            public int Interrupts;
            public int Invulned;
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

        public readonly Dictionary<Boss, Dictionary<Player, FinalBossStats[]>> StatsBoss;
        public readonly Dictionary<Player, FinalStats[]> StatsAll;
        public readonly Dictionary<Boss, double[]> AvgBossConditions;
        public readonly Dictionary<Boss, double[]> AvgBossBoons;

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

        public class FinalBossBoon
        {
            public FinalBossBoon(List<Player> plist)
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

        public readonly Dictionary<Boss,Dictionary<long, FinalBossBoon>[]> BossConditions;

        public Dictionary<Boss, double[]> BossHealth { get; set; }

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
