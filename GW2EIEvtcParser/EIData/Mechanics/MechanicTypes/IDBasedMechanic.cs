using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public abstract class IDBasedMechanic : Mechanic
    {

        protected HashSet<long> MechanicIDs { get; } = new HashSet<long>();

        protected IDBasedMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(mechanicID, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }
        protected IDBasedMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown): base(inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            MechanicIDs.Add(mechanicID);
        }

    }
}
