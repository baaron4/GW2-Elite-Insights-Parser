using System;

namespace LuckParser.Models.ParseModels
{
    public abstract class GenericActor
    {    
        public Tuple<int, int> Lifespan { get; }
        protected Connector ConnectedTo;
        
        protected GenericActor(Tuple<int, int> lifespan, Connector connector)
        {
            Lifespan = lifespan;
            ConnectedTo = connector;
        }
        //
        protected class GenericSerializable
        {
            public string Type { get; set; }
            public long Start { get; set; }
            public long End { get; set; }
            public object ConnectedTo { get; set; }
        }

        public abstract string GetCombatReplayJSON(CombatReplayMap map);

    }
}
