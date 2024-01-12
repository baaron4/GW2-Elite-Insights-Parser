using System;

namespace GW2EIEvtcParser.EIData
{
    internal class DoughnutDecoration : FormDecoration
    {
        public uint OuterRadius { get; }
        public uint InnerRadius { get; }

        public DoughnutDecoration(uint innerRadius, uint outerRadius, (long start, long end) lifespan, string color, GeographicalConnector connector) : base(lifespan, color, connector)
        {
            if (outerRadius <= innerRadius)
            {
                throw new InvalidOperationException("OuterRadius must be > then InnerRadius");
            }
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
        }
        public DoughnutDecoration(uint innerRadius, uint outerRadius, (long start, long end) lifespan, Color color, double opacity, GeographicalConnector connector) : this(innerRadius, outerRadius, lifespan, color.WithAlpha(opacity).ToString(true), connector)
        {
        }
        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new DoughnutDecorationCombatReplayDescription(log, this, map);
        }
        public override FormDecoration Copy()
        {
            return (FormDecoration)new DoughnutDecoration(InnerRadius, OuterRadius, Lifespan, Color, ConnectedTo).UsingFilled(Filled).UsingGrowingEnd(GrowingEnd, GrowingReverse).UsingRotationConnector(RotationConnectedTo).UsingSkillMode(SkillMode);
        }

        public override FormDecoration GetBorderDecoration(string borderColor = null)
        {
            if (!Filled)
            {
                throw new InvalidOperationException("Non filled doughtnuts can't have borders");
            }
            var copy = (DoughnutDecoration)Copy().UsingFilled(false);
            if (borderColor != null)
            {
                copy.Color = borderColor;
            }
            return copy;
        }

    }
}
