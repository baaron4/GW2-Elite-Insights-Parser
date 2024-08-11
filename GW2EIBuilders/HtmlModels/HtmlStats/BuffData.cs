using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIBuilders.HtmlModels.HTMLStats
{
    internal class BuffData
    {
        public double Avg { get; set; }
        public List<List<object>> Data { get; set; } = new List<List<object>>();

        private BuffData(IReadOnlyDictionary<long, FinalActorBuffs> buffs, IReadOnlyList<Buff> listToUse, double avg)
        {
            Avg = avg;
            foreach (Buff buff in listToUse)
            {
                var buffVals = new List<object>();
                Data.Add(buffVals);

                if (buffs.TryGetValue(buff.ID, out FinalActorBuffs uptime))
                {
                    buffVals.Add(uptime.Uptime);
                    if (buff.Type == Buff.BuffType.Intensity && uptime.Presence > 0)
                    {
                        buffVals.Add(uptime.Presence);
                    }
                }
            }
        }

        private BuffData(IReadOnlyDictionary<long, FinalBuffsDictionary> buffs, IReadOnlyList<Buff> listToUse, AbstractSingleActor actor)
        {
            foreach (Buff buff in listToUse)
            {
                if (buffs.TryGetValue(buff.ID, out FinalBuffsDictionary toUse) && toUse.GeneratedBy.ContainsKey(actor))
                {
                    Data.Add(new List<object>()
                        {
                            toUse.GeneratedBy[actor],
                            toUse.OverstackedBy[actor],
                            toUse.WastedFrom[actor],
                            toUse.UnknownExtensionFrom[actor],
                            toUse.ExtensionBy[actor],
                            toUse.ExtendedFrom[actor]
                        });
                }
                else
                {
                    Data.Add(new List<object>()
                        {
                            0,
                            0,
                            0,
                            0,
                            0,
                            0
                        });
                }
            }
        }

        private BuffData(IReadOnlyList<Buff> listToUse, IReadOnlyDictionary<long, FinalActorBuffs> uptimes)
        {
            foreach (Buff buff in listToUse)
            {
                if (uptimes.TryGetValue(buff.ID, out FinalActorBuffs uptime))
                {
                    Data.Add(new List<object>()
                        {
                            uptime.Generation,
                            uptime.Overstack,
                            uptime.Wasted,
                            uptime.UnknownExtended,
                            uptime.ByExtension,
                            uptime.Extended
                        });
                }
                else
                {
                    Data.Add(new List<object>()
                        {
                            0,
                            0,
                            0,
                            0,
                            0,
                            0
                        });
                }
            }
        }

        private BuffData(Spec spec, IReadOnlyDictionary<Spec, IReadOnlyList<Buff>> buffsBySpec, IReadOnlyDictionary<long, FinalActorBuffs> uptimes)
        {
            foreach (Buff buff in buffsBySpec[spec])
            {
                var boonVals = new List<object>();
                Data.Add(boonVals);
                if (uptimes.TryGetValue(buff.ID, out FinalActorBuffs uptime))
                {
                    boonVals.Add(uptime.Uptime);
                    if (buff.Type == Buff.BuffType.Intensity && uptime.Presence > 0)
                    {
                        boonVals.Add(uptime.Presence);
                    }
                }
                else
                {
                    boonVals.Add(0);
                }
            }
        }

        //////
        public static List<BuffData> BuildBuffUptimeData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase)
        {
            var list = new List<BuffData>();
            bool boonTable = listToUse.Any(x => x.Classification == Buff.BuffClassification.Boon);
            bool conditionTable = listToUse.Any(x => x.Classification == Buff.BuffClassification.Condition);

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                double avg = 0.0;
                if (boonTable)
                {
                    avg = actor.GetGameplayStats(log, phase.Start, phase.End).AvgBoons;
                }
                else if (conditionTable)
                {
                    avg = actor.GetGameplayStats(log, phase.Start, phase.End).AvgConditions;
                }
                list.Add(new BuffData(actor.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End), listToUse, avg));
            }
            return list;
        }

        public static List<BuffData> BuildActiveBuffUptimeData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase)
        {
            var list = new List<BuffData>();
            bool boonTable = listToUse.Any(x => x.Classification == Buff.BuffClassification.Boon);
            bool conditionTable = listToUse.Any(x => x.Classification == Buff.BuffClassification.Condition);

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                double avg = 0.0;
                if (boonTable)
                {
                    avg = actor.GetGameplayStats(log, phase.Start, phase.End).AvgActiveBoons;
                }
                else if (conditionTable)
                {
                    avg = actor.GetGameplayStats(log, phase.Start, phase.End).AvgActiveConditions;
                }
                list.Add(new BuffData(actor.GetActiveBuffs(BuffEnum.Self, log, phase.Start, phase.End), listToUse, avg));
            }
            return list;
        }

        //////
        public static List<BuffData> BuildPersonalBuffUptimeData(ParsedEvtcLog log, IReadOnlyDictionary<Spec, IReadOnlyList<Buff>> buffsBySpec, PhaseData phase)
        {
            var list = new List<BuffData>();
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(new BuffData(actor.Spec, buffsBySpec, actor.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End)));
            }
            return list;
        }

        public static List<BuffData> BuildActivePersonalBuffUptimeData(ParsedEvtcLog log, IReadOnlyDictionary<Spec, IReadOnlyList<Buff>> buffsBySpec, PhaseData phase)
        {
            var list = new List<BuffData>();
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(new BuffData(actor.Spec, buffsBySpec, actor.GetActiveBuffs(BuffEnum.Self, log, phase.Start, phase.End)));
            }
            return list;
        }


        //////
        public static List<BuffData> BuildBuffGenerationData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase, BuffEnum type)
        {
            var list = new List<BuffData>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(new BuffData(listToUse, actor.GetBuffs(type, log, phase.Start, phase.End)));
            }
            return list;
        }

        public static List<BuffData> BuildActiveBuffGenerationData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase, BuffEnum type)
        {
            var list = new List<BuffData>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(new BuffData(listToUse, actor.GetActiveBuffs(type, log, phase.Start, phase.End)));
            }
            return list;
        }
        // 
        private static List<BuffData> BuildBuffDictionaryData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase, AbstractSingleActor player)
        {
            IReadOnlyDictionary<long, FinalBuffsDictionary> buffs = player.GetBuffsDictionary(log, phase.Start, phase.End);
            var list = new List<BuffData>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(new BuffData(buffs, listToUse, actor));
            }
            return list;
        }
        public static List<List<BuffData>> BuildBuffDictionariesData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase)
        {
            var list = new List<List<BuffData>>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(BuildBuffDictionaryData(log, listToUse, phase, actor));
            }
            return list;
        }

        private static List<BuffData> BuildActiveBuffDictionaryData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase, AbstractSingleActor player)
        {
            IReadOnlyDictionary<long, FinalBuffsDictionary> buffs = player.GetActiveBuffsDictionary(log, phase.Start, phase.End);
            var list = new List<BuffData>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(new BuffData(buffs, listToUse, actor));
            }
            return list;
        }

        public static List<List<BuffData>> BuildActiveBuffDictionariesData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase)
        {
            var list = new List<List<BuffData>>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(BuildActiveBuffDictionaryData(log, listToUse, phase, actor));
            }
            return list;
        }

        /////
        public static List<BuffData> BuildTargetCondiData(ParsedEvtcLog log, PhaseData phase, AbstractSingleActor actor)
        {
            return BuildBuffDictionaryData(log, log.StatisticsHelper.PresentConditions, phase, actor);
        }

        public static BuffData BuildTargetCondiUptimeData(ParsedEvtcLog log, PhaseData phase, AbstractSingleActor target)
        {
            IReadOnlyDictionary<long, FinalActorBuffs> buffs = target.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End);
            return new BuffData(buffs, log.StatisticsHelper.PresentConditions, target.GetGameplayStats(log, phase.Start, phase.End).AvgConditions);
        }

        public static BuffData BuildTargetBoonUptimeData(ParsedEvtcLog log, PhaseData phase, AbstractSingleActor target)
        {
            IReadOnlyDictionary<long, FinalActorBuffs> buffs = target.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End);
            return new BuffData(buffs, log.StatisticsHelper.PresentBoons, target.GetGameplayStats(log, phase.Start, phase.End).AvgBoons);
        }
    }
}
