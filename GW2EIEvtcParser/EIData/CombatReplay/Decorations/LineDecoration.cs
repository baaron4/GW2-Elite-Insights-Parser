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

        public LineDecoration(int growing, Segment lifespan, string color, Connector connector, Connector targetConnector) : this(growing, ((int)lifespan.Start, (int)lifespan.End), color, connector, targetConnector)
        {
        }

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new LineDecorationCombatReplayDescription(log, this, map);
        }
        public override GenericAttachedDecoration UsingRotationConnector(RotationConnector rotationConnectedTo)
        {
            return this;
        }
    }
}
