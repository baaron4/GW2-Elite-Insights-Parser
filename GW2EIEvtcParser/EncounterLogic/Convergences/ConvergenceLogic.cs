using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal abstract class ConvergenceLogic : FightLogic
{
    protected ConvergenceLogic(int triggerID) : base(triggerID)
    {
        ParseMode = ParseModeEnum.FullInstance;
        SkillMode = SkillModeEnum.PvE;
        MechanicList.Add(new MechanicGroup(
        [
            
        ]));
        EncounterCategoryInformation.Category = FightCategory.Convergence;
        EncounterID |= EncounterIDs.EncounterMasks.ConvergenceMask;
    }

    protected override IReadOnlyList<TargetID> GetUniqueNPCIDs()
    {
        return new[] { GetTargetID(GenericTriggerID) };
    }
}
