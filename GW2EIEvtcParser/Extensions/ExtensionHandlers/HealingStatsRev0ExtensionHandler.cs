using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    public class HealingStatsRev0ExtensionHandler : AbstractExtensionHandler
    {

        private readonly List<EXTAbstractHealingEvent> _healingEvents = new List<EXTAbstractHealingEvent>();

        internal HealingStatsRev0ExtensionHandler(CombatItem c) : base(ExtensionHelper.EXT_HealingStats, "Healing Stats")
        {
            Revision = 0;
        }

        internal HealingStatsRev0ExtensionHandler() : base(ExtensionHelper.EXT_HealingStats, "Healing Stats")
        {
            Revision = 0;
            Version = "Unknown";
        }

        internal override bool HasTime(CombatItem c)
        {
            return true;
        }

        internal override bool SrcIsAgent(CombatItem c)
        {
            if (c.IsBuff != 0 && c.BuffDmg == 0 && c.Value > 0)
            {
                return false;
            }
            else if (c.IsBuff != 0 || c.Value != 0)
            {
                return true;
            }
            return false;
        }
        internal override bool DstIsAgent(CombatItem c)
        {
            if (c.IsBuff != 0 && c.BuffDmg == 0 && c.Value > 0)
            {
                return false;
            }
            else if (c.IsBuff != 0 || c.Value != 0)
            {
                return true;
            }
            return false;
        }

        internal override bool IsDamage(CombatItem c)
        {
            if (c.IsBuff != 0 && c.BuffDmg == 0 && c.Value > 0)
            {
                return false;
            }
            else if (c.IsBuff != 0 || c.Value != 0)
            {
                return true;
            }
            return false;
        }

        internal override void InsertEIExtensionEvent(CombatItem c, AgentData agentData, SkillData skillData)
        {
            if (c.IsBuff != 0 && c.BuffDmg == 0 && c.Value > 0)
            {
                // Buff apply, not sent
            }
            else if (c.IsBuff == 0 && c.Value < 0)
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

        }

    }
}
