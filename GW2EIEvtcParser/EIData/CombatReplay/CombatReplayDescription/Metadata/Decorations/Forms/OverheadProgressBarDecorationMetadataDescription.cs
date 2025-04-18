using static GW2EIEvtcParser.EIData.OverheadProgressBarDecoration;

namespace GW2EIEvtcParser.EIData;

public class OverheadProgressBarDecorationMetadataDescription : ProgressBarDecorationMetadataDescription
{
    public readonly uint PixelWidth;
    public readonly uint PixelHeight;
    internal OverheadProgressBarDecorationMetadataDescription(OverheadProgressBarDecorationMetadata decoration) : base(decoration)
    {
        Type = Types.ProgressBarOverhead;
        PixelWidth = decoration.PixelWidth;
        PixelHeight = decoration.PixelHeight;
    }
}
