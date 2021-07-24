using System;
using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;

namespace GW2EIBuilders.HtmlModels
{

    internal class EXTHealingStatsPhaseDto
    {

        public List<List<object>> OutgoingHealingStats { get; set; }
        public List<List<List<object>>> OutgoingHealingStatsFriendlies { get; set; }
        public List<List<object>> IncomingHealingStats { get; set; } 

        public EXTHealingStatsPhaseDto(PhaseData phase, ParsedEvtcLog log)
        {
            OutgoingHealingStats = BuildOutgoingHealingStatData(log, phase);
            OutgoingHealingStatsFriendlies = BuildOutgoingHealingFriendlyStatData(log, phase);
            IncomingHealingStats = BuildIncomingHealingStatData(log, phase);
        }


        // helper methods

        private static List<object> GetOutgoingHealingStatData(EXTFinalOutgoingHealingStat outgoingHealingStats)
        {
            var data = new List<object>
                {
                    outgoingHealingStats.Healing,
                    outgoingHealingStats.HealingPowerHealing,
                    outgoingHealingStats.ConversionHealing
                };
            return data;
        }

        private static List<object> GetIncomingHealingStatData(EXTFinalIncomingHealingStat incomingHealintStats)
        {
            var data = new List<object>
                {
                    incomingHealintStats.Healed,
                    incomingHealintStats.HealingPowerHealed,
                    incomingHealintStats.ConversionHealed,
                };
            return data;
        }
        public static List<List<object>> BuildOutgoingHealingStatData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<List<object>>(log.Friendlies.Count);
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                EXTFinalOutgoingHealingStat outgoingHealingStats = actor.EXTHealing.GetOutgoingHealStats(null, log, phase.Start, phase.End);
                list.Add(GetOutgoingHealingStatData(outgoingHealingStats));
            }
            return list;
        }

        public static List<List<List<object>>> BuildOutgoingHealingFriendlyStatData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<List<List<object>>>(log.Friendlies.Count);

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                var playerData = new List<List<object>>();

                foreach (AbstractSingleActor target in log.Friendlies)
                {
                    playerData.Add(GetOutgoingHealingStatData(actor.EXTHealing.GetOutgoingHealStats(target, log, phase.Start, phase.End)));
                }
                list.Add(playerData);
            }
            return list;
        } 

        public static List<List<object>> BuildIncomingHealingStatData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<List<object>>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                EXTFinalIncomingHealingStat incomingHealintStats = actor.EXTHealing.GetIncomingHealStats(null, log, phase.Start, phase.End);
                list.Add(GetIncomingHealingStatData(incomingHealintStats));
            }

            return list;
        }
    }
}
