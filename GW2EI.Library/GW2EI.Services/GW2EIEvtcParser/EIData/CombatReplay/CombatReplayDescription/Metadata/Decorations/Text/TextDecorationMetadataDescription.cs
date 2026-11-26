using static GW2EIEvtcParser.EIData.TextDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class TextDecorationMetadataDescription : DecorationMetadataDescription
{
    public readonly int Color;

    public readonly int? BackgroundColor;
    internal TextDecorationMetadataDescription(TextDecorationMetadata decoration, TokenHandler colorTokenHandler) : base(decoration)
    {
        Type = Types.Text;
        Color = colorTokenHandler.GetIndex(decoration.Color);
        BackgroundColor = decoration.BackgroundColor == null ? null : colorTokenHandler.GetIndex(decoration.BackgroundColor);
    }
}
