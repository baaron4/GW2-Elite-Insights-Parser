using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class Skorvald : FractalLogic
    {
        public Skorvald() : base()
        {        
        }

        public override CombatReplayMap getCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/PO3aoJD.png",
                            Tuple.Create(1759, 1783),
                            Tuple.Create(-22267, 14955, -17227, 20735),
                            Tuple.Create(-24576, -24576, 24576, 24576),
                            Tuple.Create(11204, 4414, 13252, 6462));
        }

        public override int isCM(List<CombatItem> clist, int health)
        {
            return (health == 5551340) ? 1 : 0;
        }

        public override string getReplayIcon()
        {
            return "https://i.imgur.com/IOPAHRE.png";
        }
    }
}
