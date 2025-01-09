using System.Text.Json.Serialization;

namespace GW2EIEvtcParser.EIData;

[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
[JsonDerivedType(typeof(ActorOrientationDecorationMetadataDescription))]
[JsonDerivedType(typeof(MovingPlatformDecorationMetadataDescription))]
[JsonDerivedType(typeof(BackgroundIconDecorationMetadataDescription))]
[JsonDerivedType(typeof(IconDecorationMetadataDescription))]
[JsonDerivedType(typeof(IconOverheadDecorationMetadataDescription))]
[JsonDerivedType(typeof(CircleDecorationMetadataDescription))]
[JsonDerivedType(typeof(DoughnutDecorationMetadataDescription))]
[JsonDerivedType(typeof(LineDecorationMetadataDescription))]
[JsonDerivedType(typeof(PieDecorationMetadataDescription))]
[JsonDerivedType(typeof(RectangleDecorationMetadataDescription))]
[JsonDerivedType(typeof(ProgressBarDecorationMetadataDescription))]
[JsonDerivedType(typeof(OverheadProgressBarDecorationMetadataDescription))]
[JsonDerivedType(typeof(TextDecorationMetadataDescription))]
public abstract class CombatReplayDecorationMetadataDescription : CombatReplayDescription
{
    internal CombatReplayDecorationMetadataDescription() 
    {
    }
}
