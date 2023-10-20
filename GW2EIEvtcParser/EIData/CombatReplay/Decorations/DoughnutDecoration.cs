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
            return (FormDecoration)new DoughnutDecoration(InnerRadius, OuterRadius, Lifespan, Color, ConnectedTo).UsingFilled(Filled).UsingGrowing(Math.Abs(GrowingEnd), GrowingEnd < 0).UsingRotationConnector(RotationConnectedTo).UsingSkillMode(SkillMode);
        }

    }
}
