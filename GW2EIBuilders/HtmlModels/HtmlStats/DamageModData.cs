using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels
{
    internal class DamageModData
    {
        public List<object[]> Data { get; } = new List<object[]>();
        public List<List<object[]>> DataTarget { get; } = new List<List<object[]>>();

        private DamageModData(Player player, ParsedEvtcLog log, List<DamageModifier> listToUse, PhaseData phase)
        {
            IReadOnlyDictionary<string, DamageModifierStat> dModData = player.GetDamageModifierStats(null, log, phase.Start, phase.End);
            foreach (DamageModifier dMod in listToUse)
            {
                if (dModData.TryGetValue(dMod.Name, out DamageModifierStat data))
                {
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
                        dMod.GetHitDamageEvents(player, log, null, phase.Start, phase.End).Count,
                        0,
                        dMod.GetTotalDamage(player, log, null, phase.Start, phase.End)
                    });
                }
            }
            foreach (NPC target in phase.Targets)
            {
                var pTarget = new List<object[]>();
                DataTarget.Add(pTarget);
                dModData = player.GetDamageModifierStats(target, log, phase.Start, phase.End);
                foreach (DamageModifier dMod in listToUse)
                {
                    if (dModData.TryGetValue(dMod.Name, out DamageModifierStat data))
                    {
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
                            dMod.GetHitDamageEvents(player, log, target, phase.Start, phase.End).Count,
                            0,
                            dMod.GetTotalDamage(player, log, target, phase.Start, phase.End)
                        });
                    }
                }
            }
        }
        public static List<DamageModData> BuildDmgModifiersData(ParsedEvtcLog log, PhaseData phase, List<DamageModifier> damageModsToUse)
        {
            var pData = new List<DamageModData>();
            foreach (Player player in log.PlayerList)
            {
                pData.Add(new DamageModData(player, log, damageModsToUse, phase));
            }
            return pData;
        }

        public static List<DamageModData> BuildPersonalDmgModifiersData(ParsedEvtcLog log, PhaseData phase, Dictionary<string, List<DamageModifier>> damageModsToUse)
        {
            var pData = new List<DamageModData>();
            foreach (Player player in log.PlayerList)
            {
                pData.Add(new DamageModData(player, log, damageModsToUse[player.Prof], phase));
            }
            return pData;
        }
    }
}
