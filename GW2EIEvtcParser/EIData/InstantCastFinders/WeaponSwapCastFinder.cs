using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class WeaponSwapCastFinder : CheckedCastFinder<WeaponSwapEvent>
    {
        private readonly long _swappedTo;
        public WeaponSwapCastFinder(long skillID, long swappedTo) : base(skillID)
        {
            _swappedTo = swappedTo;
            BeforeWeaponSwap = true;
        }

        internal override InstantCastFinder UsingBeforeWeaponSwap(bool beforeWeaponSwap)
        {
            throw new InvalidOperationException("Invalid WeaponSwapCastFinder usage, always before swap");
        }

        public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var res = new List<InstantCastEvent>();
            foreach (AgentItem playerAgent in agentData.GetAgentByType(AgentItem.AgentType.Player))
            {
                IReadOnlyList<WeaponSwapEvent> swaps = combatData.GetWeaponSwapData(playerAgent);
                long lastTime = int.MinValue;
                foreach (WeaponSwapEvent swap in swaps)
                {
                    if (swap.SwappedTo != _swappedTo)
                    {
                        continue;
                    }
                    if (swap.Time - lastTime < ICD)
                    {
                        lastTime = swap.Time;
                        continue;
                    }
                    if (CheckCondition(swap, combatData, agentData, skillData))
                    {
                        lastTime = swap.Time;
                        res.Add(new InstantCastEvent(GetTime(swap, swap.Caster, combatData), skillData.Get(SkillID), swap.Caster));
                    }
                }
            }
            return res;
        }
    }
}
