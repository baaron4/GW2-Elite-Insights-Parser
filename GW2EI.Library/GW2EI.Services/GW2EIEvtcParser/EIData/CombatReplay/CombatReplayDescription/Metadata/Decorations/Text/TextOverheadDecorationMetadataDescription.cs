using static GW2EIEvtcParser.EIData.TextDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class TextOverheadDecorationMetadataDescription : TextDecorationMetadataDescription
{

    internal TextOverheadDecorationMetadataDescription(TextDecorationMetadata decoration, StringToUlongHandler stringToUlongHandler) : base(decoration, stringToUlongHandler)
    {
        Type = Types.TextOverhead;
    }
}
