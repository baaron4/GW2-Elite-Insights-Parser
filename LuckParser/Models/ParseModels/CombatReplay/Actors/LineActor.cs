using Newtonsoft.Json;
using System;

namespace LuckParser.Models.ParseModels
{
    public class LineActor : Actor
    {
        public Connector ConnectedFrom { get; }
        public int Width { get; }

        public LineActor(int growing, int width, Tuple<int, int> lifespan, string color, Connector connector, Connector targetConnector) : base(false, growing, lifespan, color, connector)
        {
            ConnectedFrom = targetConnector;
            Width = width;
        }

        private class LineSerializable : Serializable
        {
            public Object ConnectedFrom { get; set; }
            public int Width { get; set; }
        }

        public override string GetCombatReplayJSON(CombatReplayMap map)
        {
            LineSerializable aux = new LineSerializable
            {
                Type = "Line",
                Width = Width,
                Fill = Filled,
                Color = Color,
                Growing = Growing,
                Start = Lifespan.Item1,
                End = Lifespan.Item2,
                ConnectedTo = ConnectedTo.GetConnectedTo(map),
                ConnectedFrom = ConnectedFrom.GetConnectedTo(map)
            };
            return JsonConvert.SerializeObject(aux);
        }
    }
}
