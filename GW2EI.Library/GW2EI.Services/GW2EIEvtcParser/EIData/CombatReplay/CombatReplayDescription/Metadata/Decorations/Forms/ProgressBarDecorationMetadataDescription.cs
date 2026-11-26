using static GW2EIEvtcParser.EIData.ProgressBarDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class ProgressBarDecorationMetadataDescription : RectangleDecorationMetadataDescription
{
    public readonly int SecondaryColor;
    internal ProgressBarDecorationMetadataDescription(ProgressBarDecorationMetadata decoration, TokenHandler colorTokenHandler) : base(decoration, colorTokenHandler)
    {
        Type = Types.ProgressBar;
        SecondaryColor = colorTokenHandler.GetIndex(decoration.SecondaryColor);
    }
}
