using static GW2EIEvtcParser.EIData.CircleDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class CircleDecorationMetadataDescription : FormDecorationMetadataDescription
{
    public readonly uint Radius;
    public readonly uint MinRadius;

    internal CircleDecorationMetadataDescription(CircleDecorationMetadata decoration, StringToUlongHandler stringToUlongHandler) : base(decoration, stringToUlongHandler)
    {
        Type = Types.Circle;
        Radius = decoration.Radius;
        MinRadius = decoration.MinRadius;
    }
}
