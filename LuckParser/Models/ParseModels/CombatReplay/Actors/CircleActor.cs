using Newtonsoft.Json;
using System;

namespace LuckParser.Models.ParseModels
{
    public class CircleActor : FormActor
    {
        public int Radius { get; }
        public int MinRadius { get; }

        public CircleActor(bool fill, int growing, int radius, (int start, int end) lifespan, string color, Connector connector) : base(fill, growing, lifespan, color, connector)
        {
            Radius = radius;
        }

        public CircleActor(bool fill, int growing, int radius, (int start, int end) lifespan, string color, Connector connector, int minRadius) : base(fill, growing, lifespan, color, connector)
        {
            Radius = radius;
            MinRadius = minRadius;
        }

        //
        protected class CircleSerializable : FormSerializable
        {
            public int Radius { get; set; }
            public int MinRadius { get; set; }
        }

        public override GenericActorSerializable GetCombatReplayJSON(CombatReplayMap map)
        {
            CircleSerializable aux = new CircleSerializable
            {
                Type = "Circle",
                Radius = Radius,
                MinRadius = MinRadius,
                Fill = Filled,
                Color = Color,
                Growing = Growing,
                Start = Lifespan.start,
                End = Lifespan.end,
                ConnectedTo = ConnectedTo.GetConnectedTo(map)
            };
            return aux;
        }
    }
}
