using static GW2EIEvtcParser.EIData.RectangleDecoration;

namespace GW2EIEvtcParser.EIData;

public class RectangleDecorationMetadataDescription : FormDecorationMetadataDescription
{
    public readonly uint Height;
    public readonly uint Width;

    internal RectangleDecorationMetadataDescription(RectangleDecorationMetadata decoration) : base(decoration)
    {
        Type = Types.Rectangle;
        Width = decoration.Width;
        Height = decoration.Height;
    }
}
