using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal class MinionCommandCastFinder : BuffGainCastFinder
    {
        protected int SpeciesID { get; }

        public MinionCommandCastFinder(long skillID, int speciesID) : base(skillID, MinionCommandEffect)
        {
            SpeciesID = speciesID;
        }

        public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
        {
            return base.ComputeInstantCast(combatData, skillData, agentData)
                .Where(cast => cast.Caster.IsSpecies(SpeciesID))
                .Select(cast => new InstantCastEvent(cast.Time, cast.Skill, cast.Caster.GetFinalMaster()))
                .ToList();
        }
    }
}
