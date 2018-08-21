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
            Dps = new Dictionary<Player, FinalDPS[]>();
            Defenses = new Dictionary<Player, FinalDefenses[]>();
            Stats = new Dictionary<Player, FinalStats[]>();
            Support = new Dictionary<Player, FinalSupport[]>();
            SelfBoons = new Dictionary<Player, Dictionary<long, FinalBoonUptime>[]>();
            GroupBoons = new Dictionary<Player, Dictionary<long, FinalBoonUptime>[]>();
            OffGroupBoons = new Dictionary<Player, Dictionary<long, FinalBoonUptime>[]>();
            SquadBoons = new Dictionary<Player, Dictionary<long, FinalBoonUptime>[]>();
            Phases = new List<PhaseData>();
        }

        public List<PhaseData> Phases;

        public class FinalDPS
        {
            // Total
            public int AllDps;
            public int AllDamage;
            public int AllCondiDps;
            public int AllCondiDamage;
            public int AllPowerDps;
            public int AllPowerDamage;
            // Boss
            public int BossDps;
            public int BossDamage;
            public int BossCondiDps;
            public int BossCondiDamage;
            public int BossPowerDps;
            public int BossPowerDamage;
            // Player only
            public int PlayerPowerDamage;
            public int PlayerBossPowerDamage;
        }

        public Dictionary<Player, FinalDPS[]> Dps;
        public FinalDPS[] BossDps;

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
            public int Interupts;
            public int Invulned;
            public int Wasted;
            public double TimeWasted;
            public int Saved;
            public double TimeSaved;
            public double AvgBoons;
            public double StackDist;
            //Boss only Rates
            public int PowerLoopCountBoss;
            public int CritablePowerLoopCountBoss;
            public int CriticalRateBoss;
            public int CriticalDmgBoss;
            public int ScholarRateBoss;
            public int ScholarDmgBoss;
            public int MovingRateBoss;
            public int MovingDamageBoss;
            public int FlankingRateBoss;
            public int GlanceRateBoss;
            public int MissedBoss;
            public int InteruptsBoss;
            public int InvulnedBoss;

            // Counts
            public int SwapCount;
            public int RessCount;
            public int DownCount;
            public int DodgeCount;

            

            public double Died;
            public double Dcd;
        }

        public Dictionary<Player, FinalStats[]> Stats;

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

        public Dictionary<Player, FinalDefenses[]> Defenses;

        public class FinalSupport
        {
            //public long allHeal;
            public int Resurrects;
            public float RessurrectTime;
            public int CondiCleanse;
            public float CondiCleanseTime;
        }

        public Dictionary<Player, FinalSupport[]> Support;

        public class FinalBoonUptime
        {
            public double Uptime;
            public double Generation;
            public double Overstack;
        }

        public Dictionary<Player, Dictionary<long, FinalBoonUptime>[]> SelfBoons;
        public Dictionary<Player, Dictionary<long, FinalBoonUptime>[]> GroupBoons;
        public Dictionary<Player, Dictionary<long, FinalBoonUptime>[]> OffGroupBoons;
        public Dictionary<Player, Dictionary<long, FinalBoonUptime>[]> SquadBoons;

        public class FinalBossBoon
        {
            public FinalBossBoon(List<Player> plist)
            {
                Uptime = 0;
                Generated = new Dictionary<Player, double>();
                Overstacked = new Dictionary<Player, double>();
                foreach (Player p in plist)
                {
                    Generated.Add(p, 0);
                    Overstacked.Add(p, 0);
                }
            }

            public double Uptime;
            public Dictionary<Player, double> Generated;
            public Dictionary<Player, double> Overstacked;
        }

        public Dictionary<long, FinalBossBoon>[] BossConditions;

        // present buff
        public List<Boon> PresentBoons = new List<Boon>();//Used only for Boon tables
        public List<Boon> PresentOffbuffs = new List<Boon>();//Used only for Off Buff tables
        public List<Boon> PresentDefbuffs = new List<Boon>();//Used only for Def Buff tables
        public Dictionary<long, List<Boon>> PresentPersonnalBuffs = new Dictionary<long, List<Boon>>();//Used only for personnal

        //Positions for group
        public List<Point3D> StackCenterPositions;
    }
}
