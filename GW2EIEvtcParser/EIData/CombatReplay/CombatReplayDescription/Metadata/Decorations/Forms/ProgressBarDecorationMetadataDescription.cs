using static GW2EIEvtcParser.EIData.ProgressBarDecoration;

namespace GW2EIEvtcParser.EIData;

public class ProgressBarDecorationMetadataDescription : RectangleDecorationMetadataDescription
{

    public readonly string SecondaryColor;
    internal ProgressBarDecorationMetadataDescription(ProgressBarDecorationMetadata decoration) : base(decoration)
    {
        Type = "ProgressBar";
        SecondaryColor = decoration.SecondaryColor;
    }
}
