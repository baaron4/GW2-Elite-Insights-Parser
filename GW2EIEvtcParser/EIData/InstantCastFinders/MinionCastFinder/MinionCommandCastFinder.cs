using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal class MinionCommandCastFinder : BuffGainCastFinder
{
    protected readonly int SpeciesID;

    public override BuffCastFinder<BuffApplyEvent> WithMinions()
    {
        throw new InvalidOperationException("BuffGiveCastFinder is always with minions");
    }

    public MinionCommandCastFinder(long skillID, int speciesID) : base(skillID, MinionCommandBuff)
    {
        SpeciesID = speciesID;
        Minions = true;
        UsingChecker((evt, combatData, agentData, skillData) => evt.To.Type != AgentItem.AgentType.Gadget && evt.To.IsSpecies(speciesID) && evt.To.Master != null);
    }
}
