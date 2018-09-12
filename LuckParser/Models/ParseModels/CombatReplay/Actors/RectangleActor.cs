using System;

namespace LuckParser.Models.ParseModels
{
    public class RectangleActor : Actor
    {
        private readonly int _height;
        private readonly int _width;

        public RectangleActor(bool fill, int growing, int height, int width, Tuple<int, int> lifespan, string color) : base(true, growing, lifespan, color, new MobileActor())
        {
            _height = height;
            _width = width;
        }

        public RectangleActor(bool fill, int growing, int height, int width, Tuple<int, int> lifespan, string color, Point3D position) : base(true, growing, lifespan, color, new ImmobileActor(position))
        {
            _height = height;
            _width = width;
        }

        public RectangleActor(bool fill, int growing, int height, int width, Tuple<int, int> lifespan, string color, string iconLink) : base(true, growing, lifespan, color, new MobileActor())
        {
            _height = height;
            _width = width;
        }

        public int GetHeight()
        {
            return _height;
        }

        public int GetWidth()
        {
            return _width;
        }

    }
}
