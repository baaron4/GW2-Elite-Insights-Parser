using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.EIData
{
    public class CombatReplayMap
    {

        public class MapItem
        {
            public string Link { get; set; }
            public long Start { get; set; }
            public long End { get; set; }
        }

        public List<MapItem> Maps { get; } = new List<MapItem>();
        private readonly (int width, int height) _size;
        private readonly (int topX, int topY, int bottomX, int bottomY) _rect;
        private readonly (int topX, int topY, int bottomX, int bottomY) _fullRect;
        private readonly (int bottomX, int bottomY, int topX, int topY) _worldRect;

        public CombatReplayMap(string link, (int width, int height) size, (int topX, int topY, int bottomX, int bottomY) rect, (int topX, int topY, int bottomX, int bottomY) fullRect, (int bottomX, int bottomY, int topX, int topY) worldRect)
        {
            Maps.Add(new MapItem()
            {
                Link = link,
                Start = -1,
                End = -1
            });
            _size = size;
            _rect = rect;
            _fullRect = fullRect;
            _worldRect = worldRect;
        }

        public (int width, int height) GetPixelMapSize()
        {
            double ratio = (double)_size.width / _size.height;
            const int pixelSize = 850;
            if (ratio > 1.0)
            {
                return (pixelSize, (int)Math.Round(pixelSize / ratio));
            }
            else if (ratio < 1.0)
            {
                return ((int)Math.Round(ratio * pixelSize), pixelSize);
            }
            else
            {
                return (pixelSize, pixelSize);
            }
        }

        public (double x, double y) GetMapCoord(float realX, float realY)
        {
            (int width, int height) = GetPixelMapSize();
            double scaleX = (double)width / _size.width;
            double scaleY = (double)height / _size.height;
            double x = (realX - _rect.topX) / (_rect.bottomX - _rect.topX);
            double y = (realY - _rect.topY) / (_rect.bottomY - _rect.topY);
            return (Math.Round(scaleX * _size.width * x, 2), Math.Round(scaleY * (_size.height - _size.height * y), 2));
        }

        public void MatchMapsToPhases(List<string> urls, List<PhaseData> phases, long fightEnd)
        {
            if (phases.Count - 1 > urls.Count)
            {
                return;
            }
            MapItem originalMap = Maps[0];
            originalMap.Start = 0;
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                Maps.Last().End = phase.Start;
                Maps.Add(new MapItem()
                {
                    Link = urls[i - 1],
                    Start = phase.Start
                });
            }
            Maps.Last().End = fightEnd;
            Maps.RemoveAll(x => x.End - x.Start <= 0);
        }

        public float GetInch()
        {
            float ratio = (float)(_rect.bottomX - _rect.topX) / GetPixelMapSize().width;
            return (float)Math.Round(1.0f / ratio, 3);
        }

    }
}
