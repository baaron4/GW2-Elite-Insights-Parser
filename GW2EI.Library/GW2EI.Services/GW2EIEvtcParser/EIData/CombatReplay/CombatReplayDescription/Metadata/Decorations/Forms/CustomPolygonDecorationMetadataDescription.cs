using static GW2EIEvtcParser.EIData.CustomPolygonDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class CustomPolygonDecorationMetadataDescription : FormDecorationMetadataDescription
{

    internal CustomPolygonDecorationMetadataDescription(CustomPolygonDecorationMetadata decoration, StringToUlongHandler stringToUlongHandler) : base(decoration, stringToUlongHandler)
    {
        Type = Types.CustomPolygon;
    }
}
