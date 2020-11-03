﻿using System;
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
        private (int width, int height) _size;
        private (int topX, int topY, int bottomX, int bottomY) _rect;
        private (int topX, int topY, int bottomX, int bottomY) _fullRect;
        private (int bottomX, int bottomY, int topX, int topY) _worldRect;

        internal CombatReplayMap(string link, (int width, int height) size, (int topX, int topY, int bottomX, int bottomY) rect, (int topX, int topY, int bottomX, int bottomY) fullRect, (int bottomX, int bottomY, int topX, int topY) worldRect)
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
            if (log.CanCombatReplay && _rect.topX == _rect.bottomX)
            {
                _rect.topX = int.MaxValue;
                _rect.topY = int.MaxValue;
                _rect.bottomX = int.MinValue;
                _rect.bottomY = int.MinValue;
                foreach (Player p in log.PlayerList)
                {
                    List<Point3D> pos = p.GetCombatReplayPolledPositions(log);
                    if (pos.Count == 0)
                    {
                        continue;
                    }
                    _rect.topX = Math.Min((int)Math.Floor(pos.Min(x => x.X)) - 500, _rect.topX);
                    _rect.topY = Math.Min((int)Math.Floor(pos.Min(x => x.Y)) - 500, _rect.topY);
                    _rect.bottomX = Math.Max((int)Math.Floor(pos.Max(x => x.X)) + 500, _rect.bottomX);
                    _rect.bottomY = Math.Max((int)Math.Floor(pos.Max(x => x.Y)) + 500, _rect.bottomY);
                }
            }
        }

        internal (double x, double y) GetMapCoord(float realX, float realY)
        {
            (int width, int height) = GetPixelMapSize();
            double scaleX = (double)width / _size.width;
            double scaleY = (double)height / _size.height;
            double x = (realX - _rect.topX) / (_rect.bottomX - _rect.topX);
            double y = (realY - _rect.topY) / (_rect.bottomY - _rect.topY);
            return (Math.Round(scaleX * _size.width * x, 2), Math.Round(scaleY * (_size.height - _size.height * y), 2));
        }

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
            float ratio = (float)(_rect.bottomX - _rect.topX) / GetPixelMapSize().width;
            return (float)Math.Round(1.0f / ratio, 3);
        }

    }
}
