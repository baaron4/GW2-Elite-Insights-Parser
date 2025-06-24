using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class OverheadProgressBarDecoration : ProgressBarDecoration
{
    public class OverheadProgressBarDecorationMetadata : ProgressBarDecorationMetadata
    {
        public readonly uint PixelWidth;
        public readonly uint PixelHeight;
        public OverheadProgressBarDecorationMetadata(string color, string secondaryColor, uint width, uint pixelWidth) : base(color, secondaryColor, width, width / 5)
        {
            PixelWidth = pixelWidth;
            PixelHeight = PixelWidth / 5;
        }

        public override string GetSignature()
        {
            return "OProg" + Color + Width + Height.ToString()  + SecondaryColor + PixelWidth + PixelHeight.ToString();
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
        uint pixelWidth,
        (long start, long end) lifespan,
        string color, string secondaryColor,
        IReadOnlyList<(long, double)> progress, AgentConnector connectedTo
        ) : base(
            new OverheadProgressBarDecorationMetadata(
                color, 
                secondaryColor, 
                Math.Min(Math.Max((uint)(connectedTo.Agent.HitboxWidth), 80), 500),
                pixelWidth),
            new OverheadProgressBarDecorationRenderingData(lifespan, progress, connectedTo)
            )
    {
    }
    public OverheadProgressBarDecoration(
        uint pixelWidth,
        Segment lifespan,
        string color, string secondaryColor,
        IReadOnlyList<(long, double)> progress, AgentConnector connectedTo
        ) : this(pixelWidth, (lifespan.Start, lifespan.End), color, secondaryColor, progress, connectedTo)
    {
    }
    public OverheadProgressBarDecoration(
        uint pixelWidth,
        (long start, long end) lifespan,
        Color color, double opacity,
        Color secondaryColor, double secondaryOpacity,
        IReadOnlyList<(long, double)> progress, AgentConnector connectedTo
        ) : this(
            pixelWidth,
            lifespan,
            color.WithAlpha(opacity).ToString(true),
            secondaryColor.WithAlpha(secondaryOpacity).ToString(true),
            progress, connectedTo
            )
    {
    }

    public OverheadProgressBarDecoration(
        uint pixelWidth,
        Segment lifespan,
        Color color, double opacity,
        Color secondaryColor, double secondaryOpacity,
        IReadOnlyList<(long, double)> progress, AgentConnector connectedTo
        ) : this(
            pixelWidth,
            lifespan,
            color.WithAlpha(opacity).ToString(true),
            secondaryColor.WithAlpha(secondaryOpacity).ToString(true),
            progress, connectedTo
            )
    {
    }

    public override FormDecoration Copy(string? color = null)
    {
        return (FormDecoration)new OverheadProgressBarDecoration(PixelWidth, Lifespan, color ?? Color, SecondaryColor, Progress, (AgentConnector)ConnectedTo)
            .UsingFilled(Filled)
            .UsingGrowingEnd(GrowingEnd, GrowingReverse)
            .UsingRotationConnector(RotationConnectedTo)
            .UsingSkillMode(SkillMode);
    }

    public override FormDecoration Copy(string? color = null, string? secondaryColor = null)
    {
        return (FormDecoration)new OverheadProgressBarDecoration(PixelWidth, Lifespan, color ?? Color, secondaryColor ?? SecondaryColor, Progress, (AgentConnector)ConnectedTo)
            .UsingFilled(Filled)
            .UsingGrowingEnd(GrowingEnd, GrowingReverse)
            .UsingRotationConnector(RotationConnectedTo)
            .UsingSkillMode(SkillMode);
    }
}
