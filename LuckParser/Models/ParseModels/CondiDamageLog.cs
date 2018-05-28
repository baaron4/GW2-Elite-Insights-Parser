using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LuckParser.Models.ParseEnums;

namespace LuckParser.Models.ParseModels
{
    public class CondiDamageLog : DamageLog
    {

        // Constructor
        public CondiDamageLog(long time, CombatItem c) : base(time, c)
        {
            damage = c.getBuffDmg();
        }
    }
}