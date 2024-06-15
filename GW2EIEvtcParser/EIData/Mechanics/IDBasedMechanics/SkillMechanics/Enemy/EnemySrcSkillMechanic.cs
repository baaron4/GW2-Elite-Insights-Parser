using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class EnemySrcSkillMechanic : EnemySkillMechanic
    {

        public EnemySrcSkillMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public EnemySrcSkillMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }
        protected override AgentItem GetAgentItem(AbstractHealthDamageEvent ahde)
        {
            return ahde.From;
        }
    }
}
