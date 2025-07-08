using static GW2EIEvtcParser.EIData.PolygonDecoration;

namespace GW2EIEvtcParser.EIData;

public class PolygonDecorationMetadataDescription : FormDecorationMetadataDescription
{
    public readonly uint Radius;
    public readonly uint NbPolygon;

    internal PolygonDecorationMetadataDescription(PolygonDecorationMetadata decoration) : base(decoration)
    {
        Type = Types.Polygon;
        Radius = decoration.Radius;
        NbPolygon = decoration.NbPolygon;
    }
}
