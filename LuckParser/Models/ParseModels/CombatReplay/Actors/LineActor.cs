using LuckParser.Parser;
using Newtonsoft.Json;
using System;

namespace LuckParser.Models.ParseModels
{
    public class LineActor : FormActor
    {
        public Connector ConnectedFrom { get; }
        public int Width { get; }

        public LineActor(int growing, (int start, int end) lifespan, string color, Connector connector, Connector targetConnector) : base(false, growing, lifespan, color, connector)
        {
            ConnectedFrom = targetConnector;
        }

        private class LineSerializable : FormSerializable
        {
            public object ConnectedFrom { get; set; }
        }

        public override GenericActorSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            LineSerializable aux = new LineSerializable
            {
                Type = "Line",
                Fill = Filled,
                Color = Color,
                Growing = Growing,
                Start = Lifespan.start,
                End = Lifespan.end,
                ConnectedTo = ConnectedTo.GetConnectedTo(map, log),
                ConnectedFrom = ConnectedFrom.GetConnectedTo(map, log)
            };
            return aux;
        }
    }
}
