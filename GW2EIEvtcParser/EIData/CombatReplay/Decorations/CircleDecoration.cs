using System;

namespace GW2EIEvtcParser.EIData
{
    internal class CircleDecoration : FormDecoration
    {
        public int Radius { get; }
        public int MinRadius { get; }

        public CircleDecoration(int radius, (long start, long end) lifespan, string color, Connector connector) : base(lifespan, color, connector)
        {
            Radius = radius;
        }

        public CircleDecoration(int radius, (long start, long end) lifespan, string color, Connector connector, int minRadius) : base(lifespan, color, connector)
        {
            Radius = radius;
            MinRadius = minRadius;
        }

        public CircleDecoration(int radius, Segment lifespan, string color, Connector connector) : this(radius, (lifespan.Start, lifespan.End), color, connector)
        {
        }

        public CircleDecoration(int radius, Segment lifespan, string color, Connector connector, int minRadius) : this(radius, (lifespan.Start, lifespan.End), color, connector, minRadius)
        {
        }

        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new CircleDecorationCombatReplayDescription(log, this, map);
        }

        public override FormDecoration Copy()
        {
            return (FormDecoration)new CircleDecoration(Radius, Lifespan, Color, ConnectedTo).UsingFilled(Filled).UsingGrowing(GrowingEnd, GrowingReverse).UsingRotationConnector(RotationConnectedTo).UsingSkillMode(SkillMode);
        }
    }
}
