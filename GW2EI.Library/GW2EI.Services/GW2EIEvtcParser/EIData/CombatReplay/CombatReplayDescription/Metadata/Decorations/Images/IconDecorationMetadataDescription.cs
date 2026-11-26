using static GW2EIEvtcParser.EIData.IconDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class IconDecorationMetadataDescription : ImageDecorationMetadataDescription
{
    public readonly float Opacity;

    internal IconDecorationMetadataDescription(IconDecorationMetadata decoration, StringToUlongHandler stringToUlongHandler) : base(decoration, stringToUlongHandler)
    {
        Type = Types.Icon;
        Opacity = decoration.Opacity;
    }
}
