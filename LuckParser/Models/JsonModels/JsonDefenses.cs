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
            BlockedCount = new int[phaseCount];
            DamageBarrier = new int[phaseCount];
            DamageInvulned = new int[phaseCount];
            DamageTaken = new long[phaseCount];
            EvadedCount = new int[phaseCount];
            DodgeCount = new int[phaseCount];
            InterruptedCount = new int[phaseCount];
            InvulnedCount = new int[phaseCount];
            DownCount = new int[phaseCount];
            DownDuration = new int[phaseCount];
            DeadCount = new int[phaseCount];
            DeadDuration = new int[phaseCount];
            DCCount = new int[phaseCount];
            DCDuration = new int[phaseCount];
        }

        public long[] DamageTaken;
        public int[] BlockedCount;
        public int[] EvadedCount;
        public int[] DodgeCount;
        public int[] InvulnedCount;
        public int[] DamageInvulned;
        public int[] DamageBarrier;
        public int[] InterruptedCount;
        public int[] DownCount;
        public int[] DownDuration;
        public int[] DeadCount;
        public int[] DeadDuration;
        public int[] DCCount;
        public int[] DCDuration;
    }
}
