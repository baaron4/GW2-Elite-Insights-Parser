using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class RegularPolygonDecoration : FormDecoration
{
    public class RegularPolygonDecorationMetadata : FormDecorationMetadata
    {
        public readonly uint Radius;
        public readonly uint NbPolygon;

        public RegularPolygonDecorationMetadata(string color, uint radius, uint nbPolygon) : base(color)
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
            return "RegPol" + Radius + Color + NbPolygon;
        }
        public override DecorationMetadataDescription GetCombatReplayMetadataDescription()
        {
            return new RegularPolygonDecorationMetadataDescription(this);
        }
    }
    public class RegularPolygonDecorationRenderingData : FormDecorationRenderingData
    {
        public RegularPolygonDecorationRenderingData((long, long) lifespan, GeographicalConnector connector) : base(lifespan, connector)
        {
        }

        public override DecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature)
        {
            return new RegularPolygonDecorationRenderingDescription(log, this, map, usedSkills, usedBuffs, metadataSignature);
        }
    }
    private new RegularPolygonDecorationMetadata DecorationMetadata => (RegularPolygonDecorationMetadata)base.DecorationMetadata;
    public uint Radius => DecorationMetadata.Radius;
    public uint NbPolygon => DecorationMetadata.NbPolygon;


    protected RegularPolygonDecoration(RegularPolygonDecorationMetadata metadata, RegularPolygonDecorationRenderingData renderingData) : base(metadata, renderingData)
    {
    }

    public RegularPolygonDecoration(uint radius, uint nbPolygon, (long start, long end) lifespan, string color, GeographicalConnector connector) : base(new RegularPolygonDecorationMetadata(color, radius, nbPolygon), new RegularPolygonDecorationRenderingData(lifespan, connector))
    {
    }

    public RegularPolygonDecoration(uint radius, uint nbPolygon, (long start, long end) lifespan, Color color, double opacity, GeographicalConnector connector) : this(radius, nbPolygon, lifespan, color.WithAlpha(opacity).ToString(true), connector)
    {
    }

    public RegularPolygonDecoration(uint radius, uint nbPolygon, in Segment lifespan, string color, GeographicalConnector connector) : this(radius, nbPolygon, (lifespan.Start, lifespan.End), color, connector)
    {
    }

    public RegularPolygonDecoration(uint radius, uint nbPolygon, in Segment lifespan, Color color, double opacity, GeographicalConnector connector) : this(radius, nbPolygon, lifespan, color.WithAlpha(opacity).ToString(true), connector)
    {
    }
    public override FormDecoration Copy(string? color = null)
    {
        return (FormDecoration)new RegularPolygonDecoration(Radius, NbPolygon, Lifespan, color ?? Color, ConnectedTo).UsingFilled(Filled).UsingGrowingEnd(GrowingEnd, GrowingReverse).UsingRotationConnector(RotationConnectedTo).UsingSkillMode(SkillMode);
    }
    public override FormDecoration GetBorderDecoration(string? borderColor = null)
    {
        if (!Filled)
        {
            throw new InvalidOperationException("Non filled polygons can't have borders");
        }
        return (RegularPolygonDecoration)Copy(borderColor).UsingFilled(false);
    }
    //
}
