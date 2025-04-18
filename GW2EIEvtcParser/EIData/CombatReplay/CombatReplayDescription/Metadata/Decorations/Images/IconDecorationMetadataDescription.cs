using static GW2EIEvtcParser.EIData.IconDecoration;

namespace GW2EIEvtcParser.EIData;

public class IconDecorationMetadataDescription : ImageDecorationMetadataDescription
{
    public readonly float Opacity;

    internal IconDecorationMetadataDescription(IconDecorationMetadata decoration) : base(decoration)
    {
        Type = Types.Icon;
        Opacity = decoration.Opacity;
    }
}
