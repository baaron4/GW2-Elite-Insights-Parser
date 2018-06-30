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
        private HashSet<int> times = new HashSet<int>();
        // dps
        private List<int> dps = new List<int>();
        private List<int> dps10s = new List<int>();
        private List<int> dps30s = new List<int>();
        // boons
        private Dictionary<int, List<int>> boons = new Dictionary<int, List<int>>();

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

        public void addBoon(int id, int value)
        {
            List<int> ll;
            if (!boons.TryGetValue(id,out ll))
            {
                ll = new List<int>();
                boons.Add(id, ll);
            }
            ll.Add(value);
        }

        public HashSet<int> getTimes()
        {
            return times;
        }
    }
}
