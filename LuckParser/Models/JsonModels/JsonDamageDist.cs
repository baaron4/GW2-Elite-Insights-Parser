using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.JsonModels
{
    public class JsonDamageDist
    {
        public int Damage;
        public int Min;
        public int Max;
        public int Hits;
        public int Crit;
        public int Glance;
        public int Flank;

        public JsonDamageDist(List<ParseModels.DamageLog> list)
        {
            Hits = list.Count;
            Damage = list.Sum(x => x.Damage);
            Min = list.Min(x => x.Damage);
            Max = list.Max(x => x.Damage);
            Flank = list.Count(x => x.IsFlanking);
            Crit = list.Count(x => x.Result == Parser.ParseEnum.Result.Crit);
            Glance = list.Count(x => x.Result == Parser.ParseEnum.Result.Glance);
        }
    }
}
