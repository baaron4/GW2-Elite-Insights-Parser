using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;


internal abstract class EffectMechanic : StringBasedMechanic<EffectEvent>
{
    private int ForcedSpeciesID = int.MinValue;

    protected abstract AgentItem GetAgentItem(EffectEvent effectEvt, AgentData agentData);

    public EffectMechanic(GUID effect, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : this([ effect ], plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public EffectMechanic(ReadOnlySpan<GUID> effects, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(effects, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
        UsingEnable(log => log.CombatData.HasEffectData);
    }

    protected void PlayerChecker(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs)
    {
        foreach (var guid in MechanicIDs)
        {
            if (log.CombatData.TryGetEffectEventsByGUID(guid, out var effects))
            {
                foreach (EffectEvent effectEvent in effects)
                {
                    AgentItem agentItem = GetAgentItem(effectEvent, log.AgentData);
                    if (log.PlayerAgents.Contains(agentItem) && Keep(effectEvent, log))
                    {
                        InsertMechanic(log, mechanicLogs, effectEvent.Time, log.FindActor(agentItem));
                    }
                }
            }
        }
    }

    protected void EnemyChecker(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        foreach (var guid in MechanicIDs)
        {
            if (log.CombatData.TryGetEffectEventsByGUID(guid, out var effects))
            {
                foreach (EffectEvent effectEvent in effects)
                {
                    AgentItem agentItem = GetAgentItem(effectEvent, log.AgentData);
                    if (agentItem.IsSpecies(TargetID.Environment) && Keep(effectEvent, log))
                    {
                        SingleActor? actor = log.FindActor(agentItem, true);
                        if (actor != null)
                        {
                            InsertMechanic(log, mechanicLogs, effectEvent.Time, actor);
                        }
                    }
                    else
                    {
                        SingleActor? actor = MechanicHelper.FindEnemyActor(log, agentItem, regroupedMobs);
                        if (actor != null && Keep(effectEvent, log))
                        {
                            InsertMechanic(log, mechanicLogs, effectEvent.Time, actor);
                        }
                    }
                }
            }
        }
    }
}
