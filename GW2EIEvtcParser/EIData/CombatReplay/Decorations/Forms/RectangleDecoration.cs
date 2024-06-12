using System;
using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;

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

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            return new RectangleDecorationCombatReplayDescription(log, this, map, usedSkills, usedBuffs);
        }
        public override FormDecoration Copy(string color = null)
        {
            return (FormDecoration)new RectangleDecoration(Width, Height, Lifespan, color ?? Color, ConnectedTo).UsingFilled(Filled).UsingGrowingEnd(GrowingEnd, GrowingReverse).UsingRotationConnector(RotationConnectedTo).UsingSkillMode(SkillMode);
        }

        public override FormDecoration GetBorderDecoration(string borderColor = null)
        {
            if (!Filled)
            {
                throw new InvalidOperationException("Non filled rectangles can't have borders");
            }
            var copy = (RectangleDecoration)Copy(borderColor).UsingFilled(false);
            return copy;
        }
    }
}
