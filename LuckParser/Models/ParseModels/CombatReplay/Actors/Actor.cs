using System;

namespace LuckParser.Models.ParseModels
{
    public abstract class Actor
    {
        private readonly bool _fill;
        private readonly Tuple<int, int> _lifespan;
        private readonly string _color;
        private readonly int _growing;
        private readonly Mobility _mobility;

        protected Actor(bool fill, int growing, Tuple<int, int> lifespan, string color, Mobility mobility)
        {
            _lifespan = lifespan;
            _color = color;
            _fill = fill;
            _mobility = mobility;
            _growing = growing;
        }

        public int GetGrowing()
        {
            return _growing;
        }

        public bool IsFilled()
        {
            return _fill;
        }

        public string GetPosition(string id, CombatReplayMap map)
        {
            return _mobility.GetPosition(id, map);
        }

        public Tuple<int, int> GetLifespan()
        {
            return _lifespan;
        }

        public string GetColor()
        {
            return "'" + _color + "'";
        }
    }
}
