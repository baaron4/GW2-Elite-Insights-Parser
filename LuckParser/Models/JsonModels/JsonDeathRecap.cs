using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.JsonModels
{
    public class JsonDeathRecap
    {
        public class JsonDeathRecapDamageItem
        {
            public long skillID;
            public bool condi;
            public string source;
            public int damage;
            public int time;
        }

        public int time;
        public List<JsonDeathRecapDamageItem> toDown;
        public List<JsonDeathRecapDamageItem> toKill;
    }
}
