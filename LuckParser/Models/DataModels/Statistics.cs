using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            dps = new Dictionary<Player, FinalDPS[]>();
            defenses = new Dictionary<Player, FinalDefenses[]>();
            stats = new Dictionary<Player, FinalStats[]>();
            support = new Dictionary<Player, FinalSupport[]>();
            selfBoons = new Dictionary<Player, Dictionary<int, FinalBoonUptime>[]>();
            groupBoons = new Dictionary<Player, Dictionary<int, FinalBoonUptime>[]>();
            offGroupBoons = new Dictionary<Player, Dictionary<int, FinalBoonUptime>[]>();
            squadBoons = new Dictionary<Player, Dictionary<int, FinalBoonUptime>[]>();
        }

        public class FinalDPS
        {
            public int allDps;
            public int allDamage;
            public int allCondiDps;
            public int allCondiDamage;
            public int allPowerDps;
            public int allPowerDamage;
            public int bossDps;
            public int bossDamage;
            public int bossCondiDps;
            public int bossCondiDamage;
            public int bossPowerDps;
            public int bossPowerDamage;
        }

        public Dictionary<Player, FinalDPS[]> dps;
        public FinalDPS[] bossDps;

        public class FinalStats
        {
            // Rates
            public int powerLoopCount;
            public int criticalRate;
            public int criticalDmg;
            public int scholarRate;
            public int scholarDmg;
            public int totalDmg;
            public int movingRate;
            public int flankingRate;
            public int glanceRate;
            public int missed;
            public int interupts;
            public int invulned;
            public int wasted;
            public double timeWasted;
            public int saved;
            public double timeSaved;
            public double avgBoons;

            //Boss only Rates
            public int powerLoopCountBoss;
            public int criticalRateBoss;
            public int criticalDmgBoss;
            public int scholarRateBoss;
            public int scholarDmgBoss;
            public int totalDmgBoss;
            public int movingRateBoss;
            public int flankingRateBoss;
            public int glanceRateBoss;
            public int missedBoss;
            public int interuptsBoss;
            public int invulnedBoss;

            // Counts
            public int swapCount;
            public int ressCount;
            public int downCount;
            public int dodgeCount;

            public double died;
            public double dcd;
        }

        public Dictionary<Player, FinalStats[]> stats;

        public class FinalDefenses
        {
            public long damageTaken;
            public int blockedCount;
            public int evadedCount;
            public int invulnedCount;
            public int damageInvulned;
            public int damageBarrier;
        }

        public Dictionary<Player, FinalDefenses[]> defenses;

        public class FinalSupport
        {
            public int resurrects;
            public float ressurrectTime;
            public int condiCleanse;
            public float condiCleanseTime;
        }

        public Dictionary<Player, FinalSupport[]> support;

        public class FinalBoonUptime
        {
            public double uptime;
            public double generation;
            public double overstack;
            public Boon.BoonType boonType;
        }

        public Dictionary<Player, Dictionary<int, FinalBoonUptime>[]> selfBoons;
        public Dictionary<Player, Dictionary<int, FinalBoonUptime>[]> groupBoons;
        public Dictionary<Player, Dictionary<int, FinalBoonUptime>[]> offGroupBoons;
        public Dictionary<Player, Dictionary<int, FinalBoonUptime>[]> squadBoons;

        public class FinalBossBoon
        {
            public FinalBossBoon()
            {
                uptime = 0;
                boonType = Boon.BoonType.Intensity;
            }

            public double uptime;
            public Boon.BoonType boonType;
        }

        public Dictionary<int, FinalBossBoon>[] bossConditions;

        // present buff
        public List<Boon> present_boons = new List<Boon>();//Used only for Boon tables
        public List<Boon> present_offbuffs = new List<Boon>();//Used only for Off Buff tables
        public List<Boon> present_defbuffs = new List<Boon>();//Used only for Def Buff tables
        public Dictionary<int, List<Boon>> present_personnal = new Dictionary<int, List<Boon>>();//Used only for personnal
    }
}
