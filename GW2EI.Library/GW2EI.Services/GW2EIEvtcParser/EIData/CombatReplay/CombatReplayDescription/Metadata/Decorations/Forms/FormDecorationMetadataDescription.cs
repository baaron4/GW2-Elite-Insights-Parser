using static GW2EIEvtcParser.EIData.FormDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public abstract class FormDecorationMetadataDescription : AttachedDecorationMetadataDescription
{
    public readonly ulong Color;

    internal FormDecorationMetadataDescription(FormDecorationMetadata decoration, StringToUlongHandler stringToUlongHandler) : base(decoration, stringToUlongHandler)
    {
        Color = stringToUlongHandler.GetColorIndex(decoration.Color);
    }

}
