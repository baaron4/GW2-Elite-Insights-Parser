using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{

    public class FinalActorBuffVolumes
    {
        public double Incoming { get; internal set; }
        public double IncomingByExtension { get; internal set; }
        public double IncomingByUnknownExtension { get; internal set; }
        public double Outgoing { get; internal set; }
        public double OutgoingByExtension { get; internal set; }

        internal static Dictionary<long, FinalActorBuffVolumes>[] GetBuffVolumesForPlayers(List<Player> playerList, ParsedEvtcLog log, AgentItem srcAgentItem, long start, long end)
        {

            long phaseDuration = end - start;

            var buffsToTrack = new HashSet<Buff>();
            foreach (Player p in playerList)
            {
                buffsToTrack.UnionWith(p.GetTrackedBuffs(log));
            }

            var buffs =
                new Dictionary<long, FinalActorBuffVolumes>();
            var activeBuffs =
                new Dictionary<long, FinalActorBuffVolumes>();

            foreach (Buff buff in buffsToTrack)
            {
                double totalOutgoing = 0;
                double totalOutgoingByExtension = 0;
                //
                double totalActiveOutgoing = 0;
                double totalActiveOutgoingByExtension = 0;
                int activePlayerCount = 0;
                foreach (Player p in playerList)
                {
                    long playerActiveDuration = p.GetActiveDuration(log, start, end);
                    if (playerActiveDuration > 0)
                    {
                        activePlayerCount++;
                    }
                    foreach (AbstractBuffEvent abe in log.CombatData.GetBuffDataByIDByDst(buff.ID, p.AgentItem))
                    {
                        if (abe.Time >= start && abe.Time <= end && abe is AbstractBuffApplyEvent abae)
                        {
                            abae.TryFindSrc(log);
                            if (abae.CreditedBy == srcAgentItem)
                            {
                                if (abae is BuffApplyEvent bae)
                                {
                                    // We ignore infinite duration buffs
                                    /*if (bae.AppliedDuration >= int.MaxValue)
                                    {
                                        continue;
                                    }*/
                                    totalOutgoing += bae.AppliedDuration;
                                    if (playerActiveDuration > 0)
                                    {
                                        totalActiveOutgoing += bae.AppliedDuration / playerActiveDuration;
                                    }
                                }
                                if (abae is BuffExtensionEvent bee)
                                {
                                    totalOutgoingByExtension += bee.ExtendedDuration;
                                    if (playerActiveDuration > 0)
                                    {
                                        totalActiveOutgoingByExtension += bee.ExtendedDuration / playerActiveDuration;
                                    }
                                }
                            }
                        }
                    }
                }
                totalOutgoing += totalOutgoingByExtension;
                totalActiveOutgoing = totalActiveOutgoingByExtension;

                totalOutgoing /= phaseDuration;
                totalOutgoingByExtension /= phaseDuration;

                var uptime = new FinalActorBuffVolumes();
                var uptimeActive = new FinalActorBuffVolumes();
                buffs[buff.ID] = uptime;
                activeBuffs[buff.ID] = uptimeActive;
                if (buff.Type == BuffType.Duration)
                {
                    uptime.Outgoing = Math.Round(100.0 * totalOutgoing / playerList.Count, ParserHelper.BuffDigit);
                    uptime.OutgoingByExtension = Math.Round(100.0 * (totalOutgoingByExtension) / playerList.Count, ParserHelper.BuffDigit);
                    //
                    if (activePlayerCount > 0)
                    {
                        uptimeActive.Outgoing = Math.Round(100.0 * totalActiveOutgoing / activePlayerCount, ParserHelper.BuffDigit);
                        uptimeActive.OutgoingByExtension = Math.Round(100.0 * (totalActiveOutgoingByExtension) / activePlayerCount, ParserHelper.BuffDigit);
                    }
                }
                else if (buff.Type == BuffType.Intensity)
                {
                    uptime.Outgoing = Math.Round(totalOutgoing / playerList.Count, ParserHelper.BuffDigit);
                    uptime.OutgoingByExtension = Math.Round((totalOutgoingByExtension) / playerList.Count, ParserHelper.BuffDigit);
                    //
                    if (activePlayerCount > 0)
                    {
                        uptimeActive.Outgoing = Math.Round(totalActiveOutgoing / activePlayerCount, ParserHelper.BuffDigit);
                        uptimeActive.OutgoingByExtension = Math.Round((totalActiveOutgoingByExtension) / activePlayerCount, ParserHelper.BuffDigit);
                    }
                }
            }

            return new Dictionary<long, FinalActorBuffVolumes>[] { buffs, activeBuffs };
        }


        internal static Dictionary<long, FinalActorBuffVolumes>[] GetBuffVolumesForSelf(ParsedEvtcLog log, AbstractSingleActor actor, long start, long end)
        {
            var buffs = new Dictionary<long, FinalActorBuffVolumes>();
            var activeBuffs = new Dictionary<long, FinalActorBuffVolumes>();

            long phaseDuration = end - start;
            long playerActiveDuration = actor.GetActiveDuration(log, start, end);
            foreach (Buff buff in actor.GetTrackedBuffs(log))
            {

                double totalIncoming = 0;
                double totalIncomingByExtension = 0;
                double totalIncomingByUnknownExtension = 0;
                double totalOutgoing = 0;
                double totalOutgoingByExtension = 0;
                //
                double totalActiveIncoming = 0;
                double totalActiveIncomingByExtension = 0;
                double totalActiveIncomingByUnknownExtension = 0;
                double totalActiveOutgoing = 0;
                double totalActiveOutgoingByExtension = 0;
                foreach (AbstractBuffEvent abe in log.CombatData.GetBuffData(buff.ID))
                {
                    if (abe.Time >= start && abe.Time <= end && abe is AbstractBuffApplyEvent abae)
                    {
                        abae.TryFindSrc(log);
                        if (abe.CreditedBy == actor.AgentItem)
                        {
                            if (abae is BuffApplyEvent bae)
                            {
                                // We ignore infinite duration buffs
                                /*if (bae.AppliedDuration >= int.MaxValue)
                                {
                                    continue;
                                }*/
                                totalOutgoing += bae.AppliedDuration;
                            }
                            if (abae is BuffExtensionEvent bee)
                            {
                                totalOutgoingByExtension += bee.ExtendedDuration;
                            }
                        }
                        if (abe.To == actor.AgentItem)
                        {
                            if (abae is BuffApplyEvent bae)
                            {
                                // We ignore infinite duration buffs
                                /*if (bae.AppliedDuration >= int.MaxValue)
                                {
                                    continue;
                                }*/
                                totalIncoming += bae.AppliedDuration;
                            }
                            if (abae is BuffExtensionEvent bee)
                            {
                                totalIncomingByExtension += bee.ExtendedDuration;
                                if (abe.CreditedBy == ParserHelper._unknownAgent)
                                {
                                    totalIncomingByUnknownExtension += bee.ExtendedDuration;
                                }
                            }
                        }
                    }
                }
                totalIncoming += totalIncomingByExtension;
                totalOutgoing += totalOutgoingByExtension;

                if (playerActiveDuration > 0)
                {
                    totalActiveIncoming = totalIncoming / playerActiveDuration;
                    totalActiveOutgoing = totalOutgoing / playerActiveDuration;
                    totalActiveIncomingByExtension = totalIncomingByExtension / playerActiveDuration; 
                    totalActiveIncomingByUnknownExtension = totalIncomingByUnknownExtension / playerActiveDuration;
                    totalActiveOutgoingByExtension = totalOutgoingByExtension / playerActiveDuration;
                }

                totalIncoming /= phaseDuration;
                totalIncomingByExtension /= phaseDuration;
                totalIncomingByUnknownExtension /= phaseDuration;
                totalOutgoing /= phaseDuration;
                totalOutgoingByExtension /= phaseDuration;

                var uptime = new FinalActorBuffVolumes();
                var uptimeActive = new FinalActorBuffVolumes();
                buffs[buff.ID] = uptime;
                activeBuffs[buff.ID] = uptimeActive;
                if (buff.Type == BuffType.Duration)
                {
                    uptime.Incoming = Math.Round(100.0 * totalIncoming, ParserHelper.BuffDigit);
                    uptime.IncomingByExtension = Math.Round(100.0 * totalIncomingByExtension, ParserHelper.BuffDigit);
                    uptime.IncomingByUnknownExtension = Math.Round(100.0 * totalIncomingByUnknownExtension, ParserHelper.BuffDigit);
                    uptime.Outgoing = Math.Round(100.0 * totalOutgoing, ParserHelper.BuffDigit);
                    uptime.OutgoingByExtension = Math.Round(100.0 * (totalOutgoingByExtension), ParserHelper.BuffDigit);
                    //
                    if (playerActiveDuration > 0)
                    {
                        uptimeActive.Incoming = Math.Round(100.0 * totalActiveIncoming, ParserHelper.BuffDigit);
                        uptimeActive.IncomingByExtension = Math.Round(100.0 * totalActiveIncomingByExtension, ParserHelper.BuffDigit);
                        uptimeActive.IncomingByUnknownExtension = Math.Round(100.0 * totalActiveIncomingByUnknownExtension, ParserHelper.BuffDigit);
                        uptimeActive.Outgoing = Math.Round(100.0 * totalActiveOutgoing, ParserHelper.BuffDigit);
                        uptimeActive.OutgoingByExtension = Math.Round(100.0 * (totalActiveOutgoingByExtension), ParserHelper.BuffDigit);
                    }
                }
                else if (buff.Type == BuffType.Intensity)
                {
                    uptime.Incoming = Math.Round(totalIncoming, ParserHelper.BuffDigit);
                    uptime.IncomingByExtension = Math.Round(totalIncomingByExtension, ParserHelper.BuffDigit);
                    uptime.IncomingByUnknownExtension = Math.Round(totalIncomingByUnknownExtension, ParserHelper.BuffDigit);
                    uptime.Outgoing = Math.Round(totalOutgoing, ParserHelper.BuffDigit);
                    uptime.OutgoingByExtension = Math.Round((totalOutgoingByExtension) , ParserHelper.BuffDigit);
                    //
                    if (playerActiveDuration > 0)
                    {
                        uptimeActive.Incoming = Math.Round(totalActiveIncoming, ParserHelper.BuffDigit);
                        uptimeActive.IncomingByExtension = Math.Round(totalActiveIncomingByExtension, ParserHelper.BuffDigit);
                        uptimeActive.IncomingByUnknownExtension = Math.Round(totalActiveIncomingByUnknownExtension , ParserHelper.BuffDigit);
                        uptimeActive.Outgoing = Math.Round(totalActiveOutgoing , ParserHelper.BuffDigit);
                        uptimeActive.OutgoingByExtension = Math.Round((totalActiveOutgoingByExtension) , ParserHelper.BuffDigit);
                    }
                }
            }
            return new Dictionary<long, FinalActorBuffVolumes>[] { buffs, activeBuffs };
        }

    }

}
