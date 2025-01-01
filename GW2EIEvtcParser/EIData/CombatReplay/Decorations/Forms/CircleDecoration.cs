using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class CircleDecoration : FormDecoration
{
    public class CircleDecorationMetadata : FormDecorationMetadata
    {
        public readonly uint Radius;
        public readonly uint MinRadius;

        public CircleDecorationMetadata(string color, uint radius, uint minRadius) : base(color)
        {
            Radius = Math.Max(radius, 1);
            MinRadius = minRadius;
            if (MinRadius >= Radius)
            {
                throw new InvalidOperationException("Radius must be > MinRadius");
            }
        }

        public override string GetSignature()
        {
            return "Cir" + Radius + Color + MinRadius;
        }
        public override DecorationMetadataDescription GetCombatReplayMetadataDescription()
        {
            return new CircleDecorationMetadataDescription(this);
        }
    }
    public class CircleDecorationRenderingData : FormDecorationRenderingData
    {
        public CircleDecorationRenderingData((long, long) lifespan, GeographicalConnector connector) : base(lifespan, connector)
        {
        }

        public override DecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature)
        {
            return new CircleDecorationRenderingDescription(log, this, map, usedSkills, usedBuffs, metadataSignature);
        }
    }
    private new CircleDecorationMetadata DecorationMetadata => (CircleDecorationMetadata)base.DecorationMetadata;
    public uint Radius => DecorationMetadata.Radius;
    public uint MinRadius => DecorationMetadata.MinRadius;


    protected CircleDecoration(CircleDecorationMetadata metadata, CircleDecorationRenderingData renderingData) : base(metadata, renderingData)
    {
    }

    public CircleDecoration(uint radius, uint minRadius, (long start, long end) lifespan, string color, GeographicalConnector connector) : base(new CircleDecorationMetadata(color, radius, minRadius), new CircleDecorationRenderingData(lifespan, connector))
    {
    }

    public CircleDecoration(uint radius, (long start, long end) lifespan, string color, GeographicalConnector connector) : this(radius, 0, lifespan, color, connector)
    {
    }

    public CircleDecoration(uint radius, (long start, long end) lifespan, Color color, double opacity, GeographicalConnector connector) : this(radius, 0, lifespan, color.WithAlpha(opacity).ToString(true), connector)
    {
    }

    public CircleDecoration(uint radius, uint minRadius, (long start, long end) lifespan, Color color, double opacity, GeographicalConnector connector) : this(radius, minRadius, lifespan, color.WithAlpha(opacity).ToString(true), connector)
    {
    }

    public CircleDecoration(uint radius, in Segment lifespan, string color, GeographicalConnector connector) : this(radius, (lifespan.Start, lifespan.End), color, connector)
    {
    }
    public CircleDecoration(uint radius, in Segment lifespan, Color color, double opacity, GeographicalConnector connector) : this(radius, lifespan, color.WithAlpha(opacity).ToString(true), connector)
    {
    }

    public CircleDecoration(uint radius, uint minRadius, in Segment lifespan, string color, GeographicalConnector connector) : this(radius, minRadius, (lifespan.Start, lifespan.End), color, connector)
    {
    }

    public CircleDecoration(uint radius, uint minRadius, in Segment lifespan, Color color, double opacity, GeographicalConnector connector) : this(radius, minRadius, lifespan, color.WithAlpha(opacity).ToString(true), connector)
    {
    }
    public override FormDecoration Copy(string? color = null)
    {
        return (FormDecoration)new CircleDecoration(Radius, MinRadius, Lifespan, color ?? Color, ConnectedTo).UsingFilled(Filled).UsingGrowingEnd(GrowingEnd, GrowingReverse).UsingRotationConnector(RotationConnectedTo).UsingSkillMode(SkillMode);
    }
    public override FormDecoration GetBorderDecoration(string? borderColor = null)
    {
        if (!Filled)
        {
            throw new InvalidOperationException("Non filled circles can't have borders");
        }
        return (CircleDecoration)Copy(borderColor).UsingFilled(false);
    }
    //
}
