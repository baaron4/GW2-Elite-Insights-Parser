using Newtonsoft.Json;
using System;

namespace LuckParser.Models.ParseModels
{
    public class CircleActor : Actor
    {
        public int Radius { get; }
        public int MinRadius { get; }

        public CircleActor(bool fill, int growing, int radius, Tuple<int, int> lifespan, string color, Connector connector) : base(fill, growing, lifespan, color, connector)
        {
            Radius = radius;
        }

        public CircleActor(bool fill, int growing, int radius, Tuple<int, int> lifespan, string color, Connector connector, int minRadius) : base(fill, growing, lifespan, color, connector)
        {
            Radius = radius;
            MinRadius = minRadius;
        }

        //
        protected class CircleSerializable : Serializable
        {
            public int Radius { get; set; }
            public int MinRadius { get; set; }
        }

        public override string GetCombatReplayJSON(CombatReplayMap map)
        {
            CircleSerializable aux = new CircleSerializable
            {
                Type = "Circle",
                Radius = Radius,
                MinRadius = MinRadius,
                Fill = Filled,
                Color = Color,
                Growing = Growing,
                Start = Lifespan.Item1,
                End = Lifespan.Item2,
                ConnectedTo = ConnectedTo.GetConnectedTo(map)
            };
            return JsonConvert.SerializeObject(aux);
        }
    }
}
