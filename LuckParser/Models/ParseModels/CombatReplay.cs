using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class CombatReplay
    {
        private List<Point3D> positions = new List<Point3D>();
        private List<Point3D> velocities = new List<Point3D>();
        // dps
        private List<int> dps = new List<int>();
        private List<int> dps10s = new List<int>();
        private List<int> dps30s = new List<int>();
        // boons
        private Dictionary<long, List<int>> boons = new Dictionary<long, List<int>>();

        public CombatReplay()
        {

        }

        public void addPosition(Point3D pos)
        {
            this.positions.Add(pos);
        }

        public void addVelocity(Point3D vel)
        {
            this.velocities.Add(vel);
        }
        

        public void addDPS(int dps)
        {
            this.dps.Add(dps);
        }
        public void addDPS10s(int dps)
        {
            this.dps10s.Add(dps);
        }
        public void addDPS30s(int dps)
        {
            this.dps30s.Add(dps);
        }

        public void addBoon(long id, int value)
        {
            List<int> ll;
            if (!boons.TryGetValue(id,out ll))
            {
                ll = new List<int>();
                boons.Add(id, ll);
            }
            ll.Add(value);
        }

        public List<int> getTimes()
        {
            return positions.Select(x => (int)x.time).ToList();
        }


        public void pollingRate(int rate, long fightDuration)
        {
            List<Point3D> interpolatedPositions = new List<Point3D>();
            int tablePos = 0;
            Point3D currentVelocity = null;
            for (int i = 0; i < fightDuration; i += rate)
            {
                Point3D pt = positions[tablePos];
                if (i <= pt.time)
                {
                    interpolatedPositions.Add(new Point3D(pt.X, pt.Y, pt.Z, i));
                }
                else
                {
                    if (tablePos == positions.Count - 1)
                    {
                        interpolatedPositions.Add(new Point3D(pt.X, pt.Y, pt.Z, i));
                    }
                    else
                    {
                        Point3D ptn = positions[tablePos + 1];
                        if (ptn.time < i)
                        {
                            tablePos++;
                            interpolatedPositions.Add(new Point3D(ptn.X, ptn.Y, ptn.Z, i));
                        }
                        else
                        {
                            Point3D last = interpolatedPositions.Last();
                            Point3D velocity = velocities.Find(x => x.time <= i && x.time > last.time);
                            currentVelocity = velocity != null ? velocity : currentVelocity;
                            if (ptn.time - pt.time < 400 || currentVelocity == null)
                            {
                                float ratio = (float)(i - pt.time) / (ptn.time - pt.time);
                                interpolatedPositions.Add(new Point3D(pt, ptn, ratio, i));
                            }
                            else
                            {
                                if (currentVelocity.X == 0 && currentVelocity.Y == 0)
                                {
                                    interpolatedPositions.Add(new Point3D(last.X, last.Y, last.Z, i));
                                }
                                else
                                {
                                    float ratio = (float)(i - last.time) / (ptn.time - last.time);
                                    interpolatedPositions.Add(new Point3D(last, ptn, ratio, i));
                                }
                            }

                        }
                    }
                }
            }
            positions = interpolatedPositions;
            velocities = null;
        }

        public List<Point3D> getPositions()
        {
            return positions;
        }
    }
}
