using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class SubMechanic : Mechanic
{
    /// <summary>
    /// Full constructor without special checks
    /// </summary>
    /// <param name="id">Unique id of the mechanic</param>
    /// <param name="plotlySetting">html plot settings <seealso cref="MechanicPlotlySetting"/></param>
    /// <param name="description">description of the mechanic</param>
    /// <param name="internalCoolDown">grace period, in ms, during which getting hit by the mechanic does not count</param>
    public SubMechanic(int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        throw new InvalidOperationException("Submechanic can't be checked, the check should be on the parent mechanic");
    }
}
