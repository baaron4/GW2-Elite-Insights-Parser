using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class Actor
    {
        private bool fill;
        private Tuple<int, int> lifespan;
        private string color;
        private int growing;

        public Actor(bool fill, int growing, Tuple<int, int> lifespan, string color)
        {
            this.lifespan = lifespan;
            this.color = color;
            this.fill = fill;
            this.growing = growing;
        }

        public int getGrowing()
        {
            return growing;
        }

        public bool isFilled()
        {
            return fill;
        }

        public Tuple<int, int> getLifespan()
        {
            return lifespan;
        }

        public string getColor()
        {
            return "'" + color + "'";
        }
    }
}
