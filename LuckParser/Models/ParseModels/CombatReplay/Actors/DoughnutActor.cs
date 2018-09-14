using System;

namespace LuckParser.Models.ParseModels
{
    public class DoughnutActor : Actor
    {
        public int OuterRadius { get; }
        public int InnerRadius { get; }

        public DoughnutActor(int growing, int innerRadius, int outerRadius, Tuple<int, int> lifespan, string color) : base(true, growing, lifespan, color, new MobileActor())
        {
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
        }

        public DoughnutActor(int growing, int innerRadius, int outerRadius, Tuple<int, int> lifespan, string color, Point3D position) : base(true, growing, lifespan, color, new ImmobileActor(position))
        {
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
        }
        
    }
}
