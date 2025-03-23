using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal abstract class BuffRemoveMechanic<T> : IDBasedMechanic<T> where T : AbstractBuffRemoveEvent
{

    public BuffRemoveMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : this([mechanicID], inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public BuffRemoveMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    protected abstract AgentItem GetAgentItem(T brae);
    protected abstract bool TryGetActor(ParsedEvtcLog log, AgentItem agentItem, Dictionary<int, SingleActor> regroupedMobs, [NotNullWhen(true)] out SingleActor? actor);

    protected virtual void AddMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, T brae, SingleActor actor)
    {
        InsertMechanic(log, mechanicLogs, brae.Time, actor);
    }

    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        foreach (long mechanicID in MechanicIDs)
        {
            foreach (BuffEvent c in log.CombatData.GetBuffData(mechanicID))
            {
                if (c is T brae && TryGetActor(log, GetAgentItem(brae), regroupedMobs, out var amp) && Keep(brae, log))
                {
                    AddMechanic(log, mechanicLogs, brae, amp);
                }
            }
        }
    }

}
