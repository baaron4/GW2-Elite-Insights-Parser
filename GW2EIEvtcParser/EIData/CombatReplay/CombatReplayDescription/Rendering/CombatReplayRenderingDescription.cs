using System.Text.Json.Serialization;

namespace GW2EIEvtcParser.EIData;

[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
[JsonDerivedType(typeof(ActorOrientationDecorationRenderingDescription))]
[JsonDerivedType(typeof(MovingPlatformDecorationRenderingDescription))]
[JsonDerivedType(typeof(BackgroundIconDecorationRenderingDescription))]
[JsonDerivedType(typeof(IconDecorationRenderingDescription))]
[JsonDerivedType(typeof(IconOverheadDecorationRenderingDescription))]
[JsonDerivedType(typeof(DoughnutDecorationRenderingDescription))]
[JsonDerivedType(typeof(LineDecorationRenderingDescription))]
[JsonDerivedType(typeof(PolygonDecorationRenderingDescription))]
[JsonDerivedType(typeof(PieDecorationRenderingDescription))]
[JsonDerivedType(typeof(RectangleDecorationRenderingDescription))]
[JsonDerivedType(typeof(CircleDecorationRenderingDescription))]
[JsonDerivedType(typeof(ProgressBarDecorationRenderingDescription))]
[JsonDerivedType(typeof(OverheadProgressBarDecorationRenderingDescription))]
[JsonDerivedType(typeof(TextDecorationRenderingDescription))]
public abstract class CombatReplayRenderingDescription : CombatReplayDescription
{
    public long Start { get; protected set; }
    public long End { get; protected set; }

    public bool IsMechanicOrSkill { get; protected set; }

    protected CombatReplayRenderingDescription()
    {
    }
}
