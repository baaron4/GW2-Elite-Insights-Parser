using System;

namespace LuckParser.Models.ParseModels
{
    public abstract class Actor
    {

        public bool Filled { get; }
        public Tuple<int, int> Lifespan { get; }
        public string Color { get; }
        public int Growing { get; }
        protected Connector ConnectedTo;
        
        protected Actor(bool fill, int growing, Tuple<int, int> lifespan, string color, Connector connector)
        {
            Lifespan = lifespan;
            Color = color;
            Filled = fill;
            Growing = growing;
            ConnectedTo = connector;
        }
        //
        protected class Serializable
        {

            public bool Fill { get; set; }
            public int Growing { get; set; }
            public string Color { get; set; }
            public string Type { get; set; }
            public long Start { get; set; }
            public long End { get; set; }
            public Object ConnectedTo { get; set; }
        }

        public abstract string GetCombatReplayJSON(CombatReplayMap map);

    }
}
