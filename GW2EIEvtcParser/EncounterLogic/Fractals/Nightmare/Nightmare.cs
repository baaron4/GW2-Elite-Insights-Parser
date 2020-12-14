
using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class Nightmare : FractalLogic
    {
        public Nightmare(int triggerID) : base(triggerID)
        {

        }

        protected static long GetFightOffsetByFirstInvulFilter(FightData fightData, AgentData agentData, List<CombatItem> combatData, int targetID, long invulID, long invulGainOffset)
        {
            // Find target
            AgentItem target = agentData.GetNPCsByID(targetID).FirstOrDefault();
            if (target == null)
            {
                throw new MissingKeyActorsException("Main target of the fight not found");
            }
            // check invul gain at the start of the fight (initial or with a small threshold)
            CombatItem invulGain = combatData.FirstOrDefault(x => x.DstAgent == target.Agent && (x.IsStateChange == ArcDPSEnums.StateChange.None || x.IsStateChange == ArcDPSEnums.StateChange.BuffInitial) && x.IsBuffRemove == ArcDPSEnums.BuffRemove.None && x.IsBuff > 0 && x.SkillID == invulID);
            // get invul lost
            CombatItem invulLost = combatData.FirstOrDefault(x => x.Time >= fightData.LogStart && x.SrcAgent == target.Agent && x.IsStateChange == ArcDPSEnums.StateChange.None && x.IsBuffRemove == ArcDPSEnums.BuffRemove.All && x.SkillID == invulID);
            if (invulGain != null && invulGain.Time - fightData.LogStart < invulGainOffset && invulLost != null && invulLost.Time > invulGain.Time)
            {
                return invulLost.Time + 1;
            }
            else if (invulLost != null)
            {
                CombatItem enterCombat = combatData.FirstOrDefault(x => x.SrcAgent == target.Agent && x.IsStateChange == ArcDPSEnums.StateChange.EnterCombat && Math.Abs(x.Time - invulLost.Time) < ParserHelper.ServerDelayConstant);
                if (enterCombat != null)
                {
                    return enterCombat.Time + 1;
                }
            }
            return fightData.LogStart;
        }
    }
}
