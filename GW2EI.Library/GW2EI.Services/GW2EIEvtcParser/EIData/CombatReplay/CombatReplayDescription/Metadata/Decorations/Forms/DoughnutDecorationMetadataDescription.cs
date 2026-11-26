using static GW2EIEvtcParser.EIData.DoughnutDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class DoughnutDecorationMetadataDescription : FormDecorationMetadataDescription
{
    public readonly uint InnerRadius;
    public readonly uint OuterRadius;

    internal DoughnutDecorationMetadataDescription(DoughnutDecorationMetadata decoration, TokenHandler colorTokenHandler) : base(decoration, colorTokenHandler)
    {
        Type = Types.Doughnut;
        OuterRadius = decoration.OuterRadius;
        InnerRadius = decoration.InnerRadius;
    }

}
