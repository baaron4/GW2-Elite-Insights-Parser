using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckParser.Models.ParseModels;

namespace LuckParser.Models.DataModels
{
    public class Statistics
    {
        public Statistics()
        {
            finalDps = new Dictionary<Player, FinalDPS[]>();
            finalDefenses = new Dictionary<Player, FinalDefenses[]>();
            finalStats = new Dictionary<Player, FinalStats[]>();
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

        public Dictionary<Player, FinalDPS[]> finalDps;
        public FinalDPS[] finalBossDps;

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

            // Counts
            public int swapCount;
            public int ressCount;
            public int downCount;
            public int dodgeCount;

            public double died;
            public double dcd;
        }

        public Dictionary<Player, FinalStats[]> finalStats;

        public class FinalDefenses
        {
            public long damageTaken;
            public int blockedCount;
            public int evadedCount;
            public int invulnedCount;
            public int damageInvulned;
            public int damageBarrier;
        }

        public Dictionary<Player, FinalDefenses[]> finalDefenses;
    }
}
