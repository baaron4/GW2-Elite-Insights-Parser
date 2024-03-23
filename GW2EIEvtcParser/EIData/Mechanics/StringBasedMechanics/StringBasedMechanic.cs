using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public abstract class StringBasedMechanic<Checkable> : CheckedMechanic<Checkable>
    {

        protected HashSet<string> MechanicIDs { get; } = new HashSet<string>();

        protected StringBasedMechanic(string mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            MechanicIDs.Add(mechanicID);
        }

        protected StringBasedMechanic(string[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown): base(inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            MechanicIDs.UnionWith(mechanicIDs);
        }

    }
}
