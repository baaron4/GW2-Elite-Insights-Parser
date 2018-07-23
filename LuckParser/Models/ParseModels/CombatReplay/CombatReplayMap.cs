using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class CombatReplayMap
    {
        private string link;
        private Tuple<int, int> size;
        private Tuple<int, int, int, int> rect;
        private Tuple<int, int, int, int> fullRect;
        private Tuple<int, int, int, int> worldRect;

        public CombatReplayMap(string link, Tuple<int, int> size, Tuple<int, int, int, int> rect, Tuple<int, int, int, int> fullRect, Tuple<int, int, int, int> worldRect)
        {
            this.link = link;
            this.size = size;
            this.rect = rect;
            this.fullRect = fullRect;
            this.worldRect = worldRect;
        }

        public string getLink()
        {
            return link;
        }

        public Tuple<int, int> getPixelMapSize()
        {
            double ratio = (double)size.Item1 / size.Item2;
            if (ratio > 1.0)
            {
                return new Tuple<int, int>(850, (int)Math.Round(850 / ratio));
            }
            else if (ratio < 1.0)
            {
                return new Tuple<int, int>((int)Math.Round(ratio * 850), 850);
            }
            else
            {
                return new Tuple<int, int>(850, 850);
            }
        }

        public Tuple<int, int> getMapCoord(float realX, float realY)
        {
            //Tuple<int, int, int, int> apiRect = getMapApiRect(log);
            Tuple<int, int> pixelSizes = getPixelMapSize();
            float scaleX = (float)pixelSizes.Item1 / size.Item1;
            float scaleY = (float)pixelSizes.Item2 / size.Item2;
            float x = (Math.Max(Math.Min(realX, rect.Item3), rect.Item1) - rect.Item1) / (rect.Item3 - rect.Item1);
            float y = (Math.Max(Math.Min(realY, rect.Item4), rect.Item2) - rect.Item2) / (rect.Item4 - rect.Item2);
            return Tuple.Create((int)Math.Round(scaleX * size.Item1 * x), (int)Math.Round(scaleY * (size.Item2 - size.Item2 * y)));
        }

        public int getInch()
        {
            return (int)Math.Round(24 * (double)(fullRect.Item3 - fullRect.Item1) / (worldRect.Item3 - worldRect.Item1));
        }

    }
}
