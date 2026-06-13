using static GW2EIEvtcParser.EIData.CustomPolygonDecoration;

namespace GW2EIEvtcParser.EIData;

public class CustomPolygonDecorationMetadataDescription : FormDecorationMetadataDescription
{

    internal CustomPolygonDecorationMetadataDescription(CustomPolygonDecorationMetadata decoration) : base(decoration)
    {
        Type = Types.CustomPolygon;
    }
}
