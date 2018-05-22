using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class GW2APIfacts
    {
        public string text { get; set; }
        public string icon { get; set; }
        public string type { get; set; }

        public string target { get; set; }
        //Buff
        public string status { get; set; }
        public string description { get; set; }
        public int apply_count { get; set; }
        public int duration { get; set; }
        //ComboField
        public string field_type { get; set; }
        //ComboFinisher
        public string finisher_type { get; set; }
        public float percent { get; set; }
        //Damage
        public int hit_count { get; set; }
        public float dmg_multipler { get; set; }
        //Distance
        public int distance { get; set; }
        //Duration
        //public int duration { get; set; }
        //Heal
        //public int hit_count { get; set; }
        //HealingAdjust
        //NoData
        //Number
        //value
        //Percent
        //percent
        //PrefixedBuff
        public GW2APIfacts prefix { get; set; }

        public GW2APIfacts() { }
    }
}
