using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class CircleActor : Actor
    {
        private int radius;

        public CircleActor(bool fill, int growing, int radius, Tuple<int,int> lifespan, string color) : base(fill, growing,lifespan, color)
        {
            this.radius = radius;
        }

        public int getRadius()
        {
            return radius;
        }

        public abstract string getPosition(string id, CombatReplayMap map);

    }
}
