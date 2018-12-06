using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class JsonDefenses
    {
        public JsonDefenses(int phaseCount)
        {
            blockedCount = new int[phaseCount];
            damageBarrier = new int[phaseCount];
            damageInvulned = new int[phaseCount];
            damageTaken = new long[phaseCount];
            evadedCount = new int[phaseCount];
            dodgeCount = new int[phaseCount];
            interruptedCount = new int[phaseCount];
            invulnedCount = new int[phaseCount];
            downCount = new int[phaseCount];
            downDuration = new int[phaseCount];
            deadCount = new int[phaseCount];
            deadDuration = new int[phaseCount];
            dcCount = new int[phaseCount];
            dcDuration = new int[phaseCount];
        }

        public long[] damageTaken;
        public int[] blockedCount;
        public int[] evadedCount;
        public int[] dodgeCount;
        public int[] invulnedCount;
        public int[] damageInvulned;
        public int[] damageBarrier;
        public int[] interruptedCount;
        public int[] downCount;
        public int[] downDuration;
        public int[] deadCount;
        public int[] deadDuration;
        public int[] dcCount;
        public int[] dcDuration;
    }
}
