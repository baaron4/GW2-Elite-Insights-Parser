using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class FollowingCircle : CircleActor
    {

        public FollowingCircle(bool fill, bool growing, int radius, Tuple<int, int> lifespan, int color) : base(fill, growing, radius, lifespan, color)
        {

        }

        public override string getPosition(string id, CombatReplayMap map)
        {
            return id;
        }
    }
}
