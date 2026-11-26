using static GW2EIEvtcParser.EIData.IconOverheadDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class IconOverheadDecorationMetadataDescription : IconDecorationMetadataDescription
{

    internal IconOverheadDecorationMetadataDescription(IconOverheadDecorationMetadata decoration, StringToUlongHandler stringToUlongHandler) : base(decoration, stringToUlongHandler)
    {
        Type = Types.IconOverhead;
    }
}
