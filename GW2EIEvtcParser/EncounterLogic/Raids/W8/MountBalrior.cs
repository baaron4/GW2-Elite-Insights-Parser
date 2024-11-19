
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic;

internal abstract class MountBalrior : RaidLogic
{
    public MountBalrior(int triggerID) : base(triggerID)
    {
        EncounterCategoryInformation.SubCategory = SubFightCategory.MountBalrior;
        EncounterID |= EncounterIDs.RaidWingMasks.MountBalriorMask;
    }
}
