using static GW2EIEvtcParser.EIData.PieDecoration;

namespace GW2EIEvtcParser.EIData;

public class PieDecorationMetadataDescription : CircleDecorationMetadataDescription
{
    public float OpeningAngle { get; set; }

    internal PieDecorationMetadataDescription(PieDecorationMetadata decoration) : base(decoration)
    {
        Type = Types.Pie;
        OpeningAngle = decoration.OpeningAngle;
    }

}
