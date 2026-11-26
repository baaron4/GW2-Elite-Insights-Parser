using static GW2EIEvtcParser.EIData.RectangleDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class RectangleDecorationMetadataDescription : FormDecorationMetadataDescription
{
    public readonly uint Height;
    public readonly uint Width;

    internal RectangleDecorationMetadataDescription(RectangleDecorationMetadata decoration, TokenHandler colorTokenHandler) : base(decoration, colorTokenHandler)
    {
        Type = Types.Rectangle;
        Width = decoration.Width;
        Height = decoration.Height;
    }
}
