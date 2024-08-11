using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class MinionSpawnCastFinder : CheckedCastFinder<SpawnEvent>
    {
        protected List<int> SpeciesIDs { get; }

        public MinionSpawnCastFinder(long skillID, int speciesID) : base(skillID)
        {
            SpeciesIDs = new List<int> { speciesID };
        }

        public MinionSpawnCastFinder(long skillID, IList<int> speciesIDs) : base(skillID)
        {
            SpeciesIDs = new List<int>(speciesIDs);
        }

        public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var result = new List<InstantCastEvent>();
            var minions = new List<AgentItem>();
            foreach (int id in SpeciesIDs)
            {
                minions.AddRange(agentData.GetNPCsByID(id));
            }
            minions = minions.Where(x => x.Master != null).OrderBy(x => x.FirstAware).ToList();
            foreach (KeyValuePair<AgentItem, List<AgentItem>> pair in minions.GroupBy(x => x.GetFinalMaster()).ToDictionary(x => x.Key, x => x.ToList()))
            {
                long lastTime = int.MinValue;
                foreach (AgentItem agent in pair.Value)
                {
                    foreach (SpawnEvent spawn in combatData.GetSpawnEvents(agent))
                    {
                        if (CheckCondition(spawn, combatData, agentData, skillData))
                        {
                            if (spawn.Time - lastTime < ICD)
                            {
                                lastTime = spawn.Time;
                                continue;
                            }
                            lastTime = spawn.Time;
                            result.Add(new InstantCastEvent(spawn.Time, skillData.Get(SkillID), pair.Key));
                        }
                    }
                }
            }

            return result;
        }
    }
}
