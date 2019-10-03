using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class LineDecoration : FormDecoration
    {
        public Connector ConnectedFrom { get; }
        public int Width { get; }

        public LineDecoration(int growing, (int start, int end) lifespan, string color, Connector connector, Connector targetConnector) : base(false, growing, lifespan, color, connector)
        {
            ConnectedFrom = targetConnector;
        }

        private class LineSerializable : FormSerializable
        {
            public object ConnectedFrom { get; set; }
        }

        public override GenericDecorationSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            var aux = new LineSerializable
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
