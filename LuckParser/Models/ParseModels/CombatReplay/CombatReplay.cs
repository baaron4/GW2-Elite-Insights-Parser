using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class CombatReplay
    {
        public List<Point3D> Positions { get; private set; } = new List<Point3D>();
        public List<Point3D> Velocities { get; private set; } = new List<Point3D>();
        public List<int> Times => Positions.Select(x => (int)x.Time).ToList();
        public List<Point3D> Rotations { get; } = new List<Point3D>();
        private long _start = -1;
        private long _end = -1;
        public Tuple<long, long> TimeOffsets => new Tuple<long, long>(_start, _end);
        // icon
        public string Icon { get; set; }
        //status
        public List<Tuple<long, long>> Deads { get; } = new List<Tuple<long, long>>();
        public List<Tuple<long, long>> Downs { get; } = new List<Tuple<long, long>>();
        public List<Tuple<long, long>> DCs { get; } = new List<Tuple<long, long>>();
        // actors
        public List<GenericActor> Actors { get; } = new List<GenericActor>();
  
        public void Trim(long start, long end)
        {
            Positions.RemoveAll(x => x.Time < start || x.Time > end);
            _start = Math.Max(start,1);
            _end = Math.Max(_start,end);
        }
           
        public void PollingRate(int rate, long fightDuration, bool forceInterpolate)
        {
            if (forceInterpolate && Positions.Count == 0)
            {
                Positions.Add(new Point3D(short.MinValue, short.MinValue, 0, 0));
                Deads.Add(new Tuple<long, long>(0, fightDuration));
            }
            if (Positions.Count == 0)
            {
                return;
            }
            else if (Positions.Count == 1 && !forceInterpolate)
            {
                Velocities = null;
                return;
            }
            List<Point3D> interpolatedPositions = new List<Point3D>();
            int tablePos = 0;
            Point3D currentVelocity = null;
            for (int i = -50*rate; i < fightDuration; i += rate)
            {
                Point3D pt = Positions[tablePos];
                if (i <= pt.Time)
                {
                    currentVelocity = null;
                    interpolatedPositions.Add(new Point3D(pt.X, pt.Y, pt.Z, i));
                }
                else
                {
                    if (tablePos == Positions.Count - 1)
                    {
                        interpolatedPositions.Add(new Point3D(pt.X, pt.Y, pt.Z, i));
                    }
                    else
                    {
                        Point3D ptn = Positions[tablePos + 1];
                        if (ptn.Time < i)
                        {
                            tablePos++;
                            currentVelocity = null;
                            i -= rate;
                        }
                        else
                        {
                            Point3D last = interpolatedPositions.Last().Time > pt.Time ? interpolatedPositions.Last() : pt;
                            Point3D velocity = Velocities.Find(x => x.Time <= i && x.Time > last.Time);
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
            Positions = interpolatedPositions.Where(x => x.Time >= 0).ToList();
            Velocities = null;
        }

        public List<Point3D> GetActivePositions()
        {
            List<Point3D> activePositions = new List<Point3D>(Positions);
            for (var i = 0; i < activePositions.Count; i++)
            {
                Point3D cur = activePositions[i];
                foreach (Tuple<long, long> status in Deads)
                {
                    if (cur.Time >= status.Item1 && cur.Time <= status.Item2)
                    {
                        activePositions[i] = null;
                    }
                }
                foreach (Tuple<long, long> status in DCs)
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
