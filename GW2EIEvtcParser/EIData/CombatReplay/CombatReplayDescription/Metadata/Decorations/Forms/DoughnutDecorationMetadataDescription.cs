using static GW2EIEvtcParser.EIData.DoughnutDecoration;

namespace GW2EIEvtcParser.EIData;

public class DoughnutDecorationMetadataDescription : FormDecorationMetadataDescription
{
    public readonly uint InnerRadius;
    public readonly uint OuterRadius;

    internal DoughnutDecorationMetadataDescription(DoughnutDecorationMetadata decoration) : base(decoration)
    {
        Type = Types.Doughnut;
        OuterRadius = decoration.OuterRadius;
        InnerRadius = decoration.InnerRadius;
    }

}
