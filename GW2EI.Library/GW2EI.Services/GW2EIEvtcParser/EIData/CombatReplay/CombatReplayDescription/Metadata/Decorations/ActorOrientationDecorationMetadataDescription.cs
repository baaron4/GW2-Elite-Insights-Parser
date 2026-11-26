using static GW2EIEvtcParser.EIData.ActorOrientationDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class ActorOrientationDecorationMetadataDescription : AttachedDecorationMetadataDescription
{

    internal ActorOrientationDecorationMetadataDescription(ActorOrientationDecorationMetadata decoration, StringToUlongHandler stringToUlongHandler) : base(decoration, stringToUlongHandler)
    {
        Type = Types.ActorOrientation;
    }

}
