using System.Text.Json.Serialization;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public class NPCCombatReplayDescription : SingleActorCombatReplayDescription
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public readonly int? MasterID;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public readonly string? MinionDtoID;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public readonly bool MasterIsPlayer;

    internal NPCCombatReplayDescription(NPC npc, ParsedEvtcLog log, CombatReplayMap map, CombatReplay replay) : base(npc, log, map, replay)
    {
        AgentItem master = npc.AgentItem.GetFinalMaster();
        if (!master.Is(npc.AgentItem))
        {
            MasterID = master.EnglobingAgentItem.UniqueID;
            MinionDtoID = (npc.AgentItem.IsVolatileSpecies() ? "v" : "s") + npc.ID;
            // Don't put minions of NPC or unknown minions into the minion display system
            if (master.IsPlayer && ParserHelper.IsKnownMinionID(npc.AgentItem, master.Spec))
            {
                MasterIsPlayer = true;
            }
        }
    }
}
