using static GW2EIEvtcParser.EIData.ProgressBarDecoration;

namespace GW2EIEvtcParser.EIData;

public class ProgressBarDecorationMetadataDescription : RectangleDecorationMetadataDescription
{

    public string SecondaryColor { get; private set; }
    internal ProgressBarDecorationMetadataDescription(ProgressBarDecorationMetadata decoration) : base(decoration)
    {
        Type = Types.ProgressBar;
        SecondaryColor = decoration.SecondaryColor;
    }

    public override void ColorToToken(Dictionary<string, string> colorMap)
    {
        base.ColorToToken(colorMap);
        SecondaryColor = GetToken(SecondaryColor, colorMap);
    }
}
