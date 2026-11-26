using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EIData.Decoration;

namespace GW2EIEvtcParser.EIData;

public abstract class DecorationMetadataDescription : CombatReplayMetadataDescription
{

    public readonly ulong Signature;
    internal DecorationMetadataDescription(_DecorationMetadata decoration) : base()
    {
        Signature = decoration.GetSignature().HashCyrb53();
    }
}
