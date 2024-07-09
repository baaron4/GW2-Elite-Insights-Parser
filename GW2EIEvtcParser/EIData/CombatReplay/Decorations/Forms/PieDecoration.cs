using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class PieDecoration : CircleDecoration
    {
        internal class PieDecorationMetadata : CircleDecorationMetadata
        {
            public float OpeningAngle { get; } //in degrees

            public PieDecorationMetadata(string color, uint radius, uint minRadius, float openingAngle) : base(color, radius, minRadius)
            {
                OpeningAngle = openingAngle;
                if (OpeningAngle < 0)
                {
                    throw new InvalidOperationException("OpeningAngle must be strictly positive");
                }
                if (OpeningAngle > 360)
                {
                    throw new InvalidOperationException("OpeningAngle must be <= 360");
                }
            }

            public override string GetSignature()
            {
                return "Pie" + Radius + Color + MinRadius + OpeningAngle.ToString();
            }
            internal override GenericDecoration GetDecorationFromVariable(GenericDecorationRenderingData renderingData)
            {
                if (renderingData is PieDecorationRenderingData  expectedRenderingData)
                {
                    return new PieDecoration(this,  expectedRenderingData);
                }
                throw new InvalidOperationException("Expected VariablePieDecoration");
            }
        }
        internal class PieDecorationRenderingData : CircleDecorationRenderingData
        {
            public PieDecorationRenderingData((long, long) lifespan, GeographicalConnector connector) : base(lifespan, connector)
            {
            }
        }
        private new PieDecorationMetadata DecorationMetadata => (PieDecorationMetadata)base.DecorationMetadata;
        public float OpeningAngle => DecorationMetadata.OpeningAngle;

        internal PieDecoration(PieDecorationMetadata metadata, PieDecorationRenderingData renderingData) : base(metadata, renderingData)
        {
        }

        //using arcs rotation argument as Input (cone in facing direction). Y direction is reversed due to different axis definitions for arc and javascript

        public PieDecoration(uint radius, float openingAngle, (long start, long end) lifespan, string color, GeographicalConnector connector) : base(new PieDecorationMetadata(color, radius, 0, openingAngle), new PieDecorationRenderingData(lifespan, connector))
        {
        }

        public PieDecoration(uint radius, float openingAngle, (long start, long end) lifespan, Color color, double opacity, GeographicalConnector connector) : this(radius, openingAngle, lifespan, color.WithAlpha(opacity).ToString(true), connector)
        {
        }

        public override FormDecoration Copy(string color = null)
        {
            return (PieDecoration)new PieDecoration(Radius, OpeningAngle, Lifespan, color ?? Color, ConnectedTo).UsingFilled(Filled).UsingGrowingEnd(GrowingEnd, GrowingReverse).UsingRotationConnector(RotationConnectedTo).UsingSkillMode(SkillMode);
        }

        //

        public override GenericDecorationRenderableDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            return new PieDecorationRenderableDescription(log, this, map, usedSkills, usedBuffs);
        }
    }
}
