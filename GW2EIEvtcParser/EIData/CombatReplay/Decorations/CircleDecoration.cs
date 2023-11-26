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

        public CircleDecoration(int radius, int minRadius, (long start, long end) lifespan, string color, Connector connector) : base(lifespan, color, connector)
        {
            Radius = radius;
            MinRadius = minRadius;
        }

        public CircleDecoration(int radius, Segment lifespan, string color, Connector connector) : this(radius, (lifespan.Start, lifespan.End), color, connector)
        {
        }

        public CircleDecoration(int radius, int minRadius, Segment lifespan, string color, Connector connector) : this(radius, minRadius, (lifespan.Start, lifespan.End), color, connector)
        {
        }

        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new CircleDecorationCombatReplayDescription(log, this, map);
        }

        public override FormDecoration Copy()
        {
            return (FormDecoration)new CircleDecoration(Radius, MinRadius, Lifespan, Color, ConnectedTo).UsingFilled(Filled).UsingGrowingEnd(GrowingEnd, GrowingReverse).UsingRotationConnector(RotationConnectedTo).UsingSkillMode(SkillMode);
        }
        public override FormDecoration GetBorderDecoration(string borderColor = null)
        {
            if (!Filled)
            {
                throw new InvalidOperationException("Non filled circles can't have borders");
            }
            var copy = (CircleDecoration)Copy().UsingFilled(false);
            if (borderColor != null)
            {
                copy.Color = borderColor;
            }
            return copy;
        }
    }
}
