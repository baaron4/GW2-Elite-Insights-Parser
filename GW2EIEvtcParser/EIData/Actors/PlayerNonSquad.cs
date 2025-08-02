using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public class PlayerNonSquad : PlayerActor
{

    private static int NonSquadPlayers = 0;
    // Constructors
    internal PlayerNonSquad(AgentItem agent) : base(agent)
    {
        if (agent.Type == AgentItem.AgentType.Player)
        {
            throw new InvalidDataException("Agent is not a squad Player");
        }
        Character = Spec.ToString() + " pl-" + AgentItem.InstID;
        Account = "Non Squad Player " + (++NonSquadPlayers);
    }

}
