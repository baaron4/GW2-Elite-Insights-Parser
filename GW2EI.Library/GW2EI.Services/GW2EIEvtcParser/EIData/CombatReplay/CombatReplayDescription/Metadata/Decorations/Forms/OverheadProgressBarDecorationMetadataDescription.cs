using static GW2EIEvtcParser.EIData.OverheadProgressBarDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class OverheadProgressBarDecorationMetadataDescription : ProgressBarDecorationMetadataDescription
{
    public readonly uint PixelWidth;
    public readonly uint PixelHeight;
    internal OverheadProgressBarDecorationMetadataDescription(OverheadProgressBarDecorationMetadata decoration, TokenHandler colorTokenHandler) : base(decoration, colorTokenHandler)
    {
        Type = Types.ProgressBarOverhead;
        PixelWidth = decoration.PixelWidth;
        PixelHeight = decoration.PixelHeight;
    }
}
