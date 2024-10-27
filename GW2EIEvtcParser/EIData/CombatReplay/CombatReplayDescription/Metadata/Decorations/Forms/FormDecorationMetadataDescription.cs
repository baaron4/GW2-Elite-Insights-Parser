using static GW2EIEvtcParser.EIData.FormDecoration;

namespace GW2EIEvtcParser.EIData;

public abstract class FormDecorationMetadataDescription : GenericAttachedDecorationMetadataDescription
{
    public string Color { get; }

    internal FormDecorationMetadataDescription(FormDecorationMetadata decoration) : base(decoration)
    {
        Color = decoration.Color;
    }

}
