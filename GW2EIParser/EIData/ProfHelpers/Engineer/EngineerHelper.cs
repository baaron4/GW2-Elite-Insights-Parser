using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public class EngineerHelper : ProfHelper
    {

        private static HashSet<AgentItem> GetOffensiveTurretAgents(Dictionary<long, List<AbstractBuffEvent>> buffData, long id, HashSet<AgentItem> playerAgents)
        {
            return new HashSet<AgentItem>();
        }

        public static void AttachMasterToTurrets(List<Player> players, Dictionary<long, List<AbstractBuffEvent>> buffData, Dictionary<long, List<AbstractCastEvent>> castData)
        {
        }

    }
}
