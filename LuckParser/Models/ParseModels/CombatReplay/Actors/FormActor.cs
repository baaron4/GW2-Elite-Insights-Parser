using System;

namespace LuckParser.Models.ParseModels
{
    public abstract class FormActor : GenericActor
    {

        public bool Filled { get; }
        public string Color { get; }
        public int Growing { get; }
        
        protected FormActor(bool fill, int growing, (int start, int end) lifespan, string color, Connector connector) : base(lifespan, connector)
        {
            Color = color;
            Filled = fill;
            Growing = growing;
        }
        //
        protected class FormSerializable : GenericActorSerializable
        {
            public bool Fill { get; set; }
            public int Growing { get; set; }
            public string Color { get; set; }
        }
        
    }
}
