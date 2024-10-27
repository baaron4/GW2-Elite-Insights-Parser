using static GW2EIEvtcParser.EIData.RectangleDecoration;

namespace GW2EIEvtcParser.EIData;

public class RectangleDecorationMetadataDescription : FormDecorationMetadataDescription
{
    public uint Height { get; }
    public uint Width { get; }

    internal RectangleDecorationMetadataDescription(RectangleDecorationMetadata decoration) : base(decoration)
    {
        Type = "Rectangle";
        Width = decoration.Width;
        Height = decoration.Height;
    }
}
