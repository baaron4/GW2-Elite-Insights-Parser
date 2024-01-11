using System;

namespace GW2EIEvtcParser.EIData
{
    internal class CircleDecoration : FormDecoration
    {
        public uint Radius { get; }
        public uint MinRadius { get; }

        public CircleDecoration(uint radius, (long start, long end) lifespan, string color, GeographicalConnector connector) : this(radius, 0, lifespan, color, connector)
        {
        }

        public CircleDecoration(uint radius, (long start, long end) lifespan, Color color, double opacity, GeographicalConnector connector) : this(radius, 0, lifespan, color.WithAlpha(opacity).ToString(true), connector)
        {
        }

        public CircleDecoration(uint radius, uint minRadius, (long start, long end) lifespan, string color, GeographicalConnector connector) : base(lifespan, color, connector)
        {
            if (radius == 0)
            {
                throw new InvalidOperationException("Radius must be strictly positive");
            }
            if (minRadius >= radius)
            {
                throw new InvalidOperationException("Radius must be > MinRadius");
            }
            Radius = radius;
            MinRadius = minRadius;
        }

        public CircleDecoration(uint radius, uint minRadius, (long start, long end) lifespan, Color color, double opacity, GeographicalConnector connector) : this(radius, minRadius, lifespan, color.WithAlpha(opacity).ToString(true), connector)
        {
        }

        public CircleDecoration(uint radius, Segment lifespan, string color, GeographicalConnector connector) : this(radius, (lifespan.Start, lifespan.End), color, connector)
        {
        }
        public CircleDecoration(uint radius, Segment lifespan, Color color, double opacity, GeographicalConnector connector) : this(radius, lifespan, color.WithAlpha(opacity).ToString(true), connector)
        {
        }

        public CircleDecoration(uint radius, uint minRadius, Segment lifespan, string color, GeographicalConnector connector) : this(radius, minRadius, (lifespan.Start, lifespan.End), color, connector)
        {
        }

        public CircleDecoration(uint radius, uint minRadius, Segment lifespan, Color color, double opacity, GeographicalConnector connector) : this(radius, minRadius, lifespan, color.WithAlpha(opacity).ToString(true), connector)
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
