using static GW2EIEvtcParser.EIData.TextDecoration;

namespace GW2EIEvtcParser.EIData;

public class TextDecorationMetadataDescription : DecorationMetadataDescription
{

    public readonly string Color;

    public readonly string? BackgroundColor;
    internal TextDecorationMetadataDescription(TextDecorationMetadata decoration) : base(decoration)
    {
        Type = Types.Text;
        Color = decoration.Color;
        BackgroundColor = decoration.BackgroundColor;
    }
}
