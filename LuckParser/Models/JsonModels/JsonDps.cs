using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.JsonModels
{
    public class JsonDps
    {
        public JsonDps(int phaseCount)
        {
            condiDamage = new int[phaseCount];
            condiDps = new int[phaseCount];
            damage = new int[phaseCount];
            dps = new int[phaseCount];
            powerDamage = new int[phaseCount];
            powerDps = new int[phaseCount];
        }

        public int[] dps;
        public int[] damage;
        public int[] condiDps;
        public int[] condiDamage;
        public int[] powerDps;
        public int[] powerDamage;
    }
}
