using System;

namespace LuckParser.Models.ParseModels
{
    public abstract class Actor
    {
        public bool Filled { get; }
        public Tuple<int, int> Lifespan { get; }
        private readonly string _color;
        public string Color => "'" + _color + "'";
        public int Growing { get; }
        private readonly Mobility _mobility;

        protected Actor(bool fill, int growing, Tuple<int, int> lifespan, string color, Mobility mobility)
        {
            Lifespan = lifespan;
            _color = color;
            Filled = fill;
            _mobility = mobility;
            Growing = growing;
        }
       
        public string GetPosition(string id, CombatReplayMap map)
        {
            return _mobility.GetPosition(id, map);
        }
       
    }
}
