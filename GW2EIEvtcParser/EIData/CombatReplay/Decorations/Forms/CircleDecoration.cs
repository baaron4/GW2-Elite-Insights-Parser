using System;
using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal class CircleDecoration : FormDecoration
    {
        internal class ConstantCircleDecoration : ConstantFormDecoration
        {
            public uint Radius { get; }
            public uint MinRadius { get; }

            public ConstantCircleDecoration(string color, uint radius, uint minRadius) : base(color)
            {
                Radius = Math.Max(radius, 1);
                MinRadius = minRadius;
                if (MinRadius >= Radius)
                {
                    throw new InvalidOperationException("Radius must be > MinRadius");
                }
            }

            public override string GetSignature()
            {
                return "Cir" + Radius + Color + MinRadius;
            }
        }
        internal class VariableCircleDecoration : VariableFormDecoration
        {
            public VariableCircleDecoration((long, long) lifespan, GeographicalConnector connector) : base(lifespan, connector)
            {
            }
        }
        private new ConstantCircleDecoration ConstantDecoration => (ConstantCircleDecoration)base.ConstantDecoration;
        public uint Radius => ConstantDecoration.Radius;
        public uint MinRadius => ConstantDecoration.MinRadius;

        protected CircleDecoration()
        {
        }

        public CircleDecoration(uint radius, uint minRadius, (long start, long end) lifespan, string color, GeographicalConnector connector) : base()
        {
            base.ConstantDecoration = new ConstantCircleDecoration(color, radius, minRadius);
            VariableDecoration = new VariableCircleDecoration(lifespan, connector);
        }

        public CircleDecoration(uint radius, (long start, long end) lifespan, string color, GeographicalConnector connector) : this(radius, 0, lifespan, color, connector)
        {
        }

        public CircleDecoration(uint radius, (long start, long end) lifespan, Color color, double opacity, GeographicalConnector connector) : this(radius, 0, lifespan, color.WithAlpha(opacity).ToString(true), connector)
        {
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
        public override FormDecoration Copy(string color = null)
        {
            return (FormDecoration)new CircleDecoration(Radius, MinRadius, Lifespan, color ?? Color, ConnectedTo).UsingFilled(Filled).UsingGrowingEnd(GrowingEnd, GrowingReverse).UsingRotationConnector(RotationConnectedTo).UsingSkillMode(SkillMode);
        }
        public override FormDecoration GetBorderDecoration(string borderColor = null)
        {
            if (!Filled)
            {
                throw new InvalidOperationException("Non filled circles can't have borders");
            }
            return (CircleDecoration)Copy(borderColor).UsingFilled(false);
        }
        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            return new CircleDecorationCombatReplayDescription(log, this, map, usedSkills, usedBuffs);
        }
    }
}
