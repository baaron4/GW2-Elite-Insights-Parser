using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Connector;

namespace GW2EIEvtcParser.EIData;

internal class OverheadProgressBarDecoration : ProgressBarDecoration
{
    public class OverheadProgressBarDecorationMetadata : ProgressBarDecorationMetadata
    {
        public readonly uint PixelWidth;
        public readonly uint PixelHeight;
        public OverheadProgressBarDecorationMetadata(string color, string secondaryColor, uint width, uint height, uint pixelWidth, uint pixelHeight) : base(color, secondaryColor, width, height)
        {
            PixelWidth = pixelWidth;
            PixelHeight = pixelHeight;
        }

        public override string GetSignature()
        {
            return "OProg" + Height + Color + Width + SecondaryColor + PixelWidth + PixelHeight.ToString();
        }
        public override DecorationMetadataDescription GetCombatReplayMetadataDescription()
        {
            return new OverheadProgressBarDecorationMetadataDescription(this);
        }
    }
    public class OverheadProgressBarDecorationRenderingData : ProgressBarDecorationRenderingData
    {
        public OverheadProgressBarDecorationRenderingData((long, long) lifespan, IReadOnlyList<(long, double)> progress, GeographicalConnector connector) : base(lifespan, progress, connector)
        {
        }

        public override DecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature)
        {
            return new OverheadProgressBarDecorationRenderingDescription(log, this, map, usedSkills, usedBuffs, metadataSignature);
        }
    }
    private new OverheadProgressBarDecorationMetadata DecorationMetadata => (OverheadProgressBarDecorationMetadata)base.DecorationMetadata;
    private OverheadProgressBarDecorationRenderingData RenderingMetadata => (OverheadProgressBarDecorationRenderingData)DecorationRenderingData;

    public uint PixelWidth => DecorationMetadata.PixelWidth;
    public uint PixelHeight => DecorationMetadata.PixelHeight;

    protected OverheadProgressBarDecoration(OverheadProgressBarDecorationMetadata metadata, OverheadProgressBarDecorationRenderingData renderingData) : base(metadata, renderingData)
    {
    }

    public OverheadProgressBarDecoration(
        uint width, uint height, 
        uint pixelWidth, uint pixelHeight,
        (long start, long end) lifespan,
        string color, string secondaryColor,
        IReadOnlyList<(long, double)> progress, GeographicalConnector connector
        ) : base(
            new OverheadProgressBarDecorationMetadata(color, secondaryColor, width, height, pixelWidth, pixelHeight),
            new OverheadProgressBarDecorationRenderingData(lifespan, progress, connector)
            )
    {
    }
    public OverheadProgressBarDecoration(
        uint width, uint height,
        uint pixelWidth, uint pixelHeight,
        (long start, long end) lifespan,
        Color color, double opacity,
        Color secondaryColor, double secondaryOpacity,
        IReadOnlyList<(long, double)> progress, GeographicalConnector connector
        ) : this(
            width, height, 
            pixelWidth, pixelHeight,
            lifespan,
            color.WithAlpha(opacity).ToString(true),
            secondaryColor.WithAlpha(secondaryOpacity).ToString(true),
            progress, connector
            )
    {
    }

    public override FormDecoration Copy(string? color = null)
    {
        return (FormDecoration)new OverheadProgressBarDecoration(Width, Height, PixelWidth, PixelHeight, Lifespan, color ?? Color, SecondaryColor, Progress, ConnectedTo)
            .UsingFilled(Filled)
            .UsingGrowingEnd(GrowingEnd, GrowingReverse)
            .UsingRotationConnector(RotationConnectedTo)
            .UsingSkillMode(SkillMode);
    }

    public override FormDecoration Copy(string? color = null, string? secondaryColor = null)
    {
        return (FormDecoration)new OverheadProgressBarDecoration(Width, Height, PixelWidth, PixelHeight, Lifespan, color ?? Color, secondaryColor ?? SecondaryColor, Progress, ConnectedTo)
            .UsingFilled(Filled)
            .UsingGrowingEnd(GrowingEnd, GrowingReverse)
            .UsingRotationConnector(RotationConnectedTo)
            .UsingSkillMode(SkillMode);
    }
}
