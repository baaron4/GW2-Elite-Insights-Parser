using System;

namespace LuckParser.Models.ParseModels
{
    public class CircleActor : Actor
    {
        public int Radius { get; }

        public CircleActor(bool fill, int growing, int radius, Tuple<int, int> lifespan, string color) : base(fill, growing, lifespan, color, new MobileActor())
        {
            Radius = radius;
        }

        public CircleActor(bool fill, int growing, int radius, Tuple<int, int> lifespan, string color, Point3D position) : base(fill, growing, lifespan, color, new ImmobileActor(position))
        {
            Radius = radius;
        }

    }
}
