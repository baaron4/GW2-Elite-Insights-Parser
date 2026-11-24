using static GW2EIEvtcParser.EIData.TextDecoration;

namespace GW2EIEvtcParser.EIData;

public class TextDecorationMetadataDescription : DecorationMetadataDescription
{

    public string Color { get; private set; }

    public string? BackgroundColor { get; private set; }
    internal TextDecorationMetadataDescription(TextDecorationMetadata decoration) : base(decoration)
    {
        Type = Types.Text;
        Color = decoration.Color;
        BackgroundColor = decoration.BackgroundColor;
    }

    public override void ColorToToken(Dictionary<string, string> colorMap)
    {
        Color = GetToken(Color, colorMap);
        if (BackgroundColor != null)
        {
            BackgroundColor = GetToken(BackgroundColor, colorMap);
        }
    }
}
