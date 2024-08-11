using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BackgroundIconDecoration : GenericIconDecoration
    {
        internal class BackgroundIconDecorationMetadata : GenericIconDecorationMetadata
        {

            public BackgroundIconDecorationMetadata(string icon, uint pixelSize, uint worldSize) : base(icon, pixelSize, worldSize)
            {
            }

            public override string GetSignature()
            {
                return "BI" + PixelSize + Image.GetHashCode().ToString() + WorldSize;
            }
            public override GenericDecorationMetadataDescription GetCombatReplayMetadataDescription()
            {
                return new BackgroundIconDecorationMetadataDescription(this);
            }
        }
        internal class BackgroundIconDecorationRenderingData : GenericIconDecorationRenderingData
        {
            public IReadOnlyList<ParametricPoint1D> Opacities { get; }
            public IReadOnlyList<ParametricPoint1D> Heights { get; }
            public BackgroundIconDecorationRenderingData((long, long) lifespan, IReadOnlyList<ParametricPoint1D> opacities, IReadOnlyList<ParametricPoint1D> heights, GeographicalConnector connector) : base(lifespan, connector)
            {
                Opacities = opacities;
                Heights = heights;
            }
            public override void UsingSkillMode(SkillModeDescriptor skill)
            {
            }

            public override GenericDecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature)
            {
                return new BackgroundIconDecorationRenderingDescription(log, this, map, usedSkills, usedBuffs, metadataSignature);
            }
        }
        private new BackgroundIconDecorationRenderingData DecorationRenderingData => (BackgroundIconDecorationRenderingData)base.DecorationRenderingData;

        public IReadOnlyList<ParametricPoint1D> Opacities => DecorationRenderingData.Opacities;
        public IReadOnlyList<ParametricPoint1D> Heights => DecorationRenderingData.Heights;

        internal BackgroundIconDecoration(BackgroundIconDecorationMetadata metadata, BackgroundIconDecorationRenderingData renderingData) : base(metadata, renderingData)
        {
        }
        public BackgroundIconDecoration(string icon, uint pixelSize, uint worldSize, IReadOnlyList<ParametricPoint1D> opacities, IReadOnlyList<ParametricPoint1D> heights, (long start, long end) lifespan, GeographicalConnector connector) : base(new BackgroundIconDecorationMetadata(icon, pixelSize, worldSize), new BackgroundIconDecorationRenderingData(lifespan, opacities, heights, connector))
        {
        }

        public BackgroundIconDecoration(string icon, uint pixelSize, uint worldSize, IReadOnlyList<ParametricPoint1D> opacities, IReadOnlyList<ParametricPoint1D> heights, Segment lifespan, GeographicalConnector connector) : this(icon, pixelSize, worldSize, opacities, heights, (lifespan.Start, lifespan.End), connector)
        {
        }
    }
}
