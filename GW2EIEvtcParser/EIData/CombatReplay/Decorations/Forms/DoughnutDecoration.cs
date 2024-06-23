using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class DoughnutDecoration : FormDecoration
    {
        internal class DoughnutDecorationMetadata : FormDecorationMetadata
        {
            public uint OuterRadius { get; }
            public uint InnerRadius { get; }

            public DoughnutDecorationMetadata(string color, uint innerRadius, uint outerRadius) : base(color)
            {
                OuterRadius = Math.Max(outerRadius, 1);
                InnerRadius = innerRadius;
                if (OuterRadius <= InnerRadius)
                {
                    throw new InvalidOperationException("OuterRadius must be > to InnerRadius");
                }
            }

            public override string GetSignature()
            {
                return "Dough" + OuterRadius + Color + InnerRadius;
            }
            internal override GenericDecoration GetDecorationFromVariable(VariableGenericDecoration variable)
            {
                if (variable is VariableDoughnutDecoration expectedVariable)
                {
                    return new DoughnutDecoration(this, expectedVariable);
                }
                throw new InvalidOperationException("Expected VariableDoughnutDecoration");
            }
        }
        internal class VariableDoughnutDecoration : VariableFormDecoration
        {
            public VariableDoughnutDecoration((long, long) lifespan, GeographicalConnector connector) : base(lifespan, connector)
            {
            }
        }
        private new DoughnutDecorationMetadata DecorationMetadata => (DoughnutDecorationMetadata)base.DecorationMetadata;
        public uint OuterRadius => DecorationMetadata.OuterRadius;
        public uint InnerRadius => DecorationMetadata.InnerRadius;

        internal DoughnutDecoration(DoughnutDecorationMetadata metadata, VariableDoughnutDecoration variable) : base(metadata, variable)
        {
        }

        public DoughnutDecoration(uint innerRadius, uint outerRadius, (long start, long end) lifespan, string color, GeographicalConnector connector) : base(new DoughnutDecorationMetadata(color, innerRadius, outerRadius), new VariableDoughnutDecoration(lifespan, connector))
        {
        }
        public DoughnutDecoration(uint innerRadius, uint outerRadius, (long start, long end) lifespan, Color color, double opacity, GeographicalConnector connector) : this(innerRadius, outerRadius, lifespan, color.WithAlpha(opacity).ToString(true), connector)
        {
        }
        public override FormDecoration Copy(string color = null)
        {
            return (FormDecoration)new DoughnutDecoration(InnerRadius, OuterRadius, Lifespan, color ?? Color, ConnectedTo).UsingFilled(Filled).UsingGrowingEnd(GrowingEnd, GrowingReverse).UsingRotationConnector(RotationConnectedTo).UsingSkillMode(SkillMode);
        }

        public override FormDecoration GetBorderDecoration(string borderColor = null)
        {
            if (!Filled)
            {
                throw new InvalidOperationException("Non filled doughtnuts can't have borders");
            }
            return (DoughnutDecoration)Copy(borderColor).UsingFilled(false);
        }
        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            return new DoughnutDecorationCombatReplayDescription(log, this, map, usedSkills, usedBuffs);
        }

    }
}
