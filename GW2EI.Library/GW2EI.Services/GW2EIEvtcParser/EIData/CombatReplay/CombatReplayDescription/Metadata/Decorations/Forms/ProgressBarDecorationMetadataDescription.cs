using static GW2EIEvtcParser.EIData.ProgressBarDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class ProgressBarDecorationMetadataDescription : RectangleDecorationMetadataDescription
{
    public readonly ulong SecondaryColor;
    internal ProgressBarDecorationMetadataDescription(ProgressBarDecorationMetadata decoration, StringToUlongHandler stringToUlongHandler) : base(decoration, stringToUlongHandler)
    {
        Type = Types.ProgressBar;
        SecondaryColor = stringToUlongHandler.GetColorIndex(decoration.SecondaryColor);
    }
}
