using System;

namespace GW2EIEvtcParser.EIData
{
    internal class LineDecoration : FormDecoration
    {
        public Connector ConnectedFrom { get; }

        public LineDecoration((long start, long end) lifespan, string color, Connector connector, Connector targetConnector) : base( lifespan, color, connector)
        {
            ConnectedFrom = targetConnector;
        }

        public LineDecoration(Segment lifespan, string color, Connector connector, Connector targetConnector) : this((lifespan.Start, lifespan.End), color, connector, targetConnector)
        {
        }
        public override FormDecoration UsingFilled(bool filled)
        {
            return this;
        }

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new LineDecorationCombatReplayDescription(log, this, map);
        }
        public override GenericAttachedDecoration UsingRotationConnector(RotationConnector rotationConnectedTo)
        {
            return this;
        }
        public override FormDecoration Copy()
        {
            return (FormDecoration)new LineDecoration(Lifespan, Color, ConnectedTo, ConnectedFrom).UsingFilled(Filled).UsingGrowingEnd(GrowingEnd, GrowingReverse).UsingRotationConnector(RotationConnectedTo).UsingSkillMode(SkillMode);
        }

        public override FormDecoration GetBorderDecoration(string borderColor = null)
        {
            throw new InvalidOperationException("Lines can't have borders");
        }
    }
}
