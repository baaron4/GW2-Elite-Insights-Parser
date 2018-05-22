using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckParser.Models.ParseModels;
namespace LuckParser.Models
{
    public class GW2APISkillCheck :GW2APISkill
    {
        
        public GW2APIfacts[] facts { get; set; }

        public GW2APISkillCheck() { }
    }
}
