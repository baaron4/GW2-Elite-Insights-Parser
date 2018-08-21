using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class CombatReplayMap
    {
        private string _link;
        private Tuple<int, int> _size;
        private Tuple<int, int, int, int> _rect;
        private Tuple<int, int, int, int> _fullRect;
        private Tuple<int, int, int, int> _worldRect;

        public CombatReplayMap(string link, Tuple<int, int> size, Tuple<int, int, int, int> rect, Tuple<int, int, int, int> fullRect, Tuple<int, int, int, int> worldRect)
        {
            _link = link;
            _size = size;
            _rect = rect;
            _fullRect = fullRect;
            _worldRect = worldRect;
        }

        public string GetLink()
        {
            return _link;
        }

        public Tuple<int, int> GetPixelMapSize()
        {
            double ratio = (double)_size.Item1 / _size.Item2;
            const int pixelSize = 900;
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

        public Tuple<int, int> GetMapCoord(float realX, float realY)
        {
            //Tuple<int, int, int, int> apiRect = getMapApiRect(log);
            Tuple<int, int> pixelSizes = GetPixelMapSize();
            float scaleX = (float)pixelSizes.Item1 / _size.Item1;
            float scaleY = (float)pixelSizes.Item2 / _size.Item2;
            float x = (Math.Max(Math.Min(realX, _rect.Item3), _rect.Item1) - _rect.Item1) / (_rect.Item3 - _rect.Item1);
            float y = (Math.Max(Math.Min(realY, _rect.Item4), _rect.Item2) - _rect.Item2) / (_rect.Item4 - _rect.Item2);
            return Tuple.Create((int)Math.Round(scaleX * _size.Item1 * x), (int)Math.Round(scaleY * (_size.Item2 - _size.Item2 * y)));
        }

        public float GetInch()
        {
            float ratio = (float)(_rect.Item3 - _rect.Item1) / GetPixelMapSize().Item1 ;
            return (float)Math.Round(1.0f/ratio,3);
        }

    }
}
