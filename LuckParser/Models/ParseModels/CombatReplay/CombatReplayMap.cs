using System;

namespace LuckParser.Models.ParseModels
{
    public class CombatReplayMap
    {
        public string Link { get; }
        private readonly Tuple<int, int> _size;
        private readonly Tuple<int, int, int, int> _rect;
        private readonly Tuple<int, int, int, int> _fullRect;
        private readonly Tuple<int, int, int, int> _worldRect;

        public CombatReplayMap(string link, Tuple<int, int> size, Tuple<int, int, int, int> rect, Tuple<int, int, int, int> fullRect, Tuple<int, int, int, int> worldRect)
        {
            Link = link;
            _size = size;
            _rect = rect;
            _fullRect = fullRect;
            _worldRect = worldRect;
        }

        public Tuple<int, int> GetPixelMapSize()
        {
            double ratio = (double)_size.Item1 / _size.Item2;
            const int pixelSize = 850;
            if (ratio > 1.0)
            {
                return new Tuple<int, int>(pixelSize, (int)Math.Round(pixelSize / ratio));
            }
            else if (ratio < 1.0)
            {
                return new Tuple<int, int>((int)Math.Round(ratio * pixelSize), pixelSize);
            }
            else
            {
                return new Tuple<int, int>(pixelSize, pixelSize);
            }
        }

        public Tuple<double, double> GetMapCoord(float realX, float realY)
        {
            //Tuple<int, int, int, int> apiRect = getMapApiRect(log);
            Tuple<int, int> pixelSizes = GetPixelMapSize();
            double scaleX = (double)pixelSizes.Item1 / _size.Item1;
            double scaleY = (double)pixelSizes.Item2 / _size.Item2;
            double x = (realX - _rect.Item1) / (_rect.Item3 - _rect.Item1);
            double y = (realY - _rect.Item2) / (_rect.Item4 - _rect.Item2);
            return Tuple.Create(Math.Round(scaleX * _size.Item1 * x,2), Math.Round(scaleY * (_size.Item2 - _size.Item2 * y),2));
        }

        public float GetInch()
        {
            float ratio = (float)(_rect.Item3 - _rect.Item1) / GetPixelMapSize().Item1 ;
            return (float)Math.Round(1.0f/ratio,3);
        }

    }
}
