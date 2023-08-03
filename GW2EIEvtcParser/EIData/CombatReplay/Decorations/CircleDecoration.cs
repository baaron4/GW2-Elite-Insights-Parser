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

        public CircleDecoration(bool fill, int growing, int radius, Segment lifespan, string color, Connector connector) : this(fill, growing, radius, ((int)lifespan.Start, (int)lifespan.End), color, connector)
        {
        }

        public CircleDecoration(bool fill, int growing, int radius, Segment lifespan, string color, Connector connector, int minRadius) : this(fill, growing, radius, ((int)lifespan.Start, (int)lifespan.End), color, connector, minRadius)
        {
        }

        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new CircleDecorationCombatReplayDescription(log, this, map);
        }
    }
}
