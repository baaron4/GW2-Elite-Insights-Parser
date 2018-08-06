using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class CircleActor
    {
        private bool fill;
        private int growing;
        private int radius;
        private Tuple<int, int> lifespan;
        private string color;

        public CircleActor(bool fill, int growing, int radius, Tuple<int,int> lifespan, string color)
        {
            this.fill = fill;
            this.growing = growing;
            this.radius = radius;
            this.lifespan = lifespan;
            this.color = color;
        }

        public int getGrowing()
        {
            return growing;
        }

        public bool isFilled()
        {
            return fill;
        }

        public int getRadius()
        {
            return radius;
        }

        public Tuple<int,int> getLifespan()
        {
            return lifespan;
        }

        public string getColor()
        {
            return "'" + color + "'";
        }

        public abstract string getPosition(string id, CombatReplayMap map);

    }
}
