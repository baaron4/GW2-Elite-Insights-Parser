using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public abstract class IDBasedMechanic<Checkable> : CheckedMechanic<Checkable>
    {

        protected HashSet<long> MechanicIDs { get; } = new HashSet<long>();

        protected IDBasedMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            MechanicIDs.Add(mechanicID);
        }

        protected IDBasedMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            MechanicIDs.UnionWith(mechanicIDs);
        }

    }
}
