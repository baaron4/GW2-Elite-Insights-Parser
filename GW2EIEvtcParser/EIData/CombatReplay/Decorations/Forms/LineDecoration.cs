using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class LineDecoration : FormDecoration
    {
        internal class LineDecorationMetadata : FormDecorationMetadata
        {

            public LineDecorationMetadata(string color) : base(color)
            {
            }

            public override string GetSignature()
            {
                return "Line" + Color;
            }
            public override GenericDecorationMetadataDescription GetCombatReplayMetadataDescription()
            {
                return new LineDecorationMetadataDescription(this);
            }
        }
        internal class LineDecorationRenderingData : FormDecorationRenderingData
        {
            public GeographicalConnector ConnectedFrom { get; }
            public LineDecorationRenderingData((long, long) lifespan, GeographicalConnector connector, GeographicalConnector targetConnector) : base(lifespan, connector)
            {
                ConnectedFrom = targetConnector;
            }
            public override void UsingFilled(bool filled)
            {
            }
            public override void UsingRotationConnector(RotationConnector rotationConnectedTo)
            {
            }

            public override GenericDecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature)
            {
                return new LineDecorationRenderingDescription(log, this, map, usedSkills, usedBuffs, metadataSignature);
            }
        }
        private new LineDecorationRenderingData DecorationRenderingData => (LineDecorationRenderingData)base.DecorationRenderingData;
        public GeographicalConnector ConnectedFrom => DecorationRenderingData.ConnectedFrom;

        internal LineDecoration(LineDecorationMetadata metadata, LineDecorationRenderingData renderingData) : base(metadata, renderingData)
        {
        }

        public LineDecoration((long start, long end) lifespan, string color, GeographicalConnector connector, GeographicalConnector targetConnector) : base(new LineDecorationMetadata(color), new LineDecorationRenderingData(lifespan, connector, targetConnector))
        {
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
    }
}
