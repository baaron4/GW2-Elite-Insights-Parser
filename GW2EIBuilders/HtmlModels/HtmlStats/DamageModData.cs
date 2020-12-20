using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels
{
    internal class DamageModData
    {
        public List<object[]> Data { get; } = new List<object[]>();
        public List<List<object[]>> DataTarget { get; } = new List<List<object[]>>();

        private DamageModData(Player player, ParsedEvtcLog log, List<DamageModifier> listToUse, int phaseIndex)
        {
            Dictionary<string, List<DamageModifierStat>> dModData = player.GetDamageModifierStats(log, null);
            IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
            foreach (DamageModifier dMod in listToUse)
            {
                if (dModData.TryGetValue(dMod.Name, out List<DamageModifierStat> list))
                {
                    DamageModifierStat data = list[phaseIndex];
                    Data.Add(new object[]
                    {
                        data.HitCount,
                        data.TotalHitCount,
                        data.DamageGain,
                        data.TotalDamage
                    });
                }
                else
                {
                    Data.Add(new object[]
                    {
                        0,
                        dMod.GetHitDamageLogs(player, log, null, phases[phaseIndex]).Count,
                        0,
                        dMod.GetTotalDamage(player, log, null, phaseIndex)
                    });
                }
            }
            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];
            foreach (NPC target in phase.Targets)
            {
                var pTarget = new List<object[]>();
                DataTarget.Add(pTarget);
                dModData = player.GetDamageModifierStats(log, target);
                foreach (DamageModifier dMod in listToUse)
                {
                    if (dModData.TryGetValue(dMod.Name, out List<DamageModifierStat> list))
                    {
                        DamageModifierStat data = list[phaseIndex];
                        pTarget.Add(new object[]
                        {
                            data.HitCount,
                            data.TotalHitCount,
                            data.DamageGain,
                            data.TotalDamage
                        });
                    }
                    else
                    {
                        pTarget.Add(new object[]
                        {
                            0,
                            dMod.GetHitDamageLogs(player, log, target, phases[phaseIndex]).Count,
                            0,
                            dMod.GetTotalDamage(player, log, target, phaseIndex)
                        });
                    }
                }
            }
        }
        public static List<DamageModData> BuildDmgModifiersData(ParsedEvtcLog log, int phaseIndex, List<DamageModifier> damageModsToUse)
        {
            var pData = new List<DamageModData>();
            foreach (Player player in log.PlayerList)
            {
                pData.Add(new DamageModData(player, log, damageModsToUse, phaseIndex));
            }
            return pData;
        }

        public static List<DamageModData> BuildPersonalDmgModifiersData(ParsedEvtcLog log, int phaseIndex, Dictionary<string, List<DamageModifier>> damageModsToUse)
        {
            var pData = new List<DamageModData>();
            foreach (Player player in log.PlayerList)
            {
                pData.Add(new DamageModData(player, log, damageModsToUse[player.Prof], phaseIndex));
            }
            return pData;
        }
    }
}
