using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class FollowingCircle : CircleActor
    {

        public FollowingCircle(bool fill, int growing, int radius, Tuple<int, int> lifespan, string color) : base(fill, growing, radius, lifespan, color)
        {

        }

        public override string getPosition(string id, CombatReplayMap map)
        {
            return "'" + id + "'";
        }
    }
}
