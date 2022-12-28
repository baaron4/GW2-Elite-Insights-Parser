using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class StrikeMissionLogic : FightLogic
    {

        protected StrikeMissionLogic(int triggerID) : base(triggerID)
        {
            Mode = ParseMode.Instanced10;
            EncounterCategoryInformation.Category = FightCategory.Strike;
            EncounterID |= EncounterIDs.EncounterMasks.StrikeMask;
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                GenericTriggerID
            };
        }
    }
}
