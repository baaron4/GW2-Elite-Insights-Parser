using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class FestivalStrikeMissionLogic : StrikeMissionLogic
    {

        protected FestivalStrikeMissionLogic(int triggerID) : base(triggerID)
        {
            EncounterID |= EncounterIDs.StrikeMasks.FestivalMask;
        }
    }
}
