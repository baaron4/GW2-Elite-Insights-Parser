using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class CombatReplay
    {
        private List<Point3D> _positions = new List<Point3D>();
        private List<Point3D> _velocities = new List<Point3D>();
        private long _start;
        private long _end;
        // icon
        private string _icon;
        //status
        private List<Tuple<long, long>> _dead = new List<Tuple<long, long>>();
        private List<Tuple<long, long>> _down = new List<Tuple<long, long>>();
        private List<Tuple<long, long>> _dc = new List<Tuple<long, long>>();
        // dps
        private List<int> _dps = new List<int>();
        private List<int> _dps10s = new List<int>();
        private List<int> _dps30s = new List<int>();
        // boons
        private readonly Dictionary<long, List<int>> _boons = new Dictionary<long, List<int>>();
        // actors
        private readonly List<CircleActor> _circleActors = new List<CircleActor>();
        private readonly List<DoughnutActor> _doughnutActors = new List<DoughnutActor>();
        private readonly List<RectangleActor> _rectangleActors = new List<RectangleActor>();

        public void SetIcon(string icon)
        {
            _icon = icon;
        }

        public void AddPosition(Point3D pos)
        {
            _positions.Add(pos);
        }

        public void AddVelocity(Point3D vel)
        {
            _velocities.Add(vel);
        }

        public Tuple<long, long> GetTimeOffsets()
        {
            return new Tuple<long, long>(_start, _end);
        }

        public void AddDPS(int dps)
        {
            _dps.Add(dps);
        }
        public void AddDPS10s(int dps)
        {
            _dps10s.Add(dps);
        }
        public void AddDPS30s(int dps)
        {
            _dps30s.Add(dps);
        }
        public void AddCircleActor(CircleActor circleActor)
        {
            _circleActors.Add(circleActor);
        }
        public void AddDoughnutActor(DoughnutActor doughnutActor)
        {
            _doughnutActors.Add(doughnutActor);
        }
        public void AddRectangleActor(RectangleActor rectangleActor)
        {
            _rectangleActors.Add(rectangleActor);
        }

        public void SetStatus(List<Tuple<long, long>> down, List<Tuple<long, long>> dead, List<Tuple<long, long>> dc)
        {
            _down = down;
            _dead = dead;
            _dc = dc;
        }

        public void Trim(long start, long end)
        {
            _start = start;
            _end = end;
            _positions.RemoveAll(x => x.Time < start || x.Time > end);
            if (_positions.Count == 0)
            {
                _start = -1;
                _end = -1;
            }
        }

        public List<CircleActor> GetCircleActors()
        {
            return _circleActors;
        }

        public List<DoughnutActor> GetDoughnutActors()
        {
            return _doughnutActors;
        }

        public List<RectangleActor> GetRectangleActors()
        {
            return _rectangleActors;
        }

        public List<Tuple<long, long>> GetDead()
        {
            return _dead;
        }

        public List<Tuple<long, long>> GetDown()
        {
            return _down;
        }

        public List<Tuple<long, long>> GetDC()
        {
            return _dc;
        }

        public void AddBoon(long id, int value)
        {
            if (!_boons.TryGetValue(id, out List<int> ll))
            {
                ll = new List<int>();
                _boons.Add(id, ll);
            }
            ll.Add(value);
        }

        public List<int> GetTimes()
        {
            return _positions.Select(x => (int)x.Time).ToList();
        }

        public string GetIcon()
        {
            return _icon;
        }

        public void PollingRate(int rate, long fightDuration, bool forceInterpolate)
        {
            if (_positions.Count == 0)
            {
                _start = -1;
                _end = -1;
                return;
            }
            else if (_positions.Count == 1 && !forceInterpolate)
            {
                _velocities = null;
                return;
            }
            List<Point3D> interpolatedPositions = new List<Point3D>();
            int tablePos = 0;
            Point3D currentVelocity = null;
            for (int i = -1000; i < fightDuration; i += rate)
            {
                Point3D pt = _positions[tablePos];
                if (i <= pt.Time)
                {
                    currentVelocity = null;
                    interpolatedPositions.Add(new Point3D(pt.X, pt.Y, pt.Z, i));
                }
                else
                {
                    if (tablePos == _positions.Count - 1)
                    {
                        interpolatedPositions.Add(new Point3D(pt.X, pt.Y, pt.Z, i));
                    }
                    else
                    {
                        Point3D ptn = _positions[tablePos + 1];
                        if (ptn.Time < i)
                        {
                            tablePos++;
                            currentVelocity = null;
                            interpolatedPositions.Add(new Point3D(ptn.X, ptn.Y, ptn.Z, i));
                        }
                        else
                        {
                            Point3D last = interpolatedPositions.Last();
                            Point3D velocity = _velocities.Find(x => x.Time <= i && x.Time > last.Time);
                            currentVelocity = velocity ?? currentVelocity;
                            if (ptn.Time - pt.Time < 400)
                            {
                                float ratio = (float)(i - pt.Time) / (ptn.Time - pt.Time);
                                interpolatedPositions.Add(new Point3D(pt, ptn, ratio, i));
                            }
                            else
                            {
                                if (currentVelocity == null || (Math.Abs(currentVelocity.X) <= 1e-1 && Math.Abs(currentVelocity.Y) <= 1e-1))
                                {
                                    interpolatedPositions.Add(new Point3D(last.X, last.Y, last.Z, i));
                                }
                                else
                                {
                                    float ratio = (float)(i - last.Time) / (ptn.Time - last.Time);
                                    interpolatedPositions.Add(new Point3D(last, ptn, ratio, i));
                                }
                            }

                        }
                    }
                }
            }
            _positions = interpolatedPositions.Where(x => x.Time >= 0).ToList();
            _velocities = null;
        }

        public List<Point3D> GetPositions()
        {
            return _positions;
        }

        public List<Point3D> GetActivePositions()
        {
            List<Point3D> activePositions = new List<Point3D>(_positions);
            for (var i = 0; i < activePositions.Count; i++)
            {
                Point3D cur = activePositions[i];
                foreach (Tuple<long, long> status in _dead)
                {
                    if (cur.Time >= status.Item1 && cur.Time <= status.Item2)
                    {
                        activePositions[i] = null;
                    }
                }
                foreach (Tuple<long, long> status in _dc)
                {
                    if (cur.Time >= status.Item1 && cur.Time <= status.Item2)
                    {
                        activePositions[i] = null;
                    }
                }
            }        
            return activePositions;
        }
    }
}
