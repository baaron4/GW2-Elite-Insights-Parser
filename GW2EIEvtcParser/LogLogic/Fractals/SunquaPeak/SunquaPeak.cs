using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.LogLogic.LogCategories;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class SunquaPeak : FractalLogic
{
    public SunquaPeak(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.SunquaPeak;
        LogID |= LogIDs.FractalMasks.SunquaPeakMask;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        // Set manual FractalScale for old logs without the event
        AddFractalScaleEvent(gw2Build, combatData, new List<(ulong, byte)>
        {
            ( GW2Builds.September2020SunquaPeakRelease, 100),
            ( GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM, 99),
        });
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }
}
