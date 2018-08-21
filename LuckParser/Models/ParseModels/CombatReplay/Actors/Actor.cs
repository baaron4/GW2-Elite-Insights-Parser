using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class Actor
    {
        private bool fill;
        private Tuple<int, int> lifespan;
        private string color;
        private int growing;
        private Mobility mobility;

        public Actor(bool fill, int growing, Tuple<int, int> lifespan, string color, Mobility mobility)
        {
            this.lifespan = lifespan;
            this.color = color;
            this.fill = fill;
            this.mobility = mobility;
            this.growing = growing;
        }

        public int GetGrowing()
        {
            return growing;
        }

        public bool IsFilled()
        {
            return fill;
        }

        public string GetPosition(string id, CombatReplayMap map)
        {
            return mobility.GetPosition(id, map);
        }

        public Tuple<int, int> GetLifespan()
        {
            return lifespan;
        }

        public string GetColor()
        {
            return "'" + color + "'";
        }
    }
}
