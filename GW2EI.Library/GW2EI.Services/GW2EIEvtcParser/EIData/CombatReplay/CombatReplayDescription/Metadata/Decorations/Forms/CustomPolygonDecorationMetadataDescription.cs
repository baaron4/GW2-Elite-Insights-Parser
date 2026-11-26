using static GW2EIEvtcParser.EIData.CustomPolygonDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class CustomPolygonDecorationMetadataDescription : FormDecorationMetadataDescription
{

    internal CustomPolygonDecorationMetadataDescription(CustomPolygonDecorationMetadata decoration, TokenHandler colorTokenHandler) : base(decoration, colorTokenHandler)
    {
        Type = Types.CustomPolygon;
    }
}
