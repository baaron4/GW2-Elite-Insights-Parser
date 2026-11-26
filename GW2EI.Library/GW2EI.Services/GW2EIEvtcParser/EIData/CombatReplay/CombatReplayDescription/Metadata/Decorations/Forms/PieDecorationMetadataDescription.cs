using static GW2EIEvtcParser.EIData.PieDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class PieDecorationMetadataDescription : CircleDecorationMetadataDescription
{
    public float OpeningAngle { get; set; }

    internal PieDecorationMetadataDescription(PieDecorationMetadata decoration, ColorTokenHandler colorTokenHandler) : base(decoration, colorTokenHandler)
    {
        Type = Types.Pie;
        OpeningAngle = decoration.OpeningAngle;
    }

}
