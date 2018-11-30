using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class JsonDps
    {
        public JsonDps(int phaseCount)
        {
            CondiDamage = new int[phaseCount];
            CondiDps = new int[phaseCount];
            Damage = new int[phaseCount];
            Dps = new int[phaseCount];
            PowerDamage = new int[phaseCount];
            PowerDps = new int[phaseCount];
        }

        public int[] Dps;
        public int[] Damage;
        public int[] CondiDps;
        public int[] CondiDamage;
        public int[] PowerDps;
        public int[] PowerDamage;
    }
}
