using System.Collections.Generic;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.Builders.HtmlModels
{
    public class DamageModData
    {
        public List<object[]> Data { get; set; } = new List<object[]>();
        public List<List<object[]>> DataTarget { get; set; } = new List<List<object[]>>();

        public DamageModData(Player player, ParsedLog log, List<DamageModifier> listToUse, int phaseIndex)
        {
            Dictionary<string, List<Player.DamageModifierData>> dModData = player.GetDamageModifierData(log, null);
            List<PhaseData> phases = log.FightData.GetPhases(log);
            foreach (DamageModifier dMod in listToUse)
            {
                if (dModData.TryGetValue(dMod.Name, out List<Player.DamageModifierData> list))
                {
                    Player.DamageModifierData data = list[phaseIndex];
                    Data.Add(new object[]
                    {
                        data.HitCount,
                        data.TotalHitCount,
                        data.DamageGain,
                        dMod.Multiplier ? data.TotalDamage : -1
                    });
                }
                else
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
            foreach (NPC target in phase.Targets)
            {
                var pTarget = new List<object[]>();
                DataTarget.Add(pTarget);
                dModData = player.GetDamageModifierData(log, target);
                foreach (DamageModifier dMod in listToUse)
                {
                    if (dModData.TryGetValue(dMod.Name, out List<Player.DamageModifierData> list))
                    {
                        Player.DamageModifierData data = list[phaseIndex];
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
