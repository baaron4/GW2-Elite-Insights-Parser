using System;

namespace LuckParser.Models.ParseModels
{
    public class RectangleActor : Actor
    {
        public int Height { get; }
        public int Width { get; }

        public RectangleActor(bool fill, int growing, int width, int height, Tuple<int, int> lifespan, string color) : base(true, growing, lifespan, color, new MobileActor())
        {
            Height = height;
            Width = width;
        }

        public RectangleActor(bool fill, int growing, int width, int height, Tuple<int, int> lifespan, string color, Point3D position) : base(true, growing, lifespan, color, new ImmobileActor(position))
        {
            Height = height;
            Width = width;
        }
    }
}
