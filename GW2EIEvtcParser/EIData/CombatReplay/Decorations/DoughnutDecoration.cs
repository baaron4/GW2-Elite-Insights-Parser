using System;

namespace GW2EIEvtcParser.EIData
{
    internal class DoughnutDecoration : FormDecoration
    {
        public int OuterRadius { get; }
        public int InnerRadius { get; }

        public DoughnutDecoration(int innerRadius, int outerRadius, (long start, long end) lifespan, string color, Connector connector) : base(lifespan, color, connector)
        {
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
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
