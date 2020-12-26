using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels
{
    internal class BuffData
    {
        public double Avg { get; set; }
        public List<List<object>> Data { get; set; } = new List<List<object>>();

        private BuffData(IReadOnlyDictionary<long, FinalPlayerBuffs> buffs, IReadOnlyList<Buff> listToUse, double avg)
        {
            Avg = avg;
            foreach (Buff buff in listToUse)
            {
                var buffVals = new List<object>();
                Data.Add(buffVals);

                if (buffs.TryGetValue(buff.ID, out FinalPlayerBuffs uptime))
                {
                    buffVals.Add(uptime.Uptime);
                    if (buff.Type == Buff.BuffType.Intensity && uptime.Presence > 0)
                    {
                        buffVals.Add(uptime.Presence);
                    }
                }
            }
        }

        private BuffData(IReadOnlyDictionary<long, FinalBuffs> buffs, IReadOnlyList<Buff> listToUse, double avg)
        {
            Avg = avg;
            foreach (Buff buff in listToUse)
            {
                var buffVals = new List<object>();
                Data.Add(buffVals);

                if (buffs.TryGetValue(buff.ID, out FinalBuffs uptime))
                {
                    buffVals.Add(uptime.Uptime);
                    if (buff.Type == Buff.BuffType.Intensity && uptime.Presence > 0)
                    {
                        buffVals.Add(uptime.Presence);
                    }
                }
            }
        }

        private BuffData(IReadOnlyDictionary<long, FinalBuffsDictionary> buffs, IReadOnlyList<Buff> listToUse, Player player)
        {
            foreach (Buff buff in listToUse)
            {
                if (buffs.TryGetValue(buff.ID, out FinalBuffsDictionary toUse) && toUse.Generated.ContainsKey(player))
                {
                    Data.Add(new List<object>()
                        {
                            toUse.Generated[player],
                            toUse.Overstacked[player],
                            toUse.Wasted[player],
                            toUse.UnknownExtension[player],
                            toUse.Extension[player],
                            toUse.Extended[player]
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

        private BuffData(IReadOnlyList<Buff> listToUse, IReadOnlyDictionary<long, FinalPlayerBuffs> uptimes)
        {
            foreach (Buff buff in listToUse)
            {
                if (uptimes.TryGetValue(buff.ID, out FinalPlayerBuffs uptime))
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

        private BuffData(string prof, IReadOnlyDictionary<string, List<Buff>> buffsBySpec, IReadOnlyDictionary<long, FinalPlayerBuffs> uptimes)
        {
            foreach (Buff buff in buffsBySpec[prof])
            {
                var boonVals = new List<object>();
                Data.Add(boonVals);
                if (uptimes.TryGetValue(buff.ID, out FinalPlayerBuffs uptime))
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
        public static List<BuffData> BuildBuffUptimeData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, int phaseIndex)
        {
            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];
            var list = new List<BuffData>();
            bool boonTable = listToUse.Any(x => x.Nature == Buff.BuffNature.Boon);

            foreach (Player player in log.PlayerList)
            {
                double avg = 0.0;
                if (boonTable)
                {
                    avg = player.GetGameplayStats(log, phase.Start, phase.End).AvgBoons;
                }
                list.Add(new BuffData(player.GetBuffs(log, phaseIndex, BuffEnum.Self), listToUse, avg));
            }
            return list;
        }

        public static List<BuffData> BuildActiveBuffUptimeData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, int phaseIndex)
        {
            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];
            var list = new List<BuffData>();
            bool boonTable = listToUse.Any(x => x.Nature == Buff.BuffNature.Boon);

            foreach (Player player in log.PlayerList)
            {
                double avg = 0.0;
                if (boonTable)
                {
                    avg = player.GetGameplayStats(log, phase.Start, phase.End).AvgActiveBoons;
                }
                list.Add(new BuffData(player.GetActiveBuffs(log, phaseIndex, BuffEnum.Self), listToUse, avg));
            }
            return list;
        }

        //////
        public static List<BuffData> BuildPersonalBuffUptimeData(ParsedEvtcLog log, IReadOnlyDictionary<string, List<Buff>> buffsBySpec, int phaseIndex)
        {
            var list = new List<BuffData>();
            foreach (Player player in log.PlayerList)
            {
                list.Add(new BuffData(player.Prof, buffsBySpec, player.GetBuffs(log, phaseIndex, BuffEnum.Self)));
            }
            return list;
        }

        public static List<BuffData> BuildActivePersonalBuffUptimeData(ParsedEvtcLog log, IReadOnlyDictionary<string, List<Buff>> buffsBySpec, int phaseIndex)
        {
            var list = new List<BuffData>();
            foreach (Player player in log.PlayerList)
            {
                list.Add(new BuffData(player.Prof, buffsBySpec, player.GetActiveBuffs(log, phaseIndex, BuffEnum.Self)));
            }
            return list;
        }


        //////
        public static List<BuffData> BuildBuffGenerationData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, int phaseIndex, BuffEnum type)
        {
            var list = new List<BuffData>();

            foreach (Player player in log.PlayerList)
            {
                Dictionary<long, FinalPlayerBuffs> uptimes;
                uptimes = player.GetBuffs(log, phaseIndex, type);
                list.Add(new BuffData(listToUse, uptimes));
            }
            return list;
        }

        public static List<BuffData> BuildActiveBuffGenerationData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, int phaseIndex, BuffEnum type)
        {
            var list = new List<BuffData>();

            foreach (Player player in log.PlayerList)
            {
                Dictionary<long, FinalPlayerBuffs> uptimes;
                uptimes = player.GetActiveBuffs(log, phaseIndex, type);
                list.Add(new BuffData(listToUse, uptimes));
            }
            return list;
        }

        /////
        public static List<BuffData> BuildTargetCondiData(ParsedEvtcLog log, long start, long end, NPC target)
        {
            Dictionary<long, FinalBuffsDictionary> conditions = target.GetBuffsDictionary(log, start, end);
            var list = new List<BuffData>();

            foreach (Player player in log.PlayerList)
            {
                list.Add(new BuffData(conditions, log.StatisticsHelper.PresentConditions, player));
            }
            return list;
        }

        public static BuffData BuildTargetCondiUptimeData(ParsedEvtcLog log, PhaseData phase, NPC target)
        {
            IReadOnlyDictionary<long, FinalBuffs> buffs = target.GetBuffs(log, phase.Start, phase.End);
            return new BuffData(buffs, log.StatisticsHelper.PresentConditions, target.GetGameplayStats(log, phase.Start, phase.End).AvgConditions);
        }

        public static BuffData BuildTargetBoonData(ParsedEvtcLog log, PhaseData phase, NPC target)
        {
            IReadOnlyDictionary<long, FinalBuffs> buffs = target.GetBuffs(log, phase.Start, phase.End);
            return new BuffData(buffs, log.StatisticsHelper.PresentBoons, target.GetGameplayStats(log, phase.Start, phase.End).AvgBoons);
        }
    }
}
