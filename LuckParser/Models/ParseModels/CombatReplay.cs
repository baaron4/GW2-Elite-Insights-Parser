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
        private List<int> times = new List<int>();
        // dps
        private List<int> dps = new List<int>();
        private List<int> dps10s = new List<int>();
        private List<int> dps30s = new List<int>();

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

        public void addTime(int time)
        {
            this.times.Add(time);
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

        public List<int> getTimes()
        {
            return times;
        }
    }
}
