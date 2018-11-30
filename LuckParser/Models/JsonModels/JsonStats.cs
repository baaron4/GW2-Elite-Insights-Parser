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
            PowerLoopCount = new int[phaseCount];
            CritablePowerLoopCount = new int[phaseCount];
            CriticalRate = new int[phaseCount];
            CriticalDmg = new int[phaseCount];
            ScholarRate = new int[phaseCount];
            ScholarDmg = new int[phaseCount];
            EagleRate = new int[phaseCount];
            EagleDmg = new int[phaseCount];
            MovingRate = new int[phaseCount];
            MovingDamage = new int[phaseCount];
            FlankingDmg = new int[phaseCount];
            FlankingRate = new int[phaseCount];
            GlanceRate = new int[phaseCount];
            Missed = new int[phaseCount];
            Interrupts = new int[phaseCount];
            Invulned = new int[phaseCount];
            PlayerPowerDamage = new int[phaseCount];
        }

        // Rates
        public int[] PowerLoopCount;
        public int[] CritablePowerLoopCount;
        public int[] CriticalRate;
        public int[] CriticalDmg;
        public int[] ScholarRate;
        public int[] ScholarDmg;
        public int[] EagleRate;
        public int[] EagleDmg;
        public int[] MovingRate;
        public int[] MovingDamage;
        public int[] FlankingDmg;
        public int[] FlankingRate;
        public int[] GlanceRate;
        public int[] Missed;
        public int[] Interrupts;
        public int[] Invulned;
        public int[] PlayerPowerDamage;
    }
}
