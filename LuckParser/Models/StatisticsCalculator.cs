using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LuckParser.Models.ParseModels;
using LuckParser.Parser;
using LuckParser.Setting;
using static LuckParser.Models.Statistics;

namespace LuckParser.Models
{
    /// <summary>
    /// Calculates statistical information from a log
    /// </summary>
    class StatisticsCalculator
    {

        private Statistics _statistics;

        private ParsedLog _log;
        private List<PhaseData> _phases;

        public StatisticsCalculator()
        {
        }

        /// <summary>
        /// Calculate a statistic from a log
        /// </summary>
        /// <param name="log"></param>
        /// <param name="switches"></param>
        /// <returns></returns>
        public Statistics CalculateStatistics(ParsedLog log)
        {

            CalculateConditions();
            //

            return _statistics;
        }

        private void CalculateConditions()
        {
            foreach (Target target in _log.FightData.Logic.Targets)
            {
                Dictionary<long, FinalTargetBuffs>[] stats = new Dictionary<long, FinalTargetBuffs>[_phases.Count];
                for (int phaseIndex = 0; phaseIndex < _phases.Count; phaseIndex++)
                {
                    BoonDistribution boonDistribution = target.GetBoonDistribution(_log, phaseIndex);
                    Dictionary<long, FinalTargetBuffs> rates = new Dictionary<long, FinalTargetBuffs>();
                    Dictionary<long, long> boonPresence = target.GetBoonPresence(_log, phaseIndex);
                    Dictionary<long, long> condiPresence = target.GetCondiPresence(_log, phaseIndex);

                    PhaseData phase = _phases[phaseIndex];
                    long fightDuration = phase.GetDuration();

                    foreach (Boon boon in target.TrackedBoons)
                    {
                        if (boonDistribution.ContainsKey(boon.ID))
                        {
                            FinalTargetBuffs buff = new FinalTargetBuffs(_log.PlayerList);
                            rates[boon.ID] = buff;
                            if (boon.Type == Boon.BoonType.Duration)
                            {
                                buff.Uptime = Math.Round(100.0 * boonDistribution.GetUptime(boon.ID) / fightDuration, 2);
                                foreach (Player p in _log.PlayerList)
                                {
                                    long gen = boonDistribution.GetGeneration(boon.ID, p.AgentItem);
                                    buff.Generated[p] = Math.Round(100.0 * gen / fightDuration, 2);
                                    buff.Overstacked[p] = Math.Round(100.0 * (boonDistribution.GetOverstack(boon.ID, p.AgentItem) + gen) / fightDuration, 2);
                                    buff.Wasted[p] = Math.Round(100.0 * boonDistribution.GetWaste(boon.ID, p.AgentItem) / fightDuration, 2);
                                    buff.UnknownExtension[p] = Math.Round(100.0 * boonDistribution.GetUnknownExtension(boon.ID, p.AgentItem) / fightDuration, 2);
                                    buff.Extension[p] = Math.Round(100.0 * boonDistribution.GetExtension(boon.ID, p.AgentItem) / fightDuration, 2);
                                    buff.Extended[p] = Math.Round(100.0 * boonDistribution.GetExtended(boon.ID, p.AgentItem) / fightDuration, 2);
                                }
                            }
                            else if (boon.Type == Boon.BoonType.Intensity)
                            {
                                buff.Uptime = Math.Round((double)boonDistribution.GetUptime(boon.ID) / fightDuration, 2);
                                foreach (Player p in _log.PlayerList)
                                {
                                    long gen = boonDistribution.GetGeneration(boon.ID, p.AgentItem);
                                    buff.Generated[p] = Math.Round((double)gen / fightDuration, 2);
                                    buff.Overstacked[p] = Math.Round((double)(boonDistribution.GetOverstack(boon.ID, p.AgentItem) + gen) / fightDuration, 2);
                                    buff.Wasted[p] = Math.Round((double)boonDistribution.GetWaste(boon.ID, p.AgentItem) / fightDuration, 2);
                                    buff.UnknownExtension[p] = Math.Round((double)boonDistribution.GetUnknownExtension(boon.ID, p.AgentItem) / fightDuration, 2);
                                    buff.Extension[p] = Math.Round((double)boonDistribution.GetExtension(boon.ID, p.AgentItem) / fightDuration, 2);
                                    buff.Extended[p] = Math.Round((double)boonDistribution.GetExtended(boon.ID, p.AgentItem) / fightDuration, 2);
                                }
                                if (boonPresence.TryGetValue(boon.ID, out long presenceValueBoon))
                                {
                                    buff.Presence = Math.Round(100.0 * presenceValueBoon / fightDuration, 2);
                                }
                                else if (condiPresence.TryGetValue(boon.ID, out long presenceValueCondi))
                                {
                                    buff.Presence = Math.Round(100.0 * presenceValueCondi / fightDuration, 2);
                                }
                            }
                        }
                    }
                    stats[phaseIndex] = rates;
                }
                _statistics.TargetBuffs[target] = stats;
            }
        }
    }
}