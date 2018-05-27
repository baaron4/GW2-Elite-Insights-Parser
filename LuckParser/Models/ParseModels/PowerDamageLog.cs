using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LuckParser.Models.ParseEnums;

namespace LuckParser.Models.ParseModels
{
    public class PowerDamageLog : DamageLog
    {

        // Constructor
        public PowerDamageLog(long time, CombatItem c) : base(time, c)
        {
            damage =  c.getValue();
        }
    }
}