using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public class NPCCombatReplayDescription : SingleActorCombatReplayDescription
{
    public readonly int MasterID;

    internal NPCCombatReplayDescription(NPC npc, ParsedEvtcLog log, CombatReplayMap map, CombatReplay replay) : base(npc, log, map, replay)
    {
        AgentItem master = npc.AgentItem.GetFinalMaster();
        // Don't put minions of NPC or unknown minions into the minion display system
        if (!master.Is(npc.AgentItem) && master.IsPlayer && ParserHelper.IsKnownMinionID(npc.AgentItem, master.Spec))
        {
            MasterID = master.UniqueID;
        }
    }
}
