using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class MinionSpawnCastFinder : CheckedCastFinder<SpawnEvent>
    {
        protected int SpeciesID { get; }

        public MinionSpawnCastFinder(long skillID, int speciesID) : base(skillID)
        {
            SpeciesID = speciesID;
        }

        public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var result = new List<InstantCastEvent>();
            long lastTime = int.MinValue;
            foreach (AgentItem agent in agentData.GetNPCsByID(SpeciesID))
            {
                if (agent.Master == null)
                {
                    continue;
                }
                foreach (SpawnEvent spawn in combatData.GetSpawnEvents(agent))
                {
                    if (spawn.Time - lastTime < ICD)
                    {
                        lastTime = spawn.Time;
                    }
                    else if (CheckCondition(spawn, combatData, agentData, skillData))
                    {
                        lastTime = spawn.Time;
                        result.Add(new InstantCastEvent(spawn.Time, skillData.Get(SkillID), agent.GetFinalMaster()));
                    }
                }
            }
            return result;
        }
    }
}
