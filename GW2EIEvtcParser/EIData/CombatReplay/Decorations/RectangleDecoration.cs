using System;

namespace GW2EIEvtcParser.EIData
{
    internal class RectangleDecoration : FormDecoration
    {
        public int Height { get; }
        public int Width { get; }

        public RectangleDecoration(int width, int height, (long start, long end) lifespan, string color, Connector connector) : base( lifespan, color, connector)
        {
            Height = height;
            Width = width;
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
