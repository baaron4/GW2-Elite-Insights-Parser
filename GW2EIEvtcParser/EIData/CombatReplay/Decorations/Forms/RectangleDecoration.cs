using System;
using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal class RectangleDecoration : FormDecoration
    {
        internal class ConstantRectangleDecoration : ConstantFormDecoration
        {
            public uint Height { get; }
            public uint Width { get; }

            public ConstantRectangleDecoration(string color, uint width, uint height) : base(color)
            {
                Height = Math.Max(height, 1);
                Width = Math.Max(width, 1);
            }

            public override string GetSignature()
            {
                return "Rect" + Height + Color + Width;
            }
        }
        internal class VariableRectangleDecoration : VariableFormDecoration
        {
            public VariableRectangleDecoration((long, long) lifespan, GeographicalConnector connector) : base(lifespan, connector)
            {
            }
        }
        private new ConstantRectangleDecoration ConstantDecoration => (ConstantRectangleDecoration)base.ConstantDecoration;
        public uint Height => ConstantDecoration.Height;
        public uint Width => ConstantDecoration.Width;

        public RectangleDecoration(uint width, uint height, (long start, long end) lifespan, string color, GeographicalConnector connector) : base()
        {
            base.ConstantDecoration = new ConstantRectangleDecoration(color, width, height);
            VariableDecoration = new VariableRectangleDecoration(lifespan, connector);
        }
        public RectangleDecoration(uint width, uint height, (long start, long end) lifespan, Color color, double opacity, GeographicalConnector connector) : this(width, height, lifespan, color.WithAlpha(opacity).ToString(true), connector)
        {
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
        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            return new RectangleDecorationCombatReplayDescription(log, this, map, usedSkills, usedBuffs);
        }
    }
}
