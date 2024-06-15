using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class PieDecoration : CircleDecoration
    {
        internal class ConstantPieDecoration : ConstantCircleDecoration
        {
            public float OpeningAngle { get; } //in degrees

            public ConstantPieDecoration(string color, uint radius, uint minRadius, float openingAngle) : base(color, radius, minRadius)
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
            internal override GenericDecoration GetDecorationFromVariable(VariableGenericDecoration variable)
            {
                if (variable is VariablePieDecoration expectedVariable)
                {
                    return new PieDecoration(this, expectedVariable);
                }
                throw new InvalidOperationException("Expected VariablePieDecoration");
            }
        }
        internal class VariablePieDecoration : VariableCircleDecoration
        {
            public VariablePieDecoration((long, long) lifespan, GeographicalConnector connector) : base(lifespan, connector)
            {
            }
        }
        private new ConstantPieDecoration ConstantDecoration => (ConstantPieDecoration)base.ConstantDecoration;
        public float OpeningAngle => ConstantDecoration.OpeningAngle;

        internal PieDecoration(ConstantPieDecoration constant, VariablePieDecoration variable) : base(constant, variable)
        {
        }

        //using arcs rotation argument as Input (cone in facing direction). Y direction is reversed due to different axis definitions for arc and javascript

        public PieDecoration(uint radius, float openingAngle, (long start, long end) lifespan, string color, GeographicalConnector connector) : base()
        {
            base.ConstantDecoration = new ConstantPieDecoration(color, radius, 0, openingAngle);
            VariableDecoration = new VariablePieDecoration(lifespan, connector);
        }

        public PieDecoration(uint radius, float openingAngle, (long start, long end) lifespan, Color color, double opacity, GeographicalConnector connector) : this(radius, openingAngle, lifespan, color.WithAlpha(opacity).ToString(true), connector)
        {
        }

        public override FormDecoration Copy(string color = null)
        {
            return (PieDecoration)new PieDecoration(Radius, OpeningAngle, Lifespan, color ?? Color, ConnectedTo).UsingFilled(Filled).UsingGrowingEnd(GrowingEnd, GrowingReverse).UsingRotationConnector(RotationConnectedTo).UsingSkillMode(SkillMode);
        }

        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            return new PieDecorationCombatReplayDescription(log, this, map, usedSkills, usedBuffs);
        }
    }
}
