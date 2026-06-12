using System.Numerics;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class CustomPolygonDecoration : FormDecoration
{
    public class CustomPolygonDecorationMetadata : FormDecorationMetadata
    {
        public CustomPolygonDecorationMetadata(string color) : base(color)
        {
        }

        public override string GetSignature()
        {
            return "CusPol" + Color;
        }
        public override DecorationMetadataDescription GetCombatReplayMetadataDescription()
        {
            return new CustomPolygonDecorationMetadataDescription(this);
        }
    }
    public class CustomPolygonDecorationRenderingData : FormDecorationRenderingData
    {
        public readonly IReadOnlyList<Vector3> Points;
        public CustomPolygonDecorationRenderingData(IReadOnlyList<Vector3> points, (long, long) lifespan, GeographicalConnector connector) : base(lifespan, connector)
        {
            Points = points;
        }

        public override DecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature)
        {
            return new CustomPolygonDecorationRenderingDescription(log, this, map, usedSkills, usedBuffs, metadataSignature);
        }
    }
    private new CustomPolygonDecorationMetadata DecorationMetadata => (CustomPolygonDecorationMetadata)base.DecorationMetadata;
    private new CustomPolygonDecorationRenderingData DecorationRenderingData => (CustomPolygonDecorationRenderingData)base.DecorationRenderingData;
    public IReadOnlyList<Vector3> Points => DecorationRenderingData.Points;


    protected CustomPolygonDecoration(CustomPolygonDecorationMetadata metadata, CustomPolygonDecorationRenderingData renderingData) : base(metadata, renderingData)
    {
    }

    public CustomPolygonDecoration(IReadOnlyList<Vector3> points, (long start, long end) lifespan, string color, GeographicalConnector connector) : base(new CustomPolygonDecorationMetadata(color), new CustomPolygonDecorationRenderingData(points, lifespan, connector))
    {
    }

    public CustomPolygonDecoration(IReadOnlyList<Vector3> points, (long start, long end) lifespan, Color color, double opacity, GeographicalConnector connector) : this(points, lifespan, color.WithAlpha(opacity).ToString(true), connector)
    {
    }

    public CustomPolygonDecoration(IReadOnlyList<Vector3> points, in Segment lifespan, string color, GeographicalConnector connector) : this(points, (lifespan.Start, lifespan.End), color, connector)
    {
    }

    public CustomPolygonDecoration(IReadOnlyList<Vector3> points, in Segment lifespan, Color color, double opacity, GeographicalConnector connector) : this(points, lifespan, color.WithAlpha(opacity).ToString(true), connector)
    {
    }
    public override FormDecoration Copy(string? color = null)
    {
        return (FormDecoration)new CustomPolygonDecoration(Points, Lifespan, color ?? Color, ConnectedTo).UsingFilled(Filled).UsingGrowingEnd(GrowingEnd, GrowingReverse).UsingRotationConnector(RotationConnectedTo).UsingSkillMode(SkillMode);
    }
    public override FormDecoration GetBorderDecoration(string? borderColor = null)
    {
        if (!Filled)
        {
            throw new InvalidOperationException("Non filled polygons can't have borders");
        }
        return (CustomPolygonDecoration)Copy(borderColor).UsingFilled(false);
    }
    //
}
