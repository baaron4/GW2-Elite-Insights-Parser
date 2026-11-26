using static GW2EIEvtcParser.EIData.LineDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class LineDecorationMetadataDescription : FormDecorationMetadataDescription
{

    public readonly bool WorldSizeThickness;
    public readonly uint Thickness;
    internal LineDecorationMetadataDescription(LineDecorationMetadata decoration, StringToUlongHandler stringToUlongHandler) : base(decoration, stringToUlongHandler)
    {
        Type = Types.Line;
        WorldSizeThickness = decoration.WorldSizeThickness;
        Thickness = decoration.Thickness;
    }
}
