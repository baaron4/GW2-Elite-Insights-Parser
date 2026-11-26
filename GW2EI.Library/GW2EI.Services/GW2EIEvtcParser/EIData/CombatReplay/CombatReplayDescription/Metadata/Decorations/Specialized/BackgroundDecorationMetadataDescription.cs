using static GW2EIEvtcParser.EIData.BackgroundDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public abstract class BackgroundDecorationMetadataDescription : DecorationMetadataDescription
{
    internal BackgroundDecorationMetadataDescription(BackgroundDecorationMetadata decoration, StringToUlongHandler stringToUlongHandler) : base(decoration, stringToUlongHandler)
    {

    }

}
