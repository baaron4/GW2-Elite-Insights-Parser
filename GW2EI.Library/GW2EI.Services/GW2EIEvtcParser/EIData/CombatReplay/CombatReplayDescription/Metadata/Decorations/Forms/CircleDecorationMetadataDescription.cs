using static GW2EIEvtcParser.EIData.CircleDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class CircleDecorationMetadataDescription : FormDecorationMetadataDescription
{
    public readonly uint Radius;
    public readonly uint MinRadius;

    internal CircleDecorationMetadataDescription(CircleDecorationMetadata decoration, ColorTokenHandler colorTokenHandler) : base(decoration, colorTokenHandler)
    {
        Type = Types.Circle;
        Radius = decoration.Radius;
        MinRadius = decoration.MinRadius;
    }
}
