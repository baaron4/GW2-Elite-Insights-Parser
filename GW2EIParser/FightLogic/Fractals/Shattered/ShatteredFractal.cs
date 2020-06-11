
using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.Logic
{
    public abstract class ShatteredFractal : FractalLogic
    {
        public ShatteredFractal(int triggerID) : base(triggerID)
        {

        }

        protected static void SetSuccessByBuffCount(CombatData combatData, FightData fightData, HashSet<AgentItem> playerAgents, NPC target, long buffID, int count)
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
