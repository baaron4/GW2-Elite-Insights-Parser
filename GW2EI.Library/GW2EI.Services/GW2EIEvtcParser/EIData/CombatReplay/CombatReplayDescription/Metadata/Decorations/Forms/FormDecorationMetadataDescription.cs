using static GW2EIEvtcParser.EIData.FormDecoration;

namespace GW2EIEvtcParser.EIData;

public abstract class FormDecorationMetadataDescription : AttachedDecorationMetadataDescription
{
    public readonly int Color;

    internal FormDecorationMetadataDescription(FormDecorationMetadata decoration, ParsedEvtcLog.ColorTokenHandler colorTokenHandler) : base(decoration)
    {
        Color = colorTokenHandler.GetColorIndex(decoration.Color);
    }

}
