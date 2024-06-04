using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using System.Collections.Generic;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class LonelyTower : FractalLogic
    {
        public LonelyTower(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.SubCategory = SubFightCategory.LonelyTower;
            EncounterID |= EncounterIDs.FractalMasks.LonelyTowerMask;
        }
    }
}
