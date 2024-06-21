using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class EnemyDstBuffRemoveSingleMechanic : EnemyBuffRemoveSingleMechanic
    {

        public EnemyDstBuffRemoveSingleMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName)
        {
            IsEnemyMechanic = true;
        }

        public EnemyDstBuffRemoveSingleMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName)
        {
            IsEnemyMechanic = true;
        }

        protected override AgentItem GetAgentItem(AbstractBuffRemoveEvent rae)
        {
            return rae.To;
        }
    }
}
