using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class SubMechanic : Mechanic
{
    /// <summary>
    /// Full constructor without special checks
    /// </summary>
    /// <param name="inGameName">official name of the mechanic</param>
    /// <param name="plotlySetting">html plot settings <seealso cref="MechanicPlotlySetting"/></param>
    /// <param name="shortName">shortened name of the mechanic</param>
    /// <param name="description">description of the mechanic</param>
    /// <param name="fullName">full name of the mechanic</param>
    /// <param name="internalCoolDown">grace period, in ms, during which getting hit by the mechanic does not count</param>
    public SubMechanic(MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        throw new InvalidOperationException("Submechanic can't be checked, the check should be on the parent mechanic");
    }
}
