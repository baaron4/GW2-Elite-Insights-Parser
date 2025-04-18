using static GW2EIEvtcParser.EIData.ActorOrientationDecoration;

namespace GW2EIEvtcParser.EIData;

public class ActorOrientationDecorationMetadataDescription : AttachedDecorationMetadataDescription
{

    internal ActorOrientationDecorationMetadataDescription(ActorOrientationDecorationMetadata decoration) : base(decoration)
    {
        Type = Types.ActorOrientation;
    }

}
