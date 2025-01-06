using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Connector;

namespace GW2EIEvtcParser.EIData;

internal class ProgressBarDecoration : RectangleDecoration
{
    public class ProgressBarDecorationMetadata : RectangleDecorationMetadata
    {
        public readonly string SecondaryColor;
        public ProgressBarDecorationMetadata(string color, string secondaryColor, uint width, uint height) : base(color, width, height)
        {
            SecondaryColor = secondaryColor;
        }

        public override string GetSignature()
        {
            return "Prog" + Height + Color + Width + SecondaryColor;
        }
        public override DecorationMetadataDescription GetCombatReplayMetadataDescription()
        {
            return new ProgressBarDecorationMetadataDescription(this);
        }
    }
    public class ProgressBarDecorationRenderingData : RectangleDecorationRenderingData
    {
        public readonly IReadOnlyList<(long, double)> Progress;
        public InterpolationMethod Method { get; private set; } = InterpolationMethod.Linear;
        public ProgressBarDecorationRenderingData((long, long) lifespan, IReadOnlyList<(long, double)> progress, GeographicalConnector connector) : base(lifespan, connector)
        {
            Progress = progress;
        }

        public override DecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature)
        {
            return new ProgressBarDecorationRenderingDescription(log, this, map, usedSkills, usedBuffs, metadataSignature);
        }

        public void UsingInterpolationMethod(InterpolationMethod method)
        {
            Method = method;
        }
    }
    private new ProgressBarDecorationMetadata DecorationMetadata => (ProgressBarDecorationMetadata)base.DecorationMetadata;
    private ProgressBarDecorationRenderingData RenderingMetadata => (ProgressBarDecorationRenderingData)DecorationRenderingData;

    public string SecondaryColor => DecorationMetadata.SecondaryColor;
    public IReadOnlyList<(long, double)> Progress => RenderingMetadata.Progress;

    protected ProgressBarDecoration(ProgressBarDecorationMetadata metadata, ProgressBarDecorationRenderingData renderingData) : base(metadata, renderingData)
    {
    }

    public ProgressBarDecoration(
        uint width, uint height, (long start, long end) lifespan,
        string color, string secondaryColor,
        IReadOnlyList<(long, double)> progress, GeographicalConnector connector
        ) : base(
            new ProgressBarDecorationMetadata(color, secondaryColor, width, height),
            new ProgressBarDecorationRenderingData(lifespan, progress, connector)
            )
    {
    }
    public ProgressBarDecoration(
        uint width, uint height, (long start, long end) lifespan,
        Color color, double opacity,
        Color secondaryColor, double secondaryOpacity,
        IReadOnlyList<(long, double)> progress, GeographicalConnector connector
        ) : this(
            width, height, lifespan,
            color.WithAlpha(opacity).ToString(true),
            secondaryColor.WithAlpha(secondaryOpacity).ToString(true),
            progress, connector
            )
    {
    }

    public override FormDecoration Copy(string? color = null)
    {
        return (FormDecoration)new ProgressBarDecoration(Width, Height, Lifespan, color ?? Color, SecondaryColor, Progress, ConnectedTo)
            .UsingFilled(Filled)
            .UsingGrowingEnd(GrowingEnd, GrowingReverse)
            .UsingRotationConnector(RotationConnectedTo)
            .UsingSkillMode(SkillMode);
    }

    public virtual FormDecoration Copy(string? color = null, string? secondaryColor = null)
    {
        return (FormDecoration)new ProgressBarDecoration(Width, Height, Lifespan, color ?? Color, secondaryColor ?? SecondaryColor, Progress, ConnectedTo)
            .UsingFilled(Filled)
            .UsingGrowingEnd(GrowingEnd, GrowingReverse)
            .UsingRotationConnector(RotationConnectedTo)
            .UsingSkillMode(SkillMode);
    }

    public ProgressBarDecoration UsingInterpolationMethod(InterpolationMethod method)
    {
        RenderingMetadata.UsingInterpolationMethod(method);
        return this;
    }

    public override FormDecoration UsingFilled(bool filled)
    {
        return this;
    }

    public override FormDecoration UsingGrowingEnd(long growingEnd, bool reverse = false)
    {
        return this;
    }

    public override FormDecoration GetBorderDecoration(string? borderColor = null)
    {
        throw new InvalidOperationException("Progress bars can't have borders");
    }
}
