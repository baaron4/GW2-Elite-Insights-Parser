using static GW2EIEvtcParser.EIData.Decoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public abstract class DecorationMetadataDescription : CombatReplayMetadataDescription
{

    public readonly ulong Signature;
    internal DecorationMetadataDescription(_DecorationMetadata decoration, StringToUlongHandler stringToUlongHandler) : base()
    {
        Signature = stringToUlongHandler.GetSignatureIndex(decoration.GetSignature());
    }
}
