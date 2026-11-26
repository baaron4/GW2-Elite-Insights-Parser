using static GW2EIEvtcParser.EIData.RegularPolygonDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class RegularPolygonDecorationMetadataDescription : FormDecorationMetadataDescription
{
    public readonly uint Radius;
    public readonly uint NbPolygon;

    internal RegularPolygonDecorationMetadataDescription(RegularPolygonDecorationMetadata decoration, StringToUlongHandler stringToUlongHandler) : base(decoration, stringToUlongHandler)
    {
        Type = Types.RegularPolygon;
        Radius = decoration.Radius;
        NbPolygon = decoration.NbPolygon;
    }
}
