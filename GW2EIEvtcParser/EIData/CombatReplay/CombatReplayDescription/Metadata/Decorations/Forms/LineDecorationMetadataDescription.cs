using static GW2EIEvtcParser.EIData.LineDecoration;

namespace GW2EIEvtcParser.EIData;

public class LineDecorationMetadataDescription : FormDecorationMetadataDescription
{

    public readonly bool WorldSizeThickness;
    public readonly uint Thickness;
    internal LineDecorationMetadataDescription(LineDecorationMetadata decoration) : base(decoration)
    {
        Type = Types.Line;
        WorldSizeThickness = decoration.WorldSizeThickness;
        Thickness = decoration.Thickness;
    }
}
