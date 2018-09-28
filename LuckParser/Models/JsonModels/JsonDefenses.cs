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
            InvulnedCount = new int[phaseCount];
        }

        public long[] DamageTaken;
        public int[] BlockedCount;
        public int[] EvadedCount;
        public int[] InvulnedCount;
        public int[] DamageInvulned;
        public int[] DamageBarrier;
    }
}
