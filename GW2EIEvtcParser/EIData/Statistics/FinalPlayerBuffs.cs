using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{

    public enum BuffEnum { Self, Group, OffGroup, Squad };
    public class FinalPlayerBuffs : FinalBuffs
    {
        public double Generation { get; internal set; }
        public double Overstack { get; internal set; }
        public double Wasted { get; internal set; }
        public double UnknownExtended { get; internal set; }
        public double ByExtension { get; internal set; }
        public double Extended { get; internal set; }

        internal static (List<Dictionary<long, FinalPlayerBuffs>>, List<Dictionary<long, FinalPlayerBuffs>>) GetBuffsForPlayers(List<Player> playerList, ParsedEvtcLog log, AgentItem agentItem)
        {
            var uptimesByPhase = new List<Dictionary<long, FinalPlayerBuffs>>();
            var uptimesActiveByPhase = new List<Dictionary<long, FinalPlayerBuffs>>();

            IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                PhaseData phase = phases[phaseIndex];
                long phaseDuration = phase.DurationInMS;

                var boonDistributions = new Dictionary<Player, BuffDistribution>();
                foreach (Player p in playerList)
                {
                    boonDistributions[p] = p.GetBuffDistribution(log, phaseIndex);
                }

                var boonsToTrack = new HashSet<Buff>(boonDistributions.SelectMany(x => x.Value.PresentBuffIDs()).Select(x => log.Buffs.BuffsByIds[x]));

                var final =
                    new Dictionary<long, FinalPlayerBuffs>();
                var finalActive =
                    new Dictionary<long, FinalPlayerBuffs>();

                foreach (Buff boon in boonsToTrack)
                {
                    double totalGeneration = 0;
                    double totalOverstack = 0;
                    double totalWasted = 0;
                    double totalUnknownExtension = 0;
                    double totalExtension = 0;
                    double totalExtended = 0;
                    //
                    double totalActiveGeneration = 0;
                    double totalActiveOverstack = 0;
                    double totalActiveWasted = 0;
                    double totalActiveUnknownExtension = 0;
                    double totalActiveExtension = 0;
                    double totalActiveExtended = 0;
                    bool hasGeneration = false;
                    int activePlayerCount = 0;
                    foreach (KeyValuePair<Player, BuffDistribution> pair in boonDistributions)
                    {
                        BuffDistribution boons = pair.Value;
                        long playerActiveDuration = phase.GetActorActiveDuration(pair.Key, log);
                        if (boons.ContainsBuffID(boon.ID))
                        {
                            hasGeneration = hasGeneration || boons.HasSrc(boon.ID, agentItem);
                            double generation = boons.GetGeneration(boon.ID, agentItem);
                            double overstack = boons.GetOverstack(boon.ID, agentItem);
                            double wasted = boons.GetWaste(boon.ID, agentItem);
                            double unknownExtension = boons.GetUnknownExtension(boon.ID, agentItem);
                            double extension = boons.GetExtension(boon.ID, agentItem);
                            double extended = boons.GetExtended(boon.ID, agentItem);

                            totalGeneration += generation;
                            totalOverstack += overstack;
                            totalWasted += wasted;
                            totalUnknownExtension += unknownExtension;
                            totalExtension += extension;
                            totalExtended += extended;
                            if (playerActiveDuration > 0)
                            {
                                activePlayerCount++;
                                totalActiveGeneration += generation / playerActiveDuration;
                                totalActiveOverstack += overstack / playerActiveDuration;
                                totalActiveWasted += wasted / playerActiveDuration;
                                totalActiveUnknownExtension += unknownExtension / playerActiveDuration;
                                totalActiveExtension += extension / playerActiveDuration;
                                totalActiveExtended += extended / playerActiveDuration;
                            }
                        }
                    }
                    totalGeneration /= phaseDuration;
                    totalOverstack /= phaseDuration;
                    totalWasted /= phaseDuration;
                    totalUnknownExtension /= phaseDuration;
                    totalExtension /= phaseDuration;
                    totalExtended /= phaseDuration;

                    if (hasGeneration)
                    {
                        var uptime = new FinalPlayerBuffs();
                        var uptimeActive = new FinalPlayerBuffs();
                        final[boon.ID] = uptime;
                        finalActive[boon.ID] = uptimeActive;
                        if (boon.Type == BuffType.Duration)
                        {
                            uptime.Generation = Math.Round(100.0 * totalGeneration / playerList.Count, ParserHelper.BuffDigit);
                            uptime.Overstack = Math.Round(100.0 * (totalOverstack + totalGeneration) / playerList.Count, ParserHelper.BuffDigit);
                            uptime.Wasted = Math.Round(100.0 * (totalWasted) / playerList.Count, ParserHelper.BuffDigit);
                            uptime.UnknownExtended = Math.Round(100.0 * (totalUnknownExtension) / playerList.Count, ParserHelper.BuffDigit);
                            uptime.ByExtension = Math.Round(100.0 * (totalExtension) / playerList.Count, ParserHelper.BuffDigit);
                            uptime.Extended = Math.Round(100.0 * (totalExtended) / playerList.Count, ParserHelper.BuffDigit);
                            //
                            if (activePlayerCount > 0)
                            {
                                uptimeActive.Generation = Math.Round(100.0 * totalActiveGeneration / activePlayerCount, ParserHelper.BuffDigit);
                                uptimeActive.Overstack = Math.Round(100.0 * (totalActiveOverstack + totalActiveGeneration) / activePlayerCount, ParserHelper.BuffDigit);
                                uptimeActive.Wasted = Math.Round(100.0 * (totalActiveWasted) / activePlayerCount, ParserHelper.BuffDigit);
                                uptimeActive.UnknownExtended = Math.Round(100.0 * (totalActiveUnknownExtension) / activePlayerCount, ParserHelper.BuffDigit);
                                uptimeActive.ByExtension = Math.Round(100.0 * (totalActiveExtension) / activePlayerCount, ParserHelper.BuffDigit);
                                uptimeActive.Extended = Math.Round(100.0 * (totalActiveExtended) / activePlayerCount, ParserHelper.BuffDigit);
                            }
                        }
                        else if (boon.Type == BuffType.Intensity)
                        {
                            uptime.Generation = Math.Round(totalGeneration / playerList.Count, ParserHelper.BuffDigit);
                            uptime.Overstack = Math.Round((totalOverstack + totalGeneration) / playerList.Count, ParserHelper.BuffDigit);
                            uptime.Wasted = Math.Round((totalWasted) / playerList.Count, ParserHelper.BuffDigit);
                            uptime.UnknownExtended = Math.Round((totalUnknownExtension) / playerList.Count, ParserHelper.BuffDigit);
                            uptime.ByExtension = Math.Round((totalExtension) / playerList.Count, ParserHelper.BuffDigit);
                            uptime.Extended = Math.Round((totalExtended) / playerList.Count, ParserHelper.BuffDigit);
                            //
                            if (activePlayerCount > 0)
                            {
                                uptimeActive.Generation = Math.Round(totalActiveGeneration / activePlayerCount, ParserHelper.BuffDigit);
                                uptimeActive.Overstack = Math.Round((totalActiveOverstack + totalActiveGeneration) / activePlayerCount, ParserHelper.BuffDigit);
                                uptimeActive.Wasted = Math.Round((totalActiveWasted) / activePlayerCount, ParserHelper.BuffDigit);
                                uptimeActive.UnknownExtended = Math.Round((totalActiveUnknownExtension) / activePlayerCount, ParserHelper.BuffDigit);
                                uptimeActive.ByExtension = Math.Round((totalActiveExtension) / activePlayerCount, ParserHelper.BuffDigit);
                                uptimeActive.Extended = Math.Round((totalActiveExtended) / activePlayerCount, ParserHelper.BuffDigit);
                            }
                        }
                    }
                }

                uptimesByPhase.Add(final);
                uptimesActiveByPhase.Add(finalActive);
            }

            return (uptimesByPhase, uptimesActiveByPhase);
        }


        internal static (List<Dictionary<long, FinalPlayerBuffs>>, List<Dictionary<long, FinalPlayerBuffs>>) GetBuffsForSelf(ParsedEvtcLog log, Player player)
        {
            var selfBuffsActive = new List<Dictionary<long, FinalPlayerBuffs>>();
            var selfBuffs = new List<Dictionary<long, FinalPlayerBuffs>>();
            IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                var final = new Dictionary<long, FinalPlayerBuffs>();
                var finalActive = new Dictionary<long, FinalPlayerBuffs>();

                PhaseData phase = phases[phaseIndex];

                BuffDistribution selfBoons = player.GetBuffDistribution(log, phaseIndex);
                IReadOnlyDictionary<long, long> buffPresence = player.GetBuffPresence(log, phaseIndex);

                long phaseDuration = phase.DurationInMS;
                long playerActiveDuration = phase.GetActorActiveDuration(player, log);
                foreach (Buff boon in player.TrackedBuffs)
                {
                    if (selfBoons.ContainsBuffID(boon.ID))
                    {
                        var uptime = new FinalPlayerBuffs
                        {
                            Uptime = 0,
                            Generation = 0,
                            Overstack = 0,
                            Wasted = 0,
                            UnknownExtended = 0,
                            ByExtension = 0,
                            Extended = 0
                        };
                        var uptimeActive = new FinalPlayerBuffs
                        {
                            Uptime = 0,
                            Generation = 0,
                            Overstack = 0,
                            Wasted = 0,
                            UnknownExtended = 0,
                            ByExtension = 0,
                            Extended = 0
                        };
                        final[boon.ID] = uptime;
                        finalActive[boon.ID] = uptimeActive;
                        double generationValue = selfBoons.GetGeneration(boon.ID, player.AgentItem);
                        double uptimeValue = selfBoons.GetUptime(boon.ID);
                        double overstackValue = selfBoons.GetOverstack(boon.ID, player.AgentItem);
                        double wasteValue = selfBoons.GetWaste(boon.ID, player.AgentItem);
                        double unknownExtensionValue = selfBoons.GetUnknownExtension(boon.ID, player.AgentItem);
                        double extensionValue = selfBoons.GetExtension(boon.ID, player.AgentItem);
                        double extendedValue = selfBoons.GetExtended(boon.ID, player.AgentItem);
                        if (boon.Type == BuffType.Duration)
                        {
                            uptime.Uptime = Math.Round(100.0 * uptimeValue / phaseDuration, ParserHelper.BuffDigit);
                            uptime.Generation = Math.Round(100.0 * generationValue / phaseDuration, ParserHelper.BuffDigit);
                            uptime.Overstack = Math.Round(100.0 * (overstackValue + generationValue) / phaseDuration, ParserHelper.BuffDigit);
                            uptime.Wasted = Math.Round(100.0 * wasteValue / phaseDuration, ParserHelper.BuffDigit);
                            uptime.UnknownExtended = Math.Round(100.0 * unknownExtensionValue / phaseDuration, ParserHelper.BuffDigit);
                            uptime.ByExtension = Math.Round(100.0 * extensionValue / phaseDuration, ParserHelper.BuffDigit);
                            uptime.Extended = Math.Round(100.0 * extendedValue / phaseDuration, ParserHelper.BuffDigit);
                            //
                            if (playerActiveDuration > 0)
                            {
                                uptimeActive.Uptime = Math.Round(100.0 * uptimeValue / playerActiveDuration, ParserHelper.BuffDigit);
                                uptimeActive.Generation = Math.Round(100.0 * generationValue / playerActiveDuration, ParserHelper.BuffDigit);
                                uptimeActive.Overstack = Math.Round(100.0 * (overstackValue + generationValue) / playerActiveDuration, ParserHelper.BuffDigit);
                                uptimeActive.Wasted = Math.Round(100.0 * wasteValue / playerActiveDuration, ParserHelper.BuffDigit);
                                uptimeActive.UnknownExtended = Math.Round(100.0 * unknownExtensionValue / playerActiveDuration, ParserHelper.BuffDigit);
                                uptimeActive.ByExtension = Math.Round(100.0 * extensionValue / playerActiveDuration, ParserHelper.BuffDigit);
                                uptimeActive.Extended = Math.Round(100.0 * extendedValue / playerActiveDuration, ParserHelper.BuffDigit);
                            }
                        }
                        else if (boon.Type == BuffType.Intensity)
                        {
                            uptime.Uptime = Math.Round(uptimeValue / phaseDuration, ParserHelper.BuffDigit);
                            uptime.Generation = Math.Round(generationValue / phaseDuration, ParserHelper.BuffDigit);
                            uptime.Overstack = Math.Round((overstackValue + generationValue) / phaseDuration, ParserHelper.BuffDigit);
                            uptime.Wasted = Math.Round(wasteValue / phaseDuration, ParserHelper.BuffDigit);
                            uptime.UnknownExtended = Math.Round(unknownExtensionValue / phaseDuration, ParserHelper.BuffDigit);
                            uptime.ByExtension = Math.Round(extensionValue / phaseDuration, ParserHelper.BuffDigit);
                            uptime.Extended = Math.Round(extendedValue / phaseDuration, ParserHelper.BuffDigit);
                            //
                            if (playerActiveDuration > 0)
                            {
                                uptimeActive.Uptime = Math.Round(uptimeValue / playerActiveDuration, ParserHelper.BuffDigit);
                                uptimeActive.Generation = Math.Round(generationValue / playerActiveDuration, ParserHelper.BuffDigit);
                                uptimeActive.Overstack = Math.Round((overstackValue + generationValue) / playerActiveDuration, ParserHelper.BuffDigit);
                                uptimeActive.Wasted = Math.Round(wasteValue / playerActiveDuration, ParserHelper.BuffDigit);
                                uptimeActive.UnknownExtended = Math.Round(unknownExtensionValue / playerActiveDuration, ParserHelper.BuffDigit);
                                uptimeActive.ByExtension = Math.Round(extensionValue / playerActiveDuration, ParserHelper.BuffDigit);
                                uptimeActive.Extended = Math.Round(extendedValue / playerActiveDuration, ParserHelper.BuffDigit);
                            }
                            //
                            if (buffPresence.TryGetValue(boon.ID, out long presenceValueBoon))
                            {
                                uptime.Presence = Math.Round(100.0 * presenceValueBoon / phaseDuration, ParserHelper.BuffDigit);
                                //
                                if (playerActiveDuration > 0)
                                {
                                    uptimeActive.Presence = Math.Round(100.0 * presenceValueBoon / playerActiveDuration, ParserHelper.BuffDigit);
                                }
                            }
                        }
                    }
                }

                selfBuffs.Add(final);
                selfBuffsActive.Add(finalActive);
            }
            return (selfBuffs, selfBuffsActive);
        }

    }

}
