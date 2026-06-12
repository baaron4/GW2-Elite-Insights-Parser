using static GW2EIEvtcParser.EIData.RegularPolygonDecoration;

namespace GW2EIEvtcParser.EIData;

public class RegularPolygonDecorationMetadataDescription : FormDecorationMetadataDescription
{
    public readonly uint Radius;
    public readonly uint NbPolygon;

    internal RegularPolygonDecorationMetadataDescription(RegularPolygonDecorationMetadata decoration) : base(decoration)
    {
        Type = Types.RegularPolygon;
        Radius = decoration.Radius;
        NbPolygon = decoration.NbPolygon;
    }
}
