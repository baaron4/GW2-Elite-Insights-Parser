using static GW2EIEvtcParser.EIData.CustomPolygonDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class CustomPolygonDecorationMetadataDescription : FormDecorationMetadataDescription
{

    internal CustomPolygonDecorationMetadataDescription(CustomPolygonDecorationMetadata decoration, ColorTokenHandler colorTokenHandler) : base(decoration, colorTokenHandler)
    {
        Type = Types.CustomPolygon;
    }
}
