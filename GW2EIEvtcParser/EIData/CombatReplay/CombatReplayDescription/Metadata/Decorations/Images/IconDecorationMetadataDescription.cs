using static GW2EIEvtcParser.EIData.IconDecoration;

namespace GW2EIEvtcParser.EIData;

public class IconDecorationMetadataDescription : GenericIconDecorationMetadataDescription
{
    public readonly float Opacity;

    internal IconDecorationMetadataDescription(IconDecorationMetadata decoration) : base(decoration)
    {
        Type = "IconDecoration";
        Opacity = decoration.Opacity;
    }
}
