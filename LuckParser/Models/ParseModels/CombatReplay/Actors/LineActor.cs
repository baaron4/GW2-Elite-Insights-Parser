using Newtonsoft.Json;
using System;

namespace LuckParser.Models.ParseModels
{
    public class LineActor : Actor
    {
        public Connector ConnectedFrom { get; }
        public int Width { get; }

        public LineActor(int growing, Tuple<int, int> lifespan, string color, Connector connector, Connector targetConnector) : base(false, growing, lifespan, color, connector)
        {
            ConnectedFrom = targetConnector;
        }

        private class LineSerializable : Serializable
        {
            public object ConnectedFrom { get; set; }
        }

        public override string GetCombatReplayJSON(CombatReplayMap map)
        {
            LineSerializable aux = new LineSerializable
            {
                Type = "Line",
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
