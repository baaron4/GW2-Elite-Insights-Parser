namespace GW2EIEvtcParser.EIData
{
    internal class CircleDecoration : FormDecoration
    {
        public int Radius { get; }
        public int MinRadius { get; }

        public CircleDecoration(bool fill, int growing, int radius, (int start, int end) lifespan, string color, Connector connector) : base(fill, growing, lifespan, color, connector)
        {
            Radius = radius;
        }

        public CircleDecoration(bool fill, int growing, int radius, (int start, int end) lifespan, string color, Connector connector, int minRadius) : base(fill, growing, lifespan, color, connector)
        {
            Radius = radius;
            MinRadius = minRadius;
        }

        //

        public override GenericDecorationSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new CircleDecorationSerializable(log, this, map);
        }
    }
}
