using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class RectangleDecoration : FormDecoration
    {
        internal class RectangleDecorationMetadata : FormDecorationMetadata
        {
            public uint Height { get; }
            public uint Width { get; }

            public RectangleDecorationMetadata(string color, uint width, uint height) : base(color)
            {
                Height = Math.Max(height, 1);
                Width = Math.Max(width, 1);
            }

            public override string GetSignature()
            {
                return "Rect" + Height + Color + Width;
            }
            public override GenericDecorationMetadataDescription GetCombatReplayMetadataDescription()
            {
                return new RectangleDecorationMetadataDescription(this);
            }
        }
        internal class RectangleDecorationRenderingData : FormDecorationRenderingData
        {
            public RectangleDecorationRenderingData((long, long) lifespan, GeographicalConnector connector) : base(lifespan, connector)
            {
            }

            public override GenericDecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature)
            {
                return new RectangleDecorationRenderingDescription(log, this, map, usedSkills, usedBuffs, metadataSignature);
            }
        }
        private new RectangleDecorationMetadata DecorationMetadata => (RectangleDecorationMetadata)base.DecorationMetadata;
        public uint Height => DecorationMetadata.Height;
        public uint Width => DecorationMetadata.Width;

        internal RectangleDecoration(RectangleDecorationMetadata metadata, RectangleDecorationRenderingData renderingData) : base(metadata, renderingData)
        {
        }

        public RectangleDecoration(uint width, uint height, (long start, long end) lifespan, string color, GeographicalConnector connector) : base(new RectangleDecorationMetadata(color, width, height), new RectangleDecorationRenderingData(lifespan, connector))
        {
        }
        public RectangleDecoration(uint width, uint height, (long start, long end) lifespan, Color color, double opacity, GeographicalConnector connector) : this(width, height, lifespan, color.WithAlpha(opacity).ToString(true), connector)
        {
        }
        public override FormDecoration Copy(string color = null)
        {
            return (FormDecoration)new RectangleDecoration(Width, Height, Lifespan, color ?? Color, ConnectedTo).UsingFilled(Filled).UsingGrowingEnd(GrowingEnd, GrowingReverse).UsingRotationConnector(RotationConnectedTo).UsingSkillMode(SkillMode);
        }

        public override FormDecoration GetBorderDecoration(string borderColor = null)
        {
            if (!Filled)
            {
                throw new InvalidOperationException("Non filled rectangles can't have borders");
            }
            var copy = (RectangleDecoration)Copy(borderColor).UsingFilled(false);
            return copy;
        }
        //
    }
}
