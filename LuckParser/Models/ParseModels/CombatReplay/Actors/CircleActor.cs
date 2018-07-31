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
        private bool growing;
        private int radius;
        private Tuple<int, int> lifespan;
        private int color;

        public CircleActor(bool fill, bool growing, int radius, Tuple<int,int> lifespan, int color)
        {
            this.fill = fill;
            this.growing = growing;
            this.radius = radius;
            this.lifespan = lifespan;
            this.color = color;
        }

        public bool isGrowing()
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

        public int getColor()
        {
            return color;
        }

        public abstract string getPosition();

    }
}
