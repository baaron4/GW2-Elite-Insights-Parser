using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class PolygonDecoration : FormDecoration
{
    public class PolygonDecorationMetadata : FormDecorationMetadata
    {
        public readonly uint Radius;
        public readonly uint NbPolygon;

        public PolygonDecorationMetadata(string color, uint radius, uint nbPolygon) : base(color)
        {
            Radius = Math.Max(radius, 1);
            NbPolygon = nbPolygon;
            if (NbPolygon < 3)
            {
                throw new InvalidOperationException("Nb Polygon must be at least 3");
            }
        }

        public override string GetSignature()
        {
            return "Pol" + Radius + Color + NbPolygon;
        }
        public override DecorationMetadataDescription GetCombatReplayMetadataDescription()
        {
            return new PolygonDecorationMetadataDescription(this);
        }
    }
    public class PolygonDecorationRenderingData : FormDecorationRenderingData
    {
        public PolygonDecorationRenderingData((long, long) lifespan, GeographicalConnector connector) : base(lifespan, connector)
        {
        }

        public override DecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature)
        {
            return new PolygonDecorationRenderingDescription(log, this, map, usedSkills, usedBuffs, metadataSignature);
        }
    }
    private new PolygonDecorationMetadata DecorationMetadata => (PolygonDecorationMetadata)base.DecorationMetadata;
    public uint Radius => DecorationMetadata.Radius;
    public uint NbPolygon => DecorationMetadata.NbPolygon;


    protected PolygonDecoration(PolygonDecorationMetadata metadata, PolygonDecorationRenderingData renderingData) : base(metadata, renderingData)
    {
    }

    public PolygonDecoration(uint radius, uint nbPolygon, (long start, long end) lifespan, string color, GeographicalConnector connector) : base(new PolygonDecorationMetadata(color, radius, nbPolygon), new PolygonDecorationRenderingData(lifespan, connector))
    {
    }

    public PolygonDecoration(uint radius, uint nbPolygon, (long start, long end) lifespan, Color color, double opacity, GeographicalConnector connector) : this(radius, nbPolygon, lifespan, color.WithAlpha(opacity).ToString(true), connector)
    {
    }

    public PolygonDecoration(uint radius, uint nbPolygon, in Segment lifespan, string color, GeographicalConnector connector) : this(radius, nbPolygon, (lifespan.Start, lifespan.End), color, connector)
    {
    }

    public PolygonDecoration(uint radius, uint nbPolygon, in Segment lifespan, Color color, double opacity, GeographicalConnector connector) : this(radius, nbPolygon, lifespan, color.WithAlpha(opacity).ToString(true), connector)
    {
    }
    public override FormDecoration Copy(string? color = null)
    {
        return (FormDecoration)new PolygonDecoration(Radius, NbPolygon, Lifespan, color ?? Color, ConnectedTo).UsingFilled(Filled).UsingGrowingEnd(GrowingEnd, GrowingReverse).UsingRotationConnector(RotationConnectedTo).UsingSkillMode(SkillMode);
    }
    public override FormDecoration GetBorderDecoration(string? borderColor = null)
    {
        if (!Filled)
        {
            throw new InvalidOperationException("Non filled polygons can't have borders");
        }
        return (PolygonDecoration)Copy(borderColor).UsingFilled(false);
    }
    //
}
