using System;

namespace GW2EIEvtcParser.EIData
{
    internal class RectangleDecoration : FormDecoration
    {
        public uint Height { get; }
        public uint Width { get; }

        public RectangleDecoration(uint width, uint height, (long start, long end) lifespan, string color, GeographicalConnector connector) : base( lifespan, color, connector)
        {
            Height = height;
            Width = width;
        }
        public RectangleDecoration(uint width, uint height, (long start, long end) lifespan, Color color, double opacity, GeographicalConnector connector) : this(width, height, lifespan, color.WithAlpha(opacity).ToString(true), connector)
        {
        }
        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new RectangleDecorationCombatReplayDescription(log, this, map);
        }
        public override FormDecoration Copy()
        {
            return (FormDecoration)new RectangleDecoration(Width, Height, Lifespan, Color, ConnectedTo).UsingFilled(Filled).UsingGrowingEnd(GrowingEnd, GrowingReverse).UsingRotationConnector(RotationConnectedTo).UsingSkillMode(SkillMode);
        }

        public override FormDecoration GetBorderDecoration(string borderColor = null)
        {
            if (!Filled)
            {
                throw new InvalidOperationException("Non filled rectangles can't have borders");
            }
            var copy = (RectangleDecoration)Copy().UsingFilled(false);
            if (borderColor != null)
            {
                copy.Color = borderColor;
            }
            return copy;
        }
    }
}
