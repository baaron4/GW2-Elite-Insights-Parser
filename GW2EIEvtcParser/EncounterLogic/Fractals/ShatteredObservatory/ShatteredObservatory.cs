using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class ShatteredObservatory : FractalLogic
    {
        public ShatteredObservatory(int triggerID) : base(triggerID)
        {

        }

        protected static HashSet<AgentItem> GetParticipatingPlayerAgents(NPC target, CombatData combatData, HashSet<AgentItem> playerAgents)
        {
            if (target == null)
            {
                return new HashSet<AgentItem>();
            }
            var participatingPlayerAgents = new HashSet<AgentItem>(combatData.GetDamageTakenData(target.AgentItem).Where(x => playerAgents.Contains(x.From.GetFinalMaster())).Select(x => x.From.GetFinalMaster()));
            participatingPlayerAgents.UnionWith(combatData.GetDamageData(target.AgentItem).Where(x => playerAgents.Contains(x.To.GetFinalMaster())).Select(x => x.From.GetFinalMaster()));
            return participatingPlayerAgents;
        }

        /// <summary>
        /// Returns true if the buff count was not reached so that another method can be called, if necessary
        /// </summary>
        protected static bool SetSuccessByBuffCount(CombatData combatData, FightData fightData, HashSet<AgentItem> playerAgents, NPC target, long buffID, int count)
        {
            if (target == null)
            {
                return false;
            }
            List<AbstractBuffEvent> invulsTarget = GetFilteredList(combatData, buffID, target, true);
            if (invulsTarget.Count == count)
            {
                AbstractBuffEvent last = invulsTarget.Last();
                if (!(last is BuffApplyEvent))
                {
                    SetSuccessByCombatExit(new List<NPC> { target }, combatData, fightData, playerAgents);
                    return false;
                }
            }
            return true;
        }
    }
}
