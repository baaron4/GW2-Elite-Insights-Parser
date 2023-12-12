using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIBuilders.HtmlModels.HTMLStats
{
    internal class DamageModData
    {
        public List<object[]> Data { get; } = new List<object[]>();
        public List<List<object[]>> DataTarget { get; } = new List<List<object[]>>();

        private DamageModData(AbstractSingleActor actor, ParsedEvtcLog log, IReadOnlyList<OutgoingDamageModifier> listToUse, PhaseData phase)
        {
            IReadOnlyDictionary<string, DamageModifierStat> dModData = actor.GetOutgoingDamageModifierStats(null, log, phase.Start, phase.End);
            foreach (OutgoingDamageModifier dMod in listToUse)
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
                        dMod.GetHitDamageEvents(actor, log, null, phase.Start, phase.End).Count,
                        0,
                        dMod.GetTotalDamage(actor, log, null, phase.Start, phase.End)
                    });
                }
            }
            foreach (AbstractSingleActor target in phase.AllTargets)
            {
                var pTarget = new List<object[]>();
                DataTarget.Add(pTarget);
                dModData = actor.GetOutgoingDamageModifierStats(target, log, phase.Start, phase.End);
                foreach (OutgoingDamageModifier dMod in listToUse)
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
                            dMod.GetHitDamageEvents(actor, log, target, phase.Start, phase.End).Count,
                            0,
                            dMod.GetTotalDamage(actor, log, target, phase.Start, phase.End)
                        });
                    }
                }
            }
        }
        private DamageModData(AbstractSingleActor actor, ParsedEvtcLog log, IReadOnlyList<IncomingDamageModifier> listToUse, PhaseData phase)
        {
            IReadOnlyDictionary<string, DamageModifierStat> dModData = actor.GetIncomingDamageModifierStats(null, log, phase.Start, phase.End) ;
            foreach (IncomingDamageModifier dMod in listToUse)
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
                        dMod.GetHitDamageEvents(actor, log, null, phase.Start, phase.End).Count,
                        0,
                        dMod.GetTotalDamage(actor, log, null, phase.Start, phase.End)
                    });
                }
            }
            foreach (AbstractSingleActor target in phase.Targets)
            {
                var pTarget = new List<object[]>();
                DataTarget.Add(pTarget);
                dModData = actor.GetIncomingDamageModifierStats(target, log, phase.Start, phase.End);
                foreach (IncomingDamageModifier dMod in listToUse)
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
                            dMod.GetHitDamageEvents(actor, log, target, phase.Start, phase.End).Count,
                            0,
                            dMod.GetTotalDamage(actor, log, target, phase.Start, phase.End)
                        });
                    }
                }
            }
        }
        public static List<DamageModData> BuildOutgoingDmgModifiersData(ParsedEvtcLog log, PhaseData phase, IReadOnlyList<OutgoingDamageModifier> damageModsToUse)
        {
            var pData = new List<DamageModData>();
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                pData.Add(new DamageModData(actor, log, damageModsToUse, phase));
            }
            return pData;
        }

        public static List<DamageModData> BuildIncomingDmgModifiersData(ParsedEvtcLog log, PhaseData phase, IReadOnlyList<IncomingDamageModifier> damageModsToUse)
        {
            var pData = new List<DamageModData>();
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                pData.Add(new DamageModData(actor, log, damageModsToUse, phase));
            }
            return pData;
        }

        public static List<DamageModData> BuildPersonalOutgoingDmgModifiersData(ParsedEvtcLog log, PhaseData phase, IReadOnlyDictionary<Spec, IReadOnlyList<OutgoingDamageModifier>> damageModsToUse)
        {
            var pData = new List<DamageModData>();
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                pData.Add(new DamageModData(actor, log, damageModsToUse[actor.Spec], phase));
            }
            return pData;
        }

        public static List<DamageModData> BuildPersonalIncomingDmgModifiersData(ParsedEvtcLog log, PhaseData phase, IReadOnlyDictionary<Spec, IReadOnlyList<IncomingDamageModifier>> damageModsToUse)
        {
            var pData = new List<DamageModData>();
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                pData.Add(new DamageModData(actor, log, damageModsToUse[actor.Spec], phase));
            }
            return pData;
        }
    }
}
