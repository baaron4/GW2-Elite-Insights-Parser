using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    internal class HealingStatsRev1ExtensionHandler : HealingStatsExtensionHandler
    {

        private readonly List<EXTAbstractHealingEvent> _healingEvents = new List<EXTAbstractHealingEvent>();

        private static bool IsHealingEvent(CombatItem c)
        {
            return (c.IsBuff == 0 && c.Value < 0) || (c.IsBuff != 0 && c.Value == 0 && c.BuffDmg < 0);
        }

        internal HealingStatsRev1ExtensionHandler(CombatItem c) : base()
        {
            Revision = 1;
            SetVersion(c);
        }

        internal override bool HasTime(CombatItem c)
        {
            return true;
        }

        internal override bool SrcIsAgent(CombatItem c)
        {
            return IsHealingEvent(c);
        }
        internal override bool DstIsAgent(CombatItem c)
        {
            return IsHealingEvent(c);
        }

        internal override bool IsDamage(CombatItem c)
        {
            return IsHealingEvent(c);
        }

        internal override void InsertEIExtensionEvent(CombatItem c, AgentData agentData, SkillData skillData)
        {
            if (!IsHealingEvent(c))
            {
                return;
            }
            if (c.IsBuff == 0 && c.Value < 0)
            {
                _healingEvents.Add(new EXTDirectHealingEvent(c, agentData, skillData));
            }
            else if (c.IsBuff != 0 && c.Value == 0 && c.BuffDmg < 0)
            {
                _healingEvents.Add(new EXTNonDirectHealingEvent(c, agentData, skillData));
            }
        }

        internal override void AttachToCombatData(CombatData combatData, ParserController operation)
        {
            var healData = _healingEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            var healReceivedData = _healingEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            var healDataById = _healingEvents.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            operation.UpdateProgressWithCancellationCheck(healData.Count + " has the addon running");
            operation.UpdateProgressWithCancellationCheck("Attached " + _healingEvents.Count + " heal events to CombatData");
            combatData.EXTHealingCombatData = new EXTHealingCombatData(healData, healReceivedData, healDataById);
        }

    }
}
