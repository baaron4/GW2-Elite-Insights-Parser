using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIUtils;

namespace GW2EIEvtcParser.EIData
{
    public class CombatReplay
    {
        internal List<Point3D> Positions { get; } = new List<Point3D>();
        internal List<Point3D> PolledPositions { get; private set; } = new List<Point3D>();
        internal List<Point3D> Velocities { get; private set; } = new List<Point3D>();
        internal List<int> Times => PolledPositions.Select(x => (int)x.Time).ToList();
        internal List<Point3D> Rotations { get; } = new List<Point3D>();
        internal List<Point3D> PolledRotations { get; private set; } = new List<Point3D>();
        private long _start = -1;
        private long _end = -1;
        internal (long start, long end) TimeOffsets => (_start, _end);
        // actors
        internal bool NoActors { get; set; } = true;
        public List<GenericDecoration> Decorations { get; } = new List<GenericDecoration>();

        internal void Trim(long start, long end)
        {
            PolledPositions.RemoveAll(x => x.Time < start || x.Time > end);
            PolledRotations.RemoveAll(x => x.Time < start || x.Time > end);
            _start = Math.Max(start, 1);
            _end = Math.Max(_start, end);
        }

        private void PositionPolling(int rate, long fightDuration, bool forceInterpolate)
        {
            if (forceInterpolate && Positions.Count == 0)
            {
                Positions.Add(new Point3D(int.MinValue, int.MinValue, 0, 0));
            }
            if (Positions.Count == 0)
            {
                return;
            }
            else if (Positions.Count == 1 && !forceInterpolate)
            {
                PolledPositions.Add(Positions[0]);
                return;
            }
            int tablePos = 0;
            Point3D currentVelocity = null;
            for (int i = (int)Math.Min(0, rate * ((Positions[0].Time / rate) - 1)); i < fightDuration; i += rate)
            {
                Point3D pt = Positions[tablePos];
                if (i <= pt.Time)
                {
                    currentVelocity = null;
                    PolledPositions.Add(new Point3D(pt.X, pt.Y, pt.Z, i));
                }
                else
                {
                    if (tablePos == Positions.Count - 1)
                    {
                        PolledPositions.Add(new Point3D(pt.X, pt.Y, pt.Z, i));
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
                            Point3D last = PolledPositions.Last().Time > pt.Time ? PolledPositions.Last() : pt;
                            Point3D velocity = Velocities.Find(x => x.Time <= i && x.Time > last.Time);
                            currentVelocity = velocity ?? currentVelocity;
                            if (ptn.Time - pt.Time < 400)
                            {
                                float ratio = (float)(i - pt.Time) / (ptn.Time - pt.Time);
                                PolledPositions.Add(new Point3D(pt, ptn, ratio, i));
                            }
                            else
                            {
                                if (currentVelocity == null || (Math.Abs(currentVelocity.X) <= 1e-1 && Math.Abs(currentVelocity.Y) <= 1e-1))
                                {
                                    PolledPositions.Add(new Point3D(last.X, last.Y, last.Z, i));
                                }
                                else
                                {
                                    float ratio = (float)(i - last.Time) / (ptn.Time - last.Time);
                                    PolledPositions.Add(new Point3D(last, ptn, ratio, i));
                                }
                            }

                        }
                    }
                }
            }
            PolledPositions = PolledPositions.Where(x => x.Time >= 0).ToList();
        }
        /// <summary>
        /// The method exists only to have the same amount of rotation as positions, it's easier to do it here than
        /// in javascript
        /// </summary>
        /// <param name="rate"></param>
        /// <param name="fightDuration"></param>
        /// <param name="forceInterpolate"></param>
        private void RotationPolling(int rate, long fightDuration, bool forceInterpolate)
        {
            if (Rotations.Count == 0)
            {
                return;
            }
            else if (Rotations.Count == 1 && !forceInterpolate)
            {
                PolledRotations.Add(Rotations[0]);
                return;
            }
            int tablePos = 0;
            for (int i = (int)Math.Min(0, rate * ((Rotations[0].Time / rate) - 1)); i < fightDuration; i += rate)
            {
                Point3D pt = Rotations[tablePos];
                if (i <= pt.Time)
                {
                    PolledRotations.Add(new Point3D(pt.X, pt.Y, pt.Z, i));
                }
                else
                {
                    if (tablePos == Rotations.Count - 1)
                    {
                        PolledRotations.Add(new Point3D(pt.X, pt.Y, pt.Z, i));
                    }
                    else
                    {
                        Point3D ptn = Rotations[tablePos + 1];
                        if (ptn.Time < i)
                        {
                            tablePos++;
                            i -= rate;
                        }
                        else
                        {
                            PolledRotations.Add(new Point3D(pt.X, pt.Y, pt.Z, i));
                        }
                    }
                }
            }
            PolledRotations = PolledRotations.Where(x => x.Time >= 0).ToList();
        }

        internal void PollingRate(long fightDuration, bool forceInterpolate)
        {
            PositionPolling(ParseHelper._pollingRate, fightDuration, forceInterpolate);
            RotationPolling(ParseHelper._pollingRate, fightDuration, forceInterpolate);
        }

        internal List<Point3D> GetActivePositions(List<(long start, long end)> deads, List<(long start, long end)> dcs)
        {
            var activePositions = new List<Point3D>(PolledPositions);
            for (int i = 0; i < activePositions.Count; i++)
            {
                Point3D cur = activePositions[i];
                foreach ((long start, long end) in deads)
                {
                    if (cur.Time >= start && cur.Time <= end)
                    {
                        activePositions[i] = null;
                    }
                }
                foreach ((long start, long end) in dcs)
                {
                    if (cur.Time >= start && cur.Time <= end)
                    {
                        activePositions[i] = null;
                    }
                }
            }
            return activePositions;
        }
    }
}

