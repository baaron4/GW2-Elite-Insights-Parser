using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class GW2APISkillDetailed : GW2APISkill
    {
        public GW2APIfactsDetail[] facts { get; set; }

        public GW2APISkillDetailed() { }
    }
}
