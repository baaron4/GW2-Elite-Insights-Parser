namespace GW2EIEvtcParser.EIData
{
    internal class LineDecoration : FormDecoration
    {
        public Connector ConnectedFrom { get; }
        public int Width { get; }

        public LineDecoration(int growing, (int start, int end) lifespan, string color, Connector connector, Connector targetConnector) : base(false, growing, lifespan, color, connector)
        {
            ConnectedFrom = targetConnector;
        }

        public override GenericDecorationSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new LineDecorationSerializable(log, this, map);
        }
    }
}
