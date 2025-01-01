using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class PieDecoration : CircleDecoration
{
    public class PieDecorationMetadata : CircleDecorationMetadata
    {
        public readonly float OpeningAngle; //in degrees

        public PieDecorationMetadata(string color, uint radius, uint minRadius, float openingAngle) : base(color, radius, minRadius)
        {
            OpeningAngle = openingAngle;
            if (OpeningAngle < 0)
            {
                throw new InvalidOperationException("OpeningAngle must be strictly positive");
            }
            if (OpeningAngle > 360)
            {
                throw new InvalidOperationException("OpeningAngle must be <= 360");
            }
        }

        public override string GetSignature()
        {
            return "Pie" + Radius + Color + MinRadius + OpeningAngle.ToString();
        }
        public override DecorationMetadataDescription GetCombatReplayMetadataDescription()
        {
            return new PieDecorationMetadataDescription(this);
        }
    }
    public class PieDecorationRenderingData : CircleDecorationRenderingData
    {
        public PieDecorationRenderingData((long, long) lifespan, GeographicalConnector connector) : base(lifespan, connector)
        {
        }

        public override DecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature)
        {
            return new PieDecorationRenderingDescription(log, this, map, usedSkills, usedBuffs, metadataSignature);
        }
    }
    private new PieDecorationMetadata DecorationMetadata => (PieDecorationMetadata)base.DecorationMetadata;
    public float OpeningAngle => DecorationMetadata.OpeningAngle;

    //using arcs rotation argument as Input (cone in facing direction). Y direction is reversed due to different axis definitions for arc and javascript

    public PieDecoration(uint radius, float openingAngle, (long start, long end) lifespan, string color, GeographicalConnector connector) : base(new PieDecorationMetadata(color, radius, 0, openingAngle), new PieDecorationRenderingData(lifespan, connector))
    {
    }

    public PieDecoration(uint radius, float openingAngle, (long start, long end) lifespan, Color color, double opacity, GeographicalConnector connector) : this(radius, openingAngle, lifespan, color.WithAlpha(opacity).ToString(true), connector)
    {
    }

    public override FormDecoration Copy(string? color = null)
    {
        return (PieDecoration)new PieDecoration(Radius, OpeningAngle, Lifespan, color ?? Color, ConnectedTo).UsingFilled(Filled).UsingGrowingEnd(GrowingEnd, GrowingReverse).UsingRotationConnector(RotationConnectedTo).UsingSkillMode(SkillMode);
    }

    //
}
