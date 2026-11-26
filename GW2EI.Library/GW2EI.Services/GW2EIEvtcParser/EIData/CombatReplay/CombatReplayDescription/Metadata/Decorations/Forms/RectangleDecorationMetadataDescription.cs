using static GW2EIEvtcParser.EIData.RectangleDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class RectangleDecorationMetadataDescription : FormDecorationMetadataDescription
{
    public readonly uint Height;
    public readonly uint Width;

    internal RectangleDecorationMetadataDescription(RectangleDecorationMetadata decoration, StringToUlongHandler stringToUlongHandler) : base(decoration, stringToUlongHandler)
    {
        Type = Types.Rectangle;
        Width = decoration.Width;
        Height = decoration.Height;
    }
}
