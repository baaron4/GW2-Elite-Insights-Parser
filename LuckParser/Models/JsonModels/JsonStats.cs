using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class JsonStats
    {
        public JsonStats(int phaseCount)
        {
            powerLoopCount = new int[phaseCount];
            critablePowerLoopCount = new int[phaseCount];
            criticalRate = new int[phaseCount];
            criticalDmg = new int[phaseCount];
            scholarRate = new int[phaseCount];
            scholarDmg = new int[phaseCount];
            eagleRate = new int[phaseCount];
            eagleDmg = new int[phaseCount];
            movingRate = new int[phaseCount];
            movingDamage = new int[phaseCount];
            flankingDmg = new int[phaseCount];
            flankingRate = new int[phaseCount];
            glanceRate = new int[phaseCount];
            missed = new int[phaseCount];
            interrupts = new int[phaseCount];
            invulned = new int[phaseCount];
            powerDamage = new int[phaseCount];
        }

        // Rates
        public int[] powerLoopCount;
        public int[] critablePowerLoopCount;
        public int[] criticalRate;
        public int[] criticalDmg;
        public int[] scholarRate;
        public int[] scholarDmg;
        public int[] eagleRate;
        public int[] eagleDmg;
        public int[] movingRate;
        public int[] movingDamage;
        public int[] flankingDmg;
        public int[] flankingRate;
        public int[] glanceRate;
        public int[] missed;
        public int[] interrupts;
        public int[] invulned;
        public int[] powerDamage;
    }
}
