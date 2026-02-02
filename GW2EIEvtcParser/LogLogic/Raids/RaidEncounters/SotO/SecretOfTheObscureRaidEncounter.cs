using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class SecretOfTheObscureRaidEncounter : PostEoDRaidEncounterLogic
{
    public SecretOfTheObscureRaidEncounter(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.SotO;
        LogID |= LogIDs.RaidEncounterMasks.SotOMask;
    }
}
