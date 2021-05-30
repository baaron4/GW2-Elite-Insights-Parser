using System.IO;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class PlayerNonSquad : AbstractPlayer
    {

        private static int NonSquadPlayers = 0;
        // Constructors
        internal PlayerNonSquad(AgentItem agent) : base(agent)
        {
            if (agent.Type == AgentItem.AgentType.Player)
            {
                throw new InvalidDataException("Agent is not a squad Player");
            }
            Group = 51;
            Account = "Non Squad Player " + (++NonSquadPlayers);
        }

    }
}
