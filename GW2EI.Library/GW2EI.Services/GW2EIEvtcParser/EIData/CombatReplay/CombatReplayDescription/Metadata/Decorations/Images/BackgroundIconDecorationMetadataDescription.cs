using static GW2EIEvtcParser.EIData.BackgroundIconDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class BackgroundIconDecorationMetadataDescription : ImageDecorationMetadataDescription
{

    internal BackgroundIconDecorationMetadataDescription(BackgroundIconDecorationMetadata decoration, StringToUlongHandler stringToUlongHandler) : base(decoration, stringToUlongHandler)
    {
        Type = Types.BackgroundIcon;
    }
}
