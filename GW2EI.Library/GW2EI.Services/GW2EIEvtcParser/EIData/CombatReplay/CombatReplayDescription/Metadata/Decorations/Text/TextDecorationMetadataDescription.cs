using static GW2EIEvtcParser.EIData.TextDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class TextDecorationMetadataDescription : DecorationMetadataDescription
{
    public readonly ulong Color;

    public readonly ulong? BackgroundColor;
    internal TextDecorationMetadataDescription(TextDecorationMetadata decoration, StringToUlongHandler stringToUlongHandler) : base(decoration, stringToUlongHandler)
    {
        Type = Types.Text;
        Color = stringToUlongHandler.GetColorIndex(decoration.Color);
        BackgroundColor = decoration.BackgroundColor == null ? null : stringToUlongHandler.GetColorIndex(decoration.BackgroundColor);
    }
}
