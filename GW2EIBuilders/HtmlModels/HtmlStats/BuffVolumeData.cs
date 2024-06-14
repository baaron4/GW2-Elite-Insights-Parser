using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIBuilders.HtmlModels.HTMLStats
{
    internal class BuffVolumeData
    {
        public List<List<object>> Data { get; set; } = new List<List<object>>();

        private BuffVolumeData(IReadOnlyDictionary<long, FinalActorBuffVolumes> buffVolumes, IReadOnlyList<Buff> listToUse)
        {
            foreach (Buff buff in listToUse)
            {
                var buffVals = new List<object>();
                Data.Add(buffVals);

                if (buffVolumes.TryGetValue(buff.ID, out FinalActorBuffVolumes volume))
                {
                    buffVals.Add(volume.Incoming);
                    buffVals.Add(volume.IncomingByExtension);
                    buffVals.Add(volume.IncomingByUnknownExtension);
                }
            }
        }

        private BuffVolumeData(IReadOnlyDictionary<long, FinalBuffVolumesDictionary> buffVolumes, IReadOnlyList<Buff> listToUse, AbstractSingleActor actor)
        {
            foreach (Buff buff in listToUse)
            {
                if (buffVolumes.TryGetValue(buff.ID, out FinalBuffVolumesDictionary toUse) && toUse.IncomingBy.ContainsKey(actor))
                {
                    Data.Add(new List<object>()
                        {
                            toUse.IncomingBy[actor],
                            toUse.IncomingByExtensionBy[actor],
                        });
                }
                else
                {
                    Data.Add(new List<object>()
                        {
                            0,
                            0,
                        });
                }
            }
        }

        private BuffVolumeData(IReadOnlyList<Buff> listToUse, IReadOnlyDictionary<long, FinalActorBuffVolumes> volumes)
        {
            foreach (Buff buff in listToUse)
            {
                if (volumes.TryGetValue(buff.ID, out FinalActorBuffVolumes volume))
                {
                    Data.Add(new List<object>()
                        {
                            volume.Outgoing,
                            volume.OutgoingByExtension,
                        });
                }
                else
                {
                    Data.Add(new List<object>()
                        {
                            0,
                            0,
                        });
                }
            }
        }

        private BuffVolumeData(Spec spec, IReadOnlyDictionary<Spec, IReadOnlyList<Buff>> buffsBySpec, IReadOnlyDictionary<long, FinalActorBuffVolumes> volumes)
        {
            foreach (Buff buff in buffsBySpec[spec])
            {
                var boonVals = new List<object>();
                Data.Add(boonVals);
                if (volumes.TryGetValue(buff.ID, out FinalActorBuffVolumes volume))
                {
                    boonVals.Add(volume.Incoming);
                    boonVals.Add(volume.IncomingByExtension);
                    boonVals.Add(volume.IncomingByUnknownExtension);
                }
                else
                {
                    boonVals.Add(0);
                }
            }
        }

        //////
        public static List<BuffVolumeData> BuildBuffIncomingVolumeData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase)
        {
            var list = new List<BuffVolumeData>();
            bool boonTable = listToUse.Any(x => x.Classification == Buff.BuffClassification.Boon);
            bool conditionTable = listToUse.Any(x => x.Classification == Buff.BuffClassification.Condition);

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(new BuffVolumeData(actor.GetBuffVolumes(BuffEnum.Self, log, phase.Start, phase.End), listToUse));
            }
            return list;
        }

        public static List<BuffVolumeData> BuildActiveBuffIncomingVolumeData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase)
        {
            var list = new List<BuffVolumeData>();
            bool boonTable = listToUse.Any(x => x.Classification == Buff.BuffClassification.Boon);
            bool conditionTable = listToUse.Any(x => x.Classification == Buff.BuffClassification.Condition);

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(new BuffVolumeData(actor.GetActiveBuffVolumes(BuffEnum.Self, log, phase.Start, phase.End), listToUse));
            }
            return list;
        }

        //////
        public static List<BuffVolumeData> BuildPersonalBuffIncomingVolueData(ParsedEvtcLog log, IReadOnlyDictionary<Spec, IReadOnlyList<Buff>> buffsBySpec, PhaseData phase)
        {
            var list = new List<BuffVolumeData>();
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(new BuffVolumeData(actor.Spec, buffsBySpec, actor.GetBuffVolumes(BuffEnum.Self, log, phase.Start, phase.End)));
            }
            return list;
        }

        public static List<BuffVolumeData> BuildActivePersonalBuffIncomingVolumeData(ParsedEvtcLog log, IReadOnlyDictionary<Spec, IReadOnlyList<Buff>> buffsBySpec, PhaseData phase)
        {
            var list = new List<BuffVolumeData>();
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(new BuffVolumeData(actor.Spec, buffsBySpec, actor.GetActiveBuffVolumes(BuffEnum.Self, log, phase.Start, phase.End)));
            }
            return list;
        }


        //////
        public static List<BuffVolumeData> BuildBuffOutgoingVolumeData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase, BuffEnum type)
        {
            var list = new List<BuffVolumeData>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(new BuffVolumeData(listToUse, actor.GetBuffVolumes(type, log, phase.Start, phase.End)));
            }
            return list;
        }

        public static List<BuffVolumeData> BuildActiveBuffOutgoingVolumeData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase, BuffEnum type)
        {
            var list = new List<BuffVolumeData>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(new BuffVolumeData(listToUse, actor.GetActiveBuffVolumes(type, log, phase.Start, phase.End)));
            }
            return list;
        }
        // 
        private static List<BuffVolumeData> BuildBuffVolumeDictionaryData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase, AbstractSingleActor player)
        {
            IReadOnlyDictionary<long, FinalBuffVolumesDictionary> buffs = player.GetBuffVolumesDictionary(log, phase.Start, phase.End);
            var list = new List<BuffVolumeData>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(new BuffVolumeData(buffs, listToUse, actor));
            }
            return list;
        }
        public static List<List<BuffVolumeData>> BuildBuffVolumeDictionariesData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase)
        {
            var list = new List<List<BuffVolumeData>>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(BuildBuffVolumeDictionaryData(log, listToUse, phase, actor));
            }
            return list;
        }

        private static List<BuffVolumeData> BuildActiveBuffVolumeDictionaryData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase, AbstractSingleActor player)
        {
            IReadOnlyDictionary<long, FinalBuffVolumesDictionary> buffs = player.GetActiveBuffVolumesDictionary(log, phase.Start, phase.End);
            var list = new List<BuffVolumeData>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(new BuffVolumeData(buffs, listToUse, actor));
            }
            return list;
        }

        public static List<List<BuffVolumeData>> BuildActiveBuffVolumeDictionariesData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase)
        {
            var list = new List<List<BuffVolumeData>>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(BuildActiveBuffVolumeDictionaryData(log, listToUse, phase, actor));
            }
            return list;
        }

        /////
        public static List<BuffVolumeData> BuildTargetCondiVolumeData(ParsedEvtcLog log, PhaseData phase, AbstractSingleActor actor)
        {
            return BuildBuffVolumeDictionaryData(log, log.StatisticsHelper.PresentConditions, phase, actor);
        }

        public static BuffVolumeData BuildTargetCondiIncomingVolumeData(ParsedEvtcLog log, PhaseData phase, AbstractSingleActor target)
        {
            IReadOnlyDictionary<long, FinalActorBuffVolumes> buffs = target.GetBuffVolumes(BuffEnum.Self, log, phase.Start, phase.End);
            return new BuffVolumeData(buffs, log.StatisticsHelper.PresentConditions);
        }

        public static BuffVolumeData BuildTargetBoonIncomingVolumeData(ParsedEvtcLog log, PhaseData phase, AbstractSingleActor target)
        {
            IReadOnlyDictionary<long, FinalActorBuffVolumes> buffs = target.GetBuffVolumes(BuffEnum.Self, log, phase.Start, phase.End);
            return new BuffVolumeData(buffs, log.StatisticsHelper.PresentBoons);
        }
    }
}
