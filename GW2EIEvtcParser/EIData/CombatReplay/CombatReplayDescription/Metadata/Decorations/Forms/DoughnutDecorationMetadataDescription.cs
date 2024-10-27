using static GW2EIEvtcParser.EIData.DoughnutDecoration;

namespace GW2EIEvtcParser.EIData;

public class DoughnutDecorationMetadataDescription : FormDecorationMetadataDescription
{
    public uint InnerRadius { get; }
    public uint OuterRadius { get; }

    internal DoughnutDecorationMetadataDescription(DoughnutDecorationMetadata decoration) : base(decoration)
    {
        Type = "Doughnut";
        OuterRadius = decoration.OuterRadius;
        InnerRadius = decoration.InnerRadius;
    }

}
