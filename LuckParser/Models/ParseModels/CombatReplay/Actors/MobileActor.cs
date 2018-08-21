using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class MobileActor : Mobility
    {

        public MobileActor() : base()
        {
        }

        public override string GetPosition(string id, CombatReplayMap map)
        {
            return "'" + id + "'";
        }
    }
}
