using static GW2EIEvtcParser.EIData.ActorOrientationDecoration;

namespace GW2EIEvtcParser.EIData;

public class ActorOrientationDecorationMetadataDescription : GenericAttachedDecorationMetadataDescription
{

    internal ActorOrientationDecorationMetadataDescription(ActorOrientationDecorationMetadata decoration) : base(decoration)
    {
        Type = "ActorOrientation";
    }

}
