using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal class MinionCommandCastFinder : BuffGainCastFinder
    {
        protected int SpeciesID { get; }

        protected override AgentItem GetCasterAgent(AgentItem agent)
        {
            return agent.GetFinalMaster();
        }

        public MinionCommandCastFinder(long skillID, int speciesID) : base(skillID, MinionCommandBuff)
        {
            SpeciesID = speciesID;
            UsingChecker((evt, combatData, agentData, skillData) => evt.To.Type != AgentItem.AgentType.Gadget && evt.To.IsSpecies(speciesID) && evt.To.Master != null);
        }
    }
}
