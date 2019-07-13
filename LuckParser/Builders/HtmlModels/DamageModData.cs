using LuckParser.EIData;
using LuckParser.Models;
using LuckParser.Parser;
using System.Collections.Generic;

namespace LuckParser.Builders.HtmlModels
{
    public class DamageModData
    {
        public List<object[]> Data = new List<object[]>();
        public List<List<object[]>> DataTarget = new List<List<object[]>>();

        public DamageModData(Player player, ParsedLog log, List<DamageModifier> listToUse, int phaseIndex)
        {
            Dictionary<string, List<Statistics.DamageModifierData>> dModData = player.GetDamageModifierData(log, null);
            List<PhaseData> phases = log.FightData.GetPhases(log);
            foreach (DamageModifier dMod in listToUse)
            {
                if (dModData.TryGetValue(dMod.Name, out var list))
                {
                    Statistics.DamageModifierData data = list[phaseIndex];
                    Data.Add(new object[]
                    {
                        data.HitCount,
                        data.TotalHitCount,
                        data.DamageGain,
                        dMod.Multiplier ? data.TotalDamage : -1
                    });
                } else
                {
                    Data.Add(new object[]
                    {
                        0,
                        dMod.GetDamageLogs(player, log, null, phases[phaseIndex]).Count,
                        0,
                        dMod.Multiplier ? dMod.GetTotalDamage(player, log, null, phaseIndex) : -1
                    });
                }
            }
            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];
            foreach (Target target in phase.Targets)
            {
                List<object[]> pTarget = new List<object[]>();
                DataTarget.Add(pTarget);
                dModData = player.GetDamageModifierData(log, target);
                foreach (DamageModifier dMod in listToUse)
                {
                    if (dModData.TryGetValue(dMod.Name, out var list))
                    {
                        Statistics.DamageModifierData data = list[phaseIndex];
                        pTarget.Add(new object[]
                        {
                            data.HitCount,
                            data.TotalHitCount,
                            data.DamageGain,
                            dMod.Multiplier ? data.TotalDamage : -1
                        });
                    }
                    else
                    {
                        pTarget.Add(new object[]
                        {
                            0,
                            dMod.GetDamageLogs(player, log, target, phases[phaseIndex]).Count,
                            0,
                            dMod.Multiplier ? dMod.GetTotalDamage(player, log, target, phaseIndex) : -1
                        });
                    }
                }
            }
        }
    }
}
