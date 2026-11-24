using static GW2EIEvtcParser.EIData.FormDecoration;

namespace GW2EIEvtcParser.EIData;

public abstract class FormDecorationMetadataDescription : AttachedDecorationMetadataDescription
{
    public string Color { get; private set; }

    internal FormDecorationMetadataDescription(FormDecorationMetadata decoration) : base(decoration)
    {
        Color = decoration.Color;
    }

    public override void ColorToToken(Dictionary<string, string> colorMap)
    {
        Color = GetToken(Color, colorMap);
    }

}
