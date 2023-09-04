using System.Collections.Generic;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class StrikeMissionLogic : FightLogic
    {

        protected StrikeMissionLogic(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstBuffApplyMechanic(Infirmity, "Infirmity", new MechanicPlotlySetting(Symbols.Diamond, Colors.DarkPurple), "Infirmity", "Infirmity (Reduced incoming healing)", "Infirmity", 0),
            }
            );
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
