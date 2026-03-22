using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class EnemyGadgetInteractEndMechanic : EnemyGadgetInteractMechanic
{
    protected override long GetTime(CastEvent evt)
    {
        return evt.EndTime;
    }

    public EnemyGadgetInteractEndMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : this([mechanicID], plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public EnemyGadgetInteractEndMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }
}
