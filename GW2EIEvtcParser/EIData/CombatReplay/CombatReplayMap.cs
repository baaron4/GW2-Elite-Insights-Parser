using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    public class CombatReplayMap
    {

        public class MapItem
        {
            public string Link { get; internal set; }
            public long Start { get; internal set; }
            public long End { get; internal set; }
        }

        public List<MapItem> Maps { get; } = new List<MapItem>();
        private (int width, int height) _urlPixelSize;
        private (int topX, int topY, int bottomX, int bottomY) _rectInMap;
        //private (int topX, int topY, int bottomX, int bottomY) _fullRect;
        //private (int bottomX, int bottomY, int topX, int topY) _worldRect;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="link">Url to the image</param>
        /// <param name="urlPixelSize">Width and Height of the image in pixel</param>
        /// <param name="rectInMap">The map rectangle region corresponding to the image in map coordinates</param>
        internal CombatReplayMap(string link, (int width, int height) urlPixelSize, (int topX, int topY, int bottomX, int bottomY) rectInMap)
        {
            Maps.Add(new MapItem()
            {
                Link = link,
                Start = -1,
                End = -1
            });
            _urlPixelSize = urlPixelSize;
            _rectInMap = rectInMap;
        }

        /*/internal CombatReplayMap(string link, (int width, int height) size, (int topX, int topY, int bottomX, int bottomY) rect, (int topX, int topY, int bottomX, int bottomY) fullRect, (int bottomX, int bottomY, int topX, int topY) worldRect)
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
        }*/

        public (int width, int height) GetPixelMapSize()
        {
            double ratio = (double)_urlPixelSize.width / _urlPixelSize.height;
            const int pixelSize = 750;
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

        internal void ComputeBoundingBox(ParsedEvtcLog log)
        {
            if (log.CanCombatReplay && _rectInMap.topX == _rectInMap.bottomX)
            {
                _rectInMap.topX = int.MaxValue;
                _rectInMap.topY = int.MaxValue;
                _rectInMap.bottomX = int.MinValue;
                _rectInMap.bottomY = int.MinValue;
                foreach (Player p in log.PlayerList)
                {
                    List<Point3D> pos = p.GetCombatReplayPolledPositions(log);
                    if (pos.Count == 0)
                    {
                        continue;
                    }
                    _rectInMap.topX = Math.Min((int)Math.Floor(pos.Min(x => x.X)) - 500, _rectInMap.topX);
                    _rectInMap.topY = Math.Min((int)Math.Floor(pos.Min(x => x.Y)) - 500, _rectInMap.topY);
                    _rectInMap.bottomX = Math.Max((int)Math.Floor(pos.Max(x => x.X)) + 500, _rectInMap.bottomX);
                    _rectInMap.bottomY = Math.Max((int)Math.Floor(pos.Max(x => x.Y)) + 500, _rectInMap.bottomY);
                }
            }
        }

        internal (double x, double y) GetMapCoord(float realX, float realY)
        {
            (int width, int height) = GetPixelMapSize();
            double scaleX = (double)width / _urlPixelSize.width;
            double scaleY = (double)height / _urlPixelSize.height;
            double x = (realX - _rectInMap.topX) / (_rectInMap.bottomX - _rectInMap.topX);
            double y = (realY - _rectInMap.topY) / (_rectInMap.bottomY - _rectInMap.topY);
            return (Math.Round(scaleX * _urlPixelSize.width * x, 2), Math.Round(scaleY * (_urlPixelSize.height - _urlPixelSize.height * y), 2));
        }

        /// <summary>
        /// This assumes that all urls are of the same size (or at least size ratio) and that they have the same map rectangle
        /// </summary>
        /// <param name="urls"></param>
        /// <param name="phases"></param>
        /// <param name="fightEnd"></param>
        internal void MatchMapsToPhases(List<string> urls, List<PhaseData> phases, long fightEnd)
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
            float ratio = (float)(_rectInMap.bottomX - _rectInMap.topX) / GetPixelMapSize().width;
            return (float)Math.Round(1.0f / ratio, 3);
        }

    }
}
