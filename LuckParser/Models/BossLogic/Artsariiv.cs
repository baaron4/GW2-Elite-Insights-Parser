using LuckParser.Controllers;
using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class Artasariiv : FractalLogic
    {
        public Artasariiv() : base()
        {   
        }

        public override CombatReplayMap getCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/4wmuc8B.png",
                            Tuple.Create(914, 914),
                            Tuple.Create(8991, 112, 11731, 2812),
                            Tuple.Create(-24576, -24576, 24576, 24576),
                            Tuple.Create(11204, 4414, 13252, 6462));
        }
    
        public override string getReplayIcon()
        {
            return "https://wiki.guildwars2.com/images/b/b4/Artsariiv.jpg";
        }
    }
}
