using System;

namespace LuckParser.Models.ParseModels
{
    public abstract class GenericActor
    {    
        public (int start, int end) Lifespan { get; }
        protected Connector ConnectedTo;
        
        protected GenericActor((int start, int end) lifespan, Connector connector)
        {
            Lifespan = lifespan;
            ConnectedTo = connector;
        }
        //
        public class GenericActorSerializable
        {
            public string Type { get; set; }
            public long Start { get; set; }
            public long End { get; set; }
            public object ConnectedTo { get; set; }
        }

        public abstract GenericActorSerializable GetCombatReplayJSON(CombatReplayMap map);

    }
}
