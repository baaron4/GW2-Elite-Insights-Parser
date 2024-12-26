using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class BackgroundIconDecoration : ImageDecoration
{
    public class BackgroundIconDecorationMetadata : ImageDecorationMetadata
    {

        public BackgroundIconDecorationMetadata(string icon, uint pixelSize, uint worldSize) : base(icon, pixelSize, worldSize)
        {
        }

        public override string GetSignature()
        {
            return "BI" + PixelSize + Image.GetHashCode().ToString() + WorldSize;
        }
        public override DecorationMetadataDescription GetCombatReplayMetadataDescription()
        {
            return new BackgroundIconDecorationMetadataDescription(this);
        }
    }
    public class BackgroundIconDecorationRenderingData : ImageDecorationRenderingData
    {
        public readonly IReadOnlyList<ParametricPoint1D> Opacities;
        public readonly IReadOnlyList<ParametricPoint1D> Heights;
        public BackgroundIconDecorationRenderingData((long, long) lifespan, IEnumerable<ParametricPoint1D> opacities, IEnumerable<ParametricPoint1D> heights, GeographicalConnector connector) : base(lifespan, connector)
        {
            Opacities = opacities.ToList();
            Heights = heights.ToList();
        }
        public override void UsingSkillMode(SkillModeDescriptor? skill)
        {
        }

        public override DecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature)
        {
            return new BackgroundIconDecorationRenderingDescription(log, this, map, usedSkills, usedBuffs, metadataSignature);
        }
    }
    private new BackgroundIconDecorationRenderingData DecorationRenderingData => (BackgroundIconDecorationRenderingData)base.DecorationRenderingData;

    public IReadOnlyList<ParametricPoint1D> Opacities => DecorationRenderingData.Opacities;
    public IReadOnlyList<ParametricPoint1D> Heights => DecorationRenderingData.Heights;

    public BackgroundIconDecoration(string icon, uint pixelSize, uint worldSize, IEnumerable<ParametricPoint1D> opacities, IEnumerable<ParametricPoint1D> heights, (long start, long end) lifespan, GeographicalConnector connector) : base(new BackgroundIconDecorationMetadata(icon, pixelSize, worldSize), new BackgroundIconDecorationRenderingData(lifespan, opacities, heights, connector))
    {
    }

    public BackgroundIconDecoration(string icon, uint pixelSize, uint worldSize, IEnumerable<ParametricPoint1D> opacities, IEnumerable<ParametricPoint1D> heights, Segment lifespan, GeographicalConnector connector) : this(icon, pixelSize, worldSize, opacities, heights, (lifespan.Start, lifespan.End), connector)
    {
    }
}
