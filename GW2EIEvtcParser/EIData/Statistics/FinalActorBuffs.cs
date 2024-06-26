using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{

    public class FinalActorBuffs
    {
        public double Uptime { get; internal set; }
        public double Presence { get; internal set; }
        public double Generation { get; internal set; }
        public double Overstack { get; internal set; }
        public double Wasted { get; internal set; }
        public double UnknownExtended { get; internal set; }
        public double ByExtension { get; internal set; }
        public double Extended { get; internal set; }

        internal static Dictionary<long, FinalActorBuffs>[] GetBuffsForPlayers(List<Player> playerList, ParsedEvtcLog log, AgentItem srcAgentItem, long start, long end)
        {

            long phaseDuration = end - start;

            var buffDistributionPerPlayer = new Dictionary<Player, BuffDistribution>();
            foreach (Player p in playerList)
            {
                buffDistributionPerPlayer[p] = p.GetBuffDistribution(log, start, end);
            }

            var buffsToTrack = new HashSet<Buff>(buffDistributionPerPlayer.SelectMany(x => x.Value.BuffIDs).Select(x => log.Buffs.BuffsByIds[x]));

            var buffs =
                new Dictionary<long, FinalActorBuffs>();
            var activeBuffs =
                new Dictionary<long, FinalActorBuffs>();

            foreach (Buff boon in buffsToTrack)
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
                foreach (KeyValuePair<Player, BuffDistribution> buffDistributionByPlayer in buffDistributionPerPlayer)
                {
                    BuffDistribution buffDistribution = buffDistributionByPlayer.Value;
                    long playerActiveDuration = buffDistributionByPlayer.Key.GetActiveDuration(log, start, end);
                    if (buffDistribution.HasBuffID(boon.ID))
                    {
                        hasGeneration = hasGeneration || buffDistribution.HasSrc(boon.ID, srcAgentItem);
                        double generation = buffDistribution.GetGeneration(boon.ID, srcAgentItem);
                        double overstack = buffDistribution.GetOverstack(boon.ID, srcAgentItem);
                        double wasted = buffDistribution.GetWaste(boon.ID, srcAgentItem);
                        double unknownExtension = buffDistribution.GetUnknownExtension(boon.ID, srcAgentItem);
                        double extension = buffDistribution.GetExtension(boon.ID, srcAgentItem);
                        double extended = buffDistribution.GetExtended(boon.ID, srcAgentItem);

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
                    var uptime = new FinalActorBuffs();
                    var uptimeActive = new FinalActorBuffs();
                    buffs[boon.ID] = uptime;
                    activeBuffs[boon.ID] = uptimeActive;
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

            return new Dictionary<long, FinalActorBuffs>[] { buffs, activeBuffs };
        }


        internal static Dictionary<long, FinalActorBuffs>[] GetBuffsForSelf(ParsedEvtcLog log, AbstractSingleActor dstActor, long start, long end)
        {
            var buffs = new Dictionary<long, FinalActorBuffs>();
            var activeBuffs = new Dictionary<long, FinalActorBuffs>();

            BuffDistribution buffDistribution = dstActor.GetBuffDistribution(log, start, end);
            IReadOnlyDictionary<long, long> buffPresence = dstActor.GetBuffPresence(log, start, end);

            long phaseDuration = end - start;
            long playerActiveDuration = dstActor.GetActiveDuration(log, start, end);
            foreach (Buff buff in dstActor.GetTrackedBuffs(log))
            {
                if (buffDistribution.HasBuffID(buff.ID))
                {
                    var uptime = new FinalActorBuffs
                    {
                        Uptime = 0,
                        Generation = 0,
                        Overstack = 0,
                        Wasted = 0,
                        UnknownExtended = 0,
                        ByExtension = 0,
                        Extended = 0
                    };
                    var uptimeActive = new FinalActorBuffs
                    {
                        Uptime = 0,
                        Generation = 0,
                        Overstack = 0,
                        Wasted = 0,
                        UnknownExtended = 0,
                        ByExtension = 0,
                        Extended = 0
                    };
                    buffs[buff.ID] = uptime;
                    activeBuffs[buff.ID] = uptimeActive;
                    double generationValue = buffDistribution.GetGeneration(buff.ID, dstActor.AgentItem);
                    double uptimeValue = buffDistribution.GetUptime(buff.ID);
                    double overstackValue = buffDistribution.GetOverstack(buff.ID, dstActor.AgentItem);
                    double wasteValue = buffDistribution.GetWaste(buff.ID, dstActor.AgentItem);
                    double unknownExtensionValue = buffDistribution.GetUnknownExtension(buff.ID, dstActor.AgentItem);
                    double extensionValue = buffDistribution.GetExtension(buff.ID, dstActor.AgentItem);
                    double extendedValue = buffDistribution.GetExtended(buff.ID, dstActor.AgentItem);
                    if (buff.Type == BuffType.Duration)
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
                    else if (buff.Type == BuffType.Intensity)
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
                        if (buffPresence.TryGetValue(buff.ID, out long presenceValueBoon))
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
            return new Dictionary<long, FinalActorBuffs>[] { buffs, activeBuffs };
        }

    }

}
