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
            public long Skill;
            public bool Condi;
            public string Src;
            public int Damage;
            public int Time;

            public JsonDeathRecapDamageItem(ParseModels.Player.DeathRecap.DeathRecapDamageItem item)
            {
                Skill = item.Skill;
                Condi = item.Condi;
                Src = item.Src;
                Damage = item.Damage;
                Time = item.Time;
            }
        }

        public int Time;
        public List<JsonDeathRecapDamageItem> ToDown;
        public List<JsonDeathRecapDamageItem> ToKill;

        public JsonDeathRecap(ParseModels.Player.DeathRecap recap)
        {
            Time = recap.Time;
            ToDown = recap.ToDown?.Select(x => new JsonDeathRecapDamageItem(x)).ToList();
            ToKill = recap.ToKill?.Select(x => new JsonDeathRecapDamageItem(x)).ToList();
        }

    }
}
