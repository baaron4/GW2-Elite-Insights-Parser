using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIBuilders.HtmlModels
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
                if (buffs.TryGetValue(buff.ID, out FinalBuffsDictionary toUse) && toUse.Generated.ContainsKey(actor))
                {
                    Data.Add(new List<object>()
                        {
                            toUse.Generated[actor],
                            toUse.Overstacked[actor],
                            toUse.Wasted[actor],
                            toUse.UnknownExtension[actor],
                            toUse.Extension[actor],
                            toUse.Extended[actor]
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

        private BuffData(string prof, IReadOnlyDictionary<string, List<Buff>> buffsBySpec, IReadOnlyDictionary<long, FinalActorBuffs> uptimes)
        {
            foreach (Buff buff in buffsBySpec[prof])
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
            bool boonTable = listToUse.Any(x => x.Nature == Buff.BuffNature.Boon);
            bool conditionTable = listToUse.Any(x => x.Nature == Buff.BuffNature.Condition);

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
            bool boonTable = listToUse.Any(x => x.Nature == Buff.BuffNature.Boon);
            bool conditionTable = listToUse.Any(x => x.Nature == Buff.BuffNature.Condition);

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
        public static List<BuffData> BuildPersonalBuffUptimeData(ParsedEvtcLog log, IReadOnlyDictionary<string, List<Buff>> buffsBySpec, PhaseData phase)
        {
            var list = new List<BuffData>();
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(new BuffData(actor.Prof, buffsBySpec, actor.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End)));
            }
            return list;
        }

        public static List<BuffData> BuildActivePersonalBuffUptimeData(ParsedEvtcLog log, IReadOnlyDictionary<string, List<Buff>> buffsBySpec, PhaseData phase)
        {
            var list = new List<BuffData>();
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(new BuffData(actor.Prof, buffsBySpec, actor.GetActiveBuffs(BuffEnum.Self, log, phase.Start, phase.End)));
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

        /////
        public static List<BuffData> BuildTargetCondiData(ParsedEvtcLog log, long start, long end, NPC target)
        {
            Dictionary<long, FinalBuffsDictionary> conditions = target.GetBuffsDictionary(log, start, end);
            var list = new List<BuffData>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(new BuffData(conditions, log.StatisticsHelper.PresentConditions, actor));
            }
            return list;
        }

        public static BuffData BuildTargetCondiUptimeData(ParsedEvtcLog log, PhaseData phase, NPC target)
        {
            IReadOnlyDictionary<long, FinalActorBuffs> buffs = target.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End);
            return new BuffData(buffs, log.StatisticsHelper.PresentConditions, target.GetGameplayStats(log, phase.Start, phase.End).AvgConditions);
        }

        public static BuffData BuildTargetBoonData(ParsedEvtcLog log, PhaseData phase, NPC target)
        {
            IReadOnlyDictionary<long, FinalActorBuffs> buffs = target.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End);
            return new BuffData(buffs, log.StatisticsHelper.PresentBoons, target.GetGameplayStats(log, phase.Start, phase.End).AvgBoons);
        }
    }
}
