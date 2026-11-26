using static GW2EIEvtcParser.EIData.FormDecoration;

namespace GW2EIEvtcParser.EIData;

public abstract class FormDecorationMetadataDescription : AttachedDecorationMetadataDescription
{
    public readonly int Color;

    internal FormDecorationMetadataDescription(FormDecorationMetadata decoration, ParsedEvtcLog.TokenHandler colorTokenHandler) : base(decoration)
    {
        Color = colorTokenHandler.GetIndex(decoration.Color);
    }

}
