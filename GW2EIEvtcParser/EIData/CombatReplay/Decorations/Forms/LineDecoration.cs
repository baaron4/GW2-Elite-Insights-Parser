using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class LineDecoration : FormDecoration
    {
        internal class ConstantLineDecoration : ConstantFormDecoration
        {

            public ConstantLineDecoration(string color) : base(color)
            {
            }

            public override string GetID()
            {
                throw new NotImplementedException();
            }
        }
        internal class VariableLineDecoration : VariableFormDecoration
        {
            public GeographicalConnector ConnectedFrom { get; }
            public VariableLineDecoration((long, long) lifespan, GeographicalConnector connector, GeographicalConnector targetConnector) : base(lifespan, connector)
            {
                ConnectedFrom = targetConnector;
            }
            public override void UsingFilled(bool filled)
            {
            }
            public override void UsingRotationConnector(RotationConnector rotationConnectedTo)
            {
            }
        }
        private new VariableLineDecoration VariableDecoration => (VariableLineDecoration)base.VariableDecoration;
        public GeographicalConnector ConnectedFrom => VariableDecoration.ConnectedFrom;

        public LineDecoration((long start, long end) lifespan, string color, GeographicalConnector connector, GeographicalConnector targetConnector) : base()
        {
            base.ConstantDecoration = new ConstantLineDecoration(color);
            base.VariableDecoration = new VariableLineDecoration(lifespan, connector, targetConnector);
        }

        public LineDecoration((long start, long end) lifespan, Color color, double opacity, GeographicalConnector connector, GeographicalConnector targetConnector) : this(lifespan, color.WithAlpha(opacity).ToString(true), connector, targetConnector)
        {
        }

        public LineDecoration(Segment lifespan, string color, GeographicalConnector connector, GeographicalConnector targetConnector) : this((lifespan.Start, lifespan.End), color, connector, targetConnector)
        {
        }
        public LineDecoration(Segment lifespan, Color color, double opacity, GeographicalConnector connector, GeographicalConnector targetConnector) : this((lifespan.Start, lifespan.End), color.WithAlpha(opacity).ToString(true), connector, targetConnector)
        {
        }
        public override FormDecoration Copy(string color = null)
        {
            return (FormDecoration)new LineDecoration(Lifespan, color ?? Color, ConnectedTo, ConnectedFrom).UsingFilled(Filled).UsingGrowingEnd(GrowingEnd, GrowingReverse).UsingRotationConnector(RotationConnectedTo).UsingSkillMode(SkillMode);
        }

        public override FormDecoration GetBorderDecoration(string borderColor = null)
        {
            throw new InvalidOperationException("Lines can't have borders");
        }
        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            return new LineDecorationCombatReplayDescription(log, this, map, usedSkills, usedBuffs);
        }
    }
}
