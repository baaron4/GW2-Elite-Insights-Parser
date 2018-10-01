using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class JsonDeathRecap
    {
        public class DamageItem
        {
            public long Skill;
            public int Condi;
            public string Src;
            public int Damage;
            public int Time;
        }

        public int Time;
        public List<DamageItem> ToDown;
        public List<DamageItem> ToKill;
    }
}
