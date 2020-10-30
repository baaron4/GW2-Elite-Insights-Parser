using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class ShatteredObservatory : FractalLogic
    {
        public ShatteredObservatory(int triggerID) : base(triggerID)
        {

        }

        protected static void SetSuccessByBuffCount(CombatData combatData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents, NPC target, long buffID, int count)
        {
            if (target == null)
            {
                return;
            }
            List<AbstractBuffEvent> invulsTarget = GetFilteredList(combatData, buffID, target, true);
            if (invulsTarget.Count == count)
            {
                AbstractBuffEvent last = invulsTarget.Last();
                if (!(last is BuffApplyEvent))
                {
                    SetSuccessByCombatExit(new List<NPC> { target }, combatData, fightData, playerAgents);
                }
            }
        }
    }
}
