using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class DoughnutActor : Actor
    {
        private int outerRadius;
        private int innerRadius;

        public DoughnutActor(int growing, int innerRadius, int outerRadius, Tuple<int, int> lifespan, string color) : base(true, growing, lifespan, color, new MobileActor())
        {
            this.innerRadius = innerRadius;
            this.outerRadius = outerRadius;
        }

        public DoughnutActor(int growing, int innerRadius, int outerRadius, Tuple<int, int> lifespan, string color, Point3D position) : base(true, growing, lifespan, color, new ImmobileActor(position))
        {
            this.innerRadius = innerRadius;
            this.outerRadius = outerRadius;
        }

        public int getInnerRadius()
        {
            return innerRadius;
        }

        public int getOuterRadius()
        {
            return outerRadius;
        }

    }
}
