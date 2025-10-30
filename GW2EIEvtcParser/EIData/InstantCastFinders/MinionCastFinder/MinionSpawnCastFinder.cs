using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class MinionSpawnCastFinder : CheckedCastFinder<SpawnEvent>
{
    protected readonly IReadOnlyList<int> SpeciesIDs;

    public MinionSpawnCastFinder(long skillID, int speciesID) : base(skillID)
    {
        SpeciesIDs = [speciesID];
    }

    public MinionSpawnCastFinder(long skillID, IEnumerable<int> speciesIDs) : base(skillID)
    {
        SpeciesIDs = new List<int>(speciesIDs);
    }

    public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
    {
        //TODO_PERF(Rennorb)
        var result = new List<InstantCastEvent>(10);
        //TODO_PERF(Rennorb)
        var minions = new List<AgentItem>(10);
        foreach (int id in SpeciesIDs)
        {
            minions.AddRange(agentData.GetNPCsByID(id).Where(m => m.Master != null));
        }
        Tracing.Trace.TrackAverageStat("minions", minions.Count);
        minions.SortByFirstAware();

        foreach (var minionsByMaster in minions.GroupBy(x => x.GetFinalMaster()))
        {
            long lastTime = int.MinValue;
            foreach (AgentItem minion in minionsByMaster)
            {
                foreach (SpawnEvent spawn in combatData.GetSpawnEvents(minion))
                {
                    if (CheckCondition(spawn, combatData, agentData, skillData))
                    {
                        if (spawn.Time - lastTime < ICD)
                        {
                            lastTime = spawn.Time;
                            continue;
                        }
                        lastTime = spawn.Time;
                        result.Add(new InstantCastEvent(spawn.Time, skillData.Get(SkillID), minionsByMaster.Key));
                    }
                }
            }
        }

        Tracing.Trace.TrackAverageStat("result", result.Count);
        return result;
    }
}
