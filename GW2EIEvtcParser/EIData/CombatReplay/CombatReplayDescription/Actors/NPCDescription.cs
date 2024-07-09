using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class NPCDescription : AbstractSingleActorCombatReplayDescription
    {
        public int MasterID { get; }

        internal NPCDescription(NPC npc, ParsedEvtcLog log, CombatReplayMap map, CombatReplay replay) : base(npc, log, map, replay)
        {
            if (log.FriendlyAgents.Contains(npc.AgentItem))
            {
                SetStatus(log, npc);
            }
            SetBreakbarStatus(log, npc);
            AgentItem master = npc.AgentItem.GetFinalMaster();
            // Don't put minions of NPC into the minion display system
            if (master != npc.AgentItem && master.IsPlayer)
            {
                MasterID = master.UniqueID;
            }
        }
    }
}
